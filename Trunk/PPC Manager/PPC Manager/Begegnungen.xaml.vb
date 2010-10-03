Class Begegnungen

    Private Sub CollectionViewSource1_Filter(ByVal sender As System.Object, ByVal e As System.Windows.Data.FilterEventArgs)
        If CheckBox1 Is Nothing OrElse Not CheckBox1.IsChecked Then
            e.Accepted = True
            Return
        End If

        Dim spielPartie As SpielPartie = e.Item
        Dim abgeschlossen = Aggregate x In spielPartie Into All(x.Abgeschlossen)
        e.Accepted = Not abgeschlossen
    End Sub

    Private Sub UpdateView(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox1.Click
        Dim c As CollectionViewSource = FindResource("MyView")
        c.View.Refresh()
    End Sub

End Class
