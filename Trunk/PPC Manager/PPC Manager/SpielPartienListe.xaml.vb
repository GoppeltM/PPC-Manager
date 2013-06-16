Public Class SpielPartienListe
    Private Sub SpielPartie_Löschen_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        Dim PlayOffAktiv = DirectCast(FindResource("PlayoffAktiv"), Boolean)
        e.CanExecute = PlayOffAktiv AndAlso SpielPartienView.SelectedItem IsNot Nothing
    End Sub

    Private Sub SpielPartie_Löschen_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim AktuelleRunde = MainWindow.AktiveCompetition.SpielRunden.Peek()
        Dim SelektierteSpielpartie = DirectCast(SpielPartienView.SelectedItem, SpielPartie)
        AktuelleRunde.Remove(SelektierteSpielpartie)
    End Sub
End Class
