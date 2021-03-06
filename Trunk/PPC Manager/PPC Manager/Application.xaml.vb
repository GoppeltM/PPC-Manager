﻿Imports System.Collections.ObjectModel
Imports System.IO

Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.

    Private Sub Application_Exit(ByVal sender As Object, ByVal e As System.Windows.ExitEventArgs) Handles Me.Exit
        My.Settings.Save()
    End Sub

    Public Sub CrashBehandeln(sender As Object, args As UnhandledExceptionEventArgs)
        Dim nachricht = ""
        If TypeOf args.ExceptionObject Is FileNotFoundException Then
            Dim x = CType(args.ExceptionObject, FileNotFoundException)
            nachricht += x.FusionLog
        End If
        nachricht += args.ExceptionObject.ToString
        Dim f As New Fehler
        f.ExceptionText.Text = nachricht
        f.ShowDialog()
    End Sub

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CrashBehandeln
        Me.ShutdownMode = ShutdownMode.OnExplicitShutdown
        With New LadenNeu

            If Not .ShowDialog() Then
                Shutdown()
                Return
            End If

            Dim spielRegeln = New SpielRegeln(.GewinnsätzeAnzahl.Value, .SatzDiffCheck.IsChecked, .SonneBorn.IsChecked)
            Dim xmlPfad = .XMLPathText.Text
            Dim klassement = .CompetitionCombo.SelectedItem.ToString
            Dim AktiveCompetition As Competition
            Dim spielRunden = New SpielRunden
            Dim spielpartien = spielRunden.SelectMany(Function(m) m)
            Dim ausgeschiedeneIds = spielRunden.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs)
            Dim spielstand = New Spielstand(spielRegeln.Gewinnsätze)
            Dim spielverlauf = New Spielverlauf(spielpartien, ausgeschiedeneIds, spielstand)
            Try
                Dim doc = XDocument.Load(.XMLPathText.Text)
                AktiveCompetition = AusXML.CompetitionFromXML(xmlPfad, doc, klassement, spielRegeln, spielRunden)
            Catch ex As SpielDatenUnvollständigException
                MessageBox.Show(String.Format("Es gibt noch {0} Spieler dessen Anwesenheitsstatus unbekannt ist. Bitte korrigieren bevor das Turnier beginnt.", ex.UnvollständigCount),
                "Spieldaten unvollständig", MessageBoxButton.OK, MessageBoxImage.Error)
                Shutdown()
                Return
            End Try

            Resources("KlassementName") = AktiveCompetition.Altersgruppe
            Dim speichern = Sub() ZuXML.SaveXML(xmlPfad, spielRegeln, klassement, AktiveCompetition.SpielRunden)
            Dim excelVerlauf = New Spielverlauf(spielRunden.Skip(1).Reverse.SelectMany(Function(m) m),
                                            spielRunden.Skip(1).SelectMany(Function(m) m.AusgeschiedeneSpielerIDs),
                                            spielstand)
            Dim excelFabrik = New ExcelFabrik(spielstand, excelVerlauf)
            Dim vergleicher = New SpielerInfoComparer(spielverlauf, spielRegeln.SatzDifferenz, spielRegeln.SonneBornBerger)
            Dim r = New ReportFactory(xmlPfad,
                                      klassement,
                                      AktiveCompetition.SpielerListe,
                                      vergleicher,
                                      AktiveCompetition.SpielRunden,
                                      AddressOf excelFabrik.HoleDokument)
            Dim ausgeschiedeneSpielerIds = spielRunden.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs)

            Dim AktiveListe = From x In AktiveCompetition.SpielerListe
                              Where Not ausgeschiedeneSpielerIds.Contains(x.Id)
                              Select x
            Dim OrganisierePakete = Function()
                                        Dim spielverlaufCache = New SpielverlaufCache(spielverlauf)
                                        Dim comparer = New SpielerInfoComparer(spielverlaufCache, spielRegeln.SatzDifferenz, spielRegeln.SonneBornBerger)
                                        Dim paarungsSuche = New PaarungsSuche(Of SpielerInfo)(AddressOf comparer.Compare, AddressOf spielverlaufCache.Habengegeneinandergespielt)
                                        Dim begegnungen = New PaketBildung(Of SpielerInfo)(spielverlaufCache, AddressOf paarungsSuche.SuchePaarungen)
                                        Dim l = AktiveListe.ToList()
                                        l.Sort(comparer)
                                        l.Reverse()
                                        Return begegnungen.organisierePakete(l, spielRunden.Count - 1)
                                    End Function

            Dim spielerWrapped = From x In AktiveCompetition.SpielerListe
                                 Select New Spieler(x, spielverlauf, vergleicher)
            Dim druckFabrik = New FixedPageFabrik(
                spielerWrapped,
                spielRunden,
                klassement,
                spielstand)
            Dim controller = New MainWindowController(speichern,
                                                      r,
                                                      OrganisierePakete,
                                                      druckFabrik)
            Dim window = New MainWindow(controller,
                                        spielerWrapped,
                                        spielRunden,
                                        klassement,
                                        spielstand,
                                        vergleicher, New DruckerFabrik)
            MainWindow = window
            window.Show()
            ShutdownMode = ShutdownMode.OnMainWindowClose
        End With
    End Sub
End Class
