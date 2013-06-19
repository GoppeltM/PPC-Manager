Public Class Fehler

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Clipboard.SetText(ExceptionText.Text)
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub
End Class
