Imports System.Globalization

Public Class SpielPartienListe


    Private Sub SpielPartie_Löschen_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = MeineCommands.Playoff.CanExecute(Nothing, Me) AndAlso SpielPartienView.SelectedItem IsNot Nothing
    End Sub

    Private Sub Refresh_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)

        'Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist
        ViewSource.View.Refresh()
        ViewSource.View.MoveCurrentToFirst()
    End Sub

    Private Sub SpielPartie_Löschen_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim box As ListBox = DirectCast(sender, ListBox)
        Dim list As ListCollectionView = CType(box.ItemsSource, ListCollectionView)
        list.Remove(box.SelectedItem)
    End Sub

    Private Sub SpielPartienView_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SpielPartienView.SelectionChanged
        MeineCommands.PartieAusgewählt.Execute(SpielPartienView.SelectedItem, Me)
    End Sub
End Class

