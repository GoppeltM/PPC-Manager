Class Begegnungen

    Friend Property BegegnungenView As CollectionViewSource

    Private Sub BegegnungenListView_Filter(ByVal sender As System.Object, ByVal e As System.Windows.Data.FilterEventArgs)

        If Not My.Settings.BegegnungenAbgeschlossen Then Return

        Dim partie As SpielPartie = CType(e.Item, SpielPartie)
        e.Accepted = Not partie.Abgeschlossen

    End Sub

    Private Sub Begegnungen_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        BegegnungenView = CType(FindResource("PartieView"), CollectionViewSource)
        BegegnungenView.View.MoveCurrentToLast()
    End Sub
End Class

Class RundenAnzeige
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Return String.Format("Runde: {0}", value)
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class