Imports Microsoft.Win32

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup

        Dim pfad = ÖffneDialog()
        Dim speicher As New Speicher(pfad)
        Dim spielerListe = New SpielerRepository(speicher.Klassements)
        spielerListe.Sync()
        Dim mainWindow As New MainWindow(
                New StartlistenController(spielerListe),
                                      speicher.KlassementNamen, AddressOf speicher.Speichern)
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
