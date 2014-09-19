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
        Me.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose
        With New LadenNeu
            Me.MainWindow = Nothing
            If Not .ShowDialog() Then
                Me.Shutdown()
                Return
            End If

            Dim spielRegeln = New SpielRegeln(.GewinnsätzeAnzahl.Value, .SatzDiffCheck.IsChecked, .SonneBorn.IsChecked)
            Dim AktiveCompetition = Competition.FromXML(.XMLPathText.Text, .CompetitionCombo.SelectedItem.ToString, spielRegeln)

            Application.Current.Resources("KlassementName") = AktiveCompetition.Altersgruppe
            Dim window = New MainWindow(AktiveCompetition, AktiveCompetition.Altersgruppe)
            Me.MainWindow = window
            window.Visibility = Visibility.Visible
            window.Show()
        End With

    End Sub
End Class
