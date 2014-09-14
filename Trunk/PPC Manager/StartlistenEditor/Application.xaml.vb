Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        Dim mainWindow As New MainWindow(New StartlistenController())
        mainWindow.Show()
    End Sub
End Class
