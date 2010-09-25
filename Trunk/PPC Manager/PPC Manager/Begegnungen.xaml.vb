Class Begegnungen

    Private Sub CheckBox1_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox1.Checked
        Dim SpielPartien = Resources.Item("SpielPartien")
        Dim View = CollectionViewSource.GetDefaultView(SpielPartien)
        View.Filter = Function(o As Object) As Boolean
                          Dim partie As SpielPartie = o

                          Dim abgeschlossen = Aggregate x In partie.Sätze Into All(x.Abgeschlossen)
                          Return Not abgeschlossen
                      End Function
    End Sub

    Private Sub CheckBox1_Unchecked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox1.Unchecked
        Dim SpielPartien = Resources.Item("SpielPartien")
        Dim View = CollectionViewSource.GetDefaultView(SpielPartien)
        View.Filter = Nothing
    End Sub
End Class
