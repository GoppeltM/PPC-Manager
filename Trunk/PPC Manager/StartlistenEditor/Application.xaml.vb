Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        Dim s = New SpielerListe
        Dim k = New KlassementListe
        Dim mainWindow As New MainWindow(
            New StartlistenController(s, k), s, k)
        mainWindow.Show()
    End Sub
End Class
