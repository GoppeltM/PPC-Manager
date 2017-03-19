Imports System.Globalization

Class Begegnungen

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Friend Property BegegnungenView As CollectionViewSource

    Private Sub BegegnungenListView_Filter(ByVal sender As System.Object, ByVal e As System.Windows.Data.FilterEventArgs)

        If Not My.Settings.BegegnungenFiltern Then
            e.Accepted = True
            Return
        End If

        Dim partie As SpielPartie = CType(e.Item, SpielPartie)
        e.Accepted = Not partie.Abgeschlossen

    End Sub

    Private Class SpielerComparer
        Implements IComparer

        Public Function Compare1(x As Object, y As Object) As Integer Implements IComparer.Compare
            Return DirectCast(y, Spieler).CompareTo(DirectCast(x, Spieler))
        End Function
    End Class

    Private Property _Controller As IController

    Private Sub Begegnungen_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        _Controller = DirectCast(DataContext, IController)
        If _Controller Is Nothing Then Return
        Dim res = CType(FindResource("RanglisteDataProvider"), ObjectDataProvider)        
        res.ObjectInstance = _Controller.AktiveCompetition.SpielerListe
        BegegnungenView = CType(FindResource("PartieView"), CollectionViewSource)
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        ViewSource.Source = _Controller.AktiveCompetition.SpielRunden
        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        Dim v = DirectCast(SpielerView.View, ListCollectionView)
        v.CustomSort = New SpielerComparer
        SpielerView.View.Refresh()
        ViewSource.View.Refresh()
        Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist.
        ViewSource.View.MoveCurrentToFirst()
        SetFocus()
        Begegnungsliste = SpielPartienListe.SpielPartienView
    End Sub

    Private Sub Ausscheiden_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        If LifeListe.SelectedIndex <> -1 Then
            Dim Spieler = CType(LifeListe.SelectedItem, Spieler)
            If Not Spieler.Ausgeschieden Then
                e.CanExecute = True
                Return
            End If
        End If
        e.CanExecute = False
    End Sub

    Private Sub Ausscheiden_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)        
        Dim Spieler = CType(LifeListe.SelectedItem, Spieler)
        If MessageBox.Show(String.Format("Sind Sie sicher dass sie Spieler {0} ausscheiden lassen wollen? Dieser Vorgang kann nicht rückgängig gemacht werden!", Spieler.Nachname),
                        "Spieler ausscheiden?", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
            _Controller.SpielerAusscheiden(Spieler)

        End If
    End Sub

    Private Sub BegegnungenFiltern_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        If BegegnungenView.View Is Nothing Then Return
        BegegnungenView.View.Refresh()
    End Sub

    Private Sub NeuePartie_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = DirectCast(FindResource("PlayoffAktiv"), Boolean) AndAlso LifeListe.SelectedItems.Count = 2
    End Sub

    Private Sub NeuePartie_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim dialog = New NeueSpielPartieDialog
        dialog.RundenNameTextBox.Text = "Runde " & _Controller.AktiveCompetition.SpielRunden.Count
        If Not dialog.ShowDialog Then Return
        Dim rundenName = dialog.RundenNameTextBox.Text
        Dim AusgewählteSpieler = LifeListe.SelectedItems
        Dim spielerA = CType(AusgewählteSpieler(0), Spieler)
        Dim SpielerB = CType(AusgewählteSpieler(1), Spieler)
        _Controller.NeuePartie(rundenName, spielerA, SpielerB)        
    End Sub


    Private Sub RefreshView()
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        'Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist
        ViewSource.View.Refresh()
        ViewSource.View.MoveCurrentToFirst()
        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        SpielerView.View.Refresh()
    End Sub

    Private Sub Refresh_Executed(sender As Object, e As ExecutedRoutedEventArgs)        
        If DirectCast(FindResource("PlayoffAktiv"), Boolean) Then
            LifeListe.SelectionMode = SelectionMode.Extended
        Else
            LifeListe.SelectionMode = SelectionMode.Single
        End If
        RefreshView()
    End Sub

    Private Sub CommandBinding_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Satzbearbeiten(True)
    End Sub

    Private Sub SetFocus()
        Punkte.Text = "0"
        Punkte.Focus()
        Punkte.SelectAll()
    End Sub

    Private Sub Satzbearbeiten(inverted As Boolean)
        If Not Integer.TryParse(Punkte.Text, Nothing) Then
            Punkte.Text = "0"
            SetFocus()
            Return
        End If

        Dim value = Integer.Parse(Punkte.Text)
        Dim partie = DirectCast(DetailGrid.DataContext, SpielPartie)

        _Controller.SatzEintragen(value, inverted, partie)

        SetFocus()
    End Sub

    Private Sub CommandBinding_Executed_1(sender As Object, e As ExecutedRoutedEventArgs)
        Satzbearbeiten(False)
    End Sub

    Private Sub CommandBinding_Executed_2(sender As Object, e As ExecutedRoutedEventArgs)
        DirectCast(DetailGrid.DataContext, SpielPartie).Remove(CType(Sätze.SelectedItem, Satz))
    End Sub

    Private Sub Punkte_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles Punkte.PreviewTextInput
        Dim i As Integer
        e.Handled = Not Integer.TryParse(e.Text, i)
    End Sub


    Private Sub NeuerSatz_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)

        Dim s = TryCast(DetailGrid.DataContext, SpielPartie)
        If s Is Nothing Then Return
        e.CanExecute = _Controller.NeuerSatz_CanExecute(s)        
    End Sub

    Private WithEvents Begegnungsliste As ListBox

    Private Sub NeuePartieAusgewählt(sender As Object, args As EventArgs) Handles Begegnungsliste.SelectionChanged, Begegnungsliste.MouseDown
        If BegegnungenView.View IsNot Nothing Then
            BegegnungenView.View.Refresh()
            Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
            SpielerView.View.Refresh()
        End If

        SetFocus()
    End Sub

    Private Sub SpielerView_Filter(sender As Object, e As FilterEventArgs)
        Dim s As Spieler = DirectCast(e.Item, Spieler)
        If _Controller Is Nothing Then Return
        e.Accepted = _Controller.FilterSpieler(s)        
    End Sub
End Class



Public Class DoppelbreitenGrid
    Inherits Grid

    'Protected Overrides Function MeasureOverride(constraint As Size) As Size
    '    MyBase.MeasureOverride(constraint)

    '    Return New Size(400, 60)
    'End Function

    'Protected Overrides Function ArrangeOverride(arrangeSize As Size) As Size
    '    Dim size = MyBase.ArrangeOverride(arrangeSize)

    '    Return New Size(400, 60)
    'End Function
End Class
