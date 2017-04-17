Public Class LiveListe

    Private Sub Ausscheiden_CanExecute(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        If LiveListe.SelectedIndex <> -1 Then
            Dim Spieler = CType(LiveListe.SelectedItem, Spieler)
            If Not Spieler.Ausgeschieden Then
                e.CanExecute = True
                Return
            End If
        End If
        e.CanExecute = False
    End Sub

    Private Sub Refresh_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MeineCommands.Playoff.CanExecute(Nothing, Me) Then
            LiveListe.SelectionMode = SelectionMode.Extended
            PartieErstellenMenu.IsEnabled = True
        Else
            LiveListe.SelectionMode = SelectionMode.Single
            PartieErstellenMenu.IsEnabled = False
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

    Private Sub NeuePartie_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        If Not MeineCommands.Playoff.CanExecute(Nothing, Me) Then
            e.CanExecute = False
            Return
        End If
        e.CanExecute = LiveListe.SelectedItems.Count = 2
    End Sub

    Private Sub NeuePartie_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim spieler = LiveListe.SelectedItems.OfType(Of SpielerInfo)
        Dim c = ApplicationCommands.[New]
        c.Execute(spieler, Me)
    End Sub

    Private Sub Ausscheiden_Execute(sender As Object, e As ExecutedRoutedEventArgs)
        Dim spieler = CType(LiveListe.SelectedItem, SpielerInfo)
        If Not MessageBox.Show(
            String.Format("Sind Sie sicher dass sie Spieler {0} ausscheiden lassen wollen? Dieser Vorgang kann nicht rückgängig gemacht werden!",
                          spieler.Nachname),
                        "Spieler ausscheiden?", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
            Return
        End If
        Dim c = ApplicationCommands.Delete()
        c.Execute(spieler, Me)
    End Sub
End Class