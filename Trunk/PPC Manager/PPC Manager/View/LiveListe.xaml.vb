Public Class LiveListe



    Private Sub Refresh_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MeineCommands.Playoff.CanExecute(Nothing, Me) Then
            LiveListe.SelectionMode = SelectionMode.Extended
        Else
            LiveListe.SelectionMode = SelectionMode.Single
        End If
        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        Dim v = DirectCast(SpielerView.View, ListCollectionView)
        v.CustomSort = New SpielerComparer
        SpielerView?.View?.Refresh()
    End Sub


    Private Sub LiveListe_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        ' Refresh_Executed(Nothing, Nothing)
    End Sub

    Private Class SpielerComparer
        Implements IComparer

        Public Function Compare1(x As Object, y As Object) As Integer Implements IComparer.Compare
            Return DirectCast(y, Spieler).CompareTo(DirectCast(x, Spieler))
        End Function
    End Class


End Class