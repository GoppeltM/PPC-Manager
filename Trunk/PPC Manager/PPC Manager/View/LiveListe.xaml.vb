Public Class LiveListe

    Public Property SpielerComparer As IComparer

    Private Sub Refresh_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MeineCommands.Playoff.CanExecute(Nothing, Me) Then
            LiveListe.SelectionMode = SelectionMode.Extended
        Else
            LiveListe.SelectionMode = SelectionMode.Single
        End If
        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        Dim v = DirectCast(SpielerView.View, ListCollectionView)
        If v Is Nothing Then Return
        v.CustomSort = SpielerComparer
        v.Refresh()
    End Sub

End Class