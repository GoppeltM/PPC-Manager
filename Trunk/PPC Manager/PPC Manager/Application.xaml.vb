Imports System.Collections.ObjectModel
Imports System.IO
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.

    Private Sub Application_Exit(ByVal sender As Object, ByVal e As System.Windows.ExitEventArgs) Handles Me.Exit
        My.Settings.Save()
    End Sub

    Public xmlPfad As String

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

    Private Function ParseSpielregeln(doc As XDocument, klassement As String) As SpielRegeln
        Dim competition = (From x In doc.Root.<competition>
                           Where x.Attribute("age-group").Value = klassement Select x).Single

        Dim GewinnsätzeAnzahl As Double = 3
        Dim SatzDiffCheck As Boolean = True
        Dim SonneBorn As Boolean = True

        If competition.@ppc:gewinnsätze IsNot Nothing Then
            GewinnsätzeAnzahl = Double.Parse(competition.@ppc:gewinnsätze)
        End If

        If competition.@ppc:satzdifferenz IsNot Nothing Then
            SatzDiffCheck = Boolean.Parse(competition.@ppc:satzdifferenz)
        End If

        If competition.@ppc:sonnebornberger IsNot Nothing Then
            SonneBorn = Boolean.Parse(competition.@ppc:sonnebornberger)
        End If


        Return New SpielRegeln(GewinnsätzeAnzahl, SatzDiffCheck, SonneBorn)
    End Function

    Public Sub LadeCompetition(sender As Object, klassement As String)
        Dim doc = XDocument.Load(xmlPfad)
        Dim AlleCompetitions = New Collection(Of String)(doc.Root.<competition>.Select(Function(x) x.Attribute("age-group").Value).ToList)
        Dim spielRegeln = ParseSpielregeln(doc, klassement)
        Dim AktiveCompetition As Competition
        Dim spielRunden = New SpielRunden
        Dim spielpartien = spielRunden.SelectMany(Function(m) m)
        Dim ausgeschiedeneIds = spielRunden.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs)
        Dim spielstand = New Spielstand(SpielRegeln.Gewinnsätze)
        Dim spielverlauf = New Spielverlauf(spielpartien, ausgeschiedeneIds, spielstand)

        Try
            AktiveCompetition = AusXML.CompetitionFromXML(xmlPfad, doc, klassement, spielRegeln, spielRunden)

        Catch ex As SpielDatenUnvollständigException
            MessageBox.Show(String.Format("Es gibt noch {0} Spieler dessen Anwesenheitsstatus unbekannt ist. Bitte korrigieren bevor das Turnier beginnt.", ex.UnvollständigCount),
            "Spieldaten unvollständig", MessageBoxButton.OK, MessageBoxImage.Error)
            Shutdown()
            Return
        End Try

        Resources("KlassementName") = AktiveCompetition.Altersgruppe
        Dim speichern = Sub() ZuXML.SaveXML(xmlPfad, SpielRegeln, klassement, AktiveCompetition.SpielRunden)
        Dim excelVerlauf = New Spielverlauf(spielRunden.Skip(1).Reverse.SelectMany(Function(m) m),
                                        spielRunden.Skip(1).SelectMany(Function(m) m.AusgeschiedeneSpielerIDs),
                                        spielstand)
        Dim excelFabrik = New ExcelFabrik(spielstand, excelVerlauf)
        Dim vergleicher = New SpielerInfoComparer(spielverlauf, SpielRegeln.SatzDifferenz, SpielRegeln.SonneBornBerger)
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
                                    Dim comparer = New SpielerInfoComparer(spielverlaufCache, SpielRegeln.SatzDifferenz, SpielRegeln.SonneBornBerger)
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
        Dim oldWindow = MainWindow

        Dim window = New MainWindow(controller,
                                    spielerWrapped,
                                    spielRunden,
                                    klassement,
                                    spielstand,
                                    vergleicher,
                                    New DruckerFabrik,
                                    AlleCompetitions)

        MainWindow = window

        'copy position and size from old window to new window
        If oldWindow IsNot Nothing Then
            window.Left = oldWindow.Left
            window.Top = oldWindow.Top
            window.Width = oldWindow.Width
            window.Height = oldWindow.Height
        End If

        window.Show()
        If oldWindow IsNot Nothing Then
            oldWindow.Close()
        End If
    End Sub

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CrashBehandeln
        ShutdownMode = ShutdownMode.OnExplicitShutdown
        With New LadenNeu

            If Not .ShowDialog() Then
                Shutdown()
                Return
            End If

            xmlPfad = .XMLPathText.Text

            LadeCompetition(sender, .CompetitionCombo.SelectedItem.ToString)

            ShutdownMode = ShutdownMode.OnMainWindowClose
        End With
    End Sub
End Class
