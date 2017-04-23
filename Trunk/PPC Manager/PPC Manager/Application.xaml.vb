Imports System.Collections.ObjectModel

Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.

    Private Sub Application_Exit(ByVal sender As Object, ByVal e As System.Windows.ExitEventArgs) Handles Me.Exit
        My.Settings.Save()
    End Sub

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        AddHandler AppDomain.CurrentDomain.UnhandledException, Sub(sender2 As Object, args As UnhandledExceptionEventArgs)
                                                                   Dim f As New Fehler
                                                                   f.ExceptionText.Text = args.ExceptionObject.ToString
                                                                   f.ShowDialog()
                                                               End Sub
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
            Dim ausgeschiedeneSpieler = From x In spielRunden.AusgeschiedeneSpieler Select x.Spieler
            Dim spielverlauf = New Spielverlauf(spielpartien, ausgeschiedeneSpieler, spielRegeln)
            Try
                Dim doc = XDocument.Load(.XMLPathText.Text)
                AktiveCompetition = AusXML.CompetitionFromXML(xmlPfad, doc, klassement, spielRegeln, spielverlauf, spielRunden)
            Catch ex As SpielDatenUnvollständigException
                MessageBox.Show(String.Format("Es gibt noch {0} Spieler dessen Anwesenheitsstatus unbekannt ist. Bitte korrigieren bevor das Turnier beginnt.", ex.UnvollständigCount),
                "Spieldaten unvollständig", MessageBoxButton.OK, MessageBoxImage.Error)
                Shutdown()
                Return
            End Try

            Resources("KlassementName") = AktiveCompetition.Altersgruppe
            Dim speichern = Sub() ZuXML.SaveXML(xmlPfad, spielRegeln, klassement, AktiveCompetition.SpielRunden)
            Dim r = New ReportFactory(xmlPfad,
                                      klassement,
                                      AktiveCompetition.SpielerListe,
                                      AktiveCompetition.SpielRunden,
                                      spielRegeln)
            Dim habenGegeinanderGespielt = Function(a As SpielerInfo, b As SpielerInfo) spielverlauf.Habengegeneinandergespielt(a, b)

            Dim OrganisierePakete = Function(spielerListe As IEnumerable(Of SpielerInfo), spielrunde As Integer)
                                        Dim spielverlaufCache = New SpielverlaufCache(spielverlauf)
                                        Dim comparer = New SpielerInfoComparer(spielverlaufCache)
                                        Dim paarungsSuche = New PaarungsSuche(Of SpielerInfo)(AddressOf comparer.Compare, habenGegeinanderGespielt)
                                        Dim begegnungen = New PaketBildung(Of SpielerInfo)(spielverlaufCache, AddressOf paarungsSuche.SuchePaarungen)
                                        Dim l = spielerListe.ToList()
                                        l.Sort(comparer)
                                        l.Reverse()
                                        Return begegnungen.organisierePakete(l, spielrunde)
                                    End Function
            Dim druckFabrik = New FixedPageFabrik(
                AktiveCompetition.SpielerListe,
                spielRunden,
                spielverlauf,
                klassement)
            Dim controller = New MainWindowController(AktiveCompetition.SpielerListe,
                                                      spielRunden,
                                                      speichern,
                                                      r,
                                                      OrganisierePakete,
                                                      druckFabrik,
                                                      spielRegeln.Gewinnsätze)
            Dim window = New MainWindow(controller, AktiveCompetition.SpielerListe, spielRunden, klassement)
            MainWindow = window
            window.Show()
            ShutdownMode = ShutdownMode.OnMainWindowClose
        End With

    End Sub
End Class
