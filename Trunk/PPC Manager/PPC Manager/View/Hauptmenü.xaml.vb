Public Class Hauptmenü
    Private Sub MenuItem_Click(sender As Object, e As RoutedEventArgs)

        Dim dialog = New UrkundeManuellDialog()

        dialog.ShowDialog()
    End Sub
End Class
