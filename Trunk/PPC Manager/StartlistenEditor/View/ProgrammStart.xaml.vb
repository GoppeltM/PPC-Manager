Public Class ProgrammStart

    Public Property NeuesTurnier As Boolean = False

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        NeuesTurnier = True
        DialogResult = True
        Close()
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        NeuesTurnier = False
        DialogResult = True
        Close()
    End Sub
End Class
