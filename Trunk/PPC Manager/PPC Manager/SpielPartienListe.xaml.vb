﻿Imports System.Globalization

Public Class SpielPartienListe
    Private Sub SpielPartie_Löschen_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        Dim PlayOffAktiv = DirectCast(FindResource("PlayoffAktiv"), Boolean)
        e.CanExecute = PlayOffAktiv AndAlso SpielPartienView.SelectedItem IsNot Nothing
    End Sub

    Private Sub SpielPartie_Löschen_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim box As ListBox = DirectCast(sender, ListBox)
        Dim list As ListCollectionView = CType(box.ItemsSource, ListCollectionView)
        list.Remove(box.SelectedItem)        
    End Sub
End Class

