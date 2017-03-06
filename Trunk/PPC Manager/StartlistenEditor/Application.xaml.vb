Imports System.Collections.ObjectModel
Imports Microsoft.Win32

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private repository As SpielerRepository

    Private Sub Application_Exit(sender As Object, e As ExitEventArgs) Handles Me.[Exit]
        repository.Deregister()
    End Sub

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup

        Dim pfad = ÖffneDialog()
        Dim speicher As New Speicher(pfad)
        Dim cache As New SpeicherCache(speicher)
        Dim observable = New StartlistenController(speicher.LeseSpieler())
        repository = New SpielerRepository(cache, observable, observable)
        Dim mainWindow As New MainWindow(observable,
                                      cache.KlassementNamen, AddressOf cache.SpeichereAlles)
        mainWindow.Show()
    End Sub

    Private Function ÖffneDialog() As String
        With New OpenFileDialog
            .Filter = "Click-TT Turnierdaten|*.xml"
            If Not .ShowDialog Then
                Shutdown()
                Return ""
            End If
            Return .FileName
        End With
    End Function
End Class
