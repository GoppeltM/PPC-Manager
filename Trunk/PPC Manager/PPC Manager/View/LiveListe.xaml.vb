Imports System.Globalization

Public Class LiveListe

    Public Property PlayoffAktiv As Boolean = False

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
        If PlayoffAktiv Then
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

    Private Sub NeuePartie_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = PlayoffAktiv AndAlso LiveListe.SelectedItems.Count = 2
    End Sub

    Private Sub LiveListe_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Refresh_Executed(Nothing, Nothing)
    End Sub

    Private Class SpielerComparer
        Implements IComparer

        Public Function Compare1(x As Object, y As Object) As Integer Implements IComparer.Compare
            Return DirectCast(y, Spieler).CompareTo(DirectCast(x, Spieler))
        End Function
    End Class

    Private Sub AusscheidenMenu_Click(sender As Object, e As RoutedEventArgs) Handles AusscheidenMenu.Click
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

    Private Sub PartieErstellenMenu_Click(sender As Object, e As RoutedEventArgs) Handles PartieErstellenMenu.Click
        Dim spieler = LiveListe.SelectedItems.OfType(Of SpielerInfo)
        Dim c = ApplicationCommands.[New]
        c.Execute(Spieler, Me)
    End Sub

    Private Sub LiveListe_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles LiveListe.SelectionChanged
        Dim liste = LiveListe.SelectedItems.OfType(Of Spieler)
        AusscheidenMenu.IsEnabled = False
        PartieErstellenMenu.IsEnabled = False
        If liste.Count = 1 Then
            AusscheidenMenu.IsEnabled = Not liste.First.Ausgeschieden
        End If
        If liste.Count = 2 Then
            PartieErstellenMenu.IsEnabled = PlayoffAktiv
        End If

    End Sub
End Class