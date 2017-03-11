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
        Me.ShutdownMode = ShutdownMode.OnMainWindowClose
        With New LadenNeu
            Me.MainWindow = Nothing
            If Not .ShowDialog() Then
                Shutdown()
                Return
            End If

            Dim spielRegeln = New SpielRegeln(.GewinnsätzeAnzahl.Value, .SatzDiffCheck.IsChecked, .SonneBorn.IsChecked)
            Dim xmlPfad = .XMLPathText.Text
            Dim klassement = .CompetitionCombo.SelectedItem.ToString
            Dim AktiveCompetition As Competition
            Try
                Dim doc = XDocument.Load(.XMLPathText.Text)
                AktiveCompetition = AusXML.CompetitionFromXML(xmlPfad, doc, klassement, spielRegeln)
            Catch ex As SpielDatenUnvollständigException
                MessageBox.Show(String.Format("Es gibt noch {0} Spieler dessen Anwesenheitsstatus unbekannt ist. Bitte korrigieren bevor das Turnier beginnt.", ex.UnvollständigCount), _
                "Spieldaten unvollständig", MessageBoxButton.OK, MessageBoxImage.Error)
                Application.Current.Shutdown()
                Return
            End Try

            Resources("KlassementName") = AktiveCompetition.Altersgruppe
            Dim speichern = Sub() ZuXML.SaveXML(xmlPfad, spielRegeln, klassement, AktiveCompetition.SpielRunden)
            Dim controller = New MainWindowController(AktiveCompetition, speichern)
            Dim window = New MainWindow(controller)
            Me.MainWindow = window
            window.Visibility = Visibility.Visible
            window.Show()
        End With

    End Sub
End Class
