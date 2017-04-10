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


    Private Property _Controller As IController

    Private Sub Begegnungen_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        _Controller = DirectCast(DataContext, IController)
        DetailGrid.Controller = _Controller
        If _Controller Is Nothing Then Return
        Dim res = CType(FindResource("RanglisteDataProvider"), ObjectDataProvider)        
        res.ObjectInstance = _Controller.AktiveCompetition.SpielerListe
        BegegnungenView = CType(FindResource("PartieView"), CollectionViewSource)
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        ViewSource.Source = _Controller.AktiveCompetition.SpielRunden
        ViewSource.View.Refresh()
        Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist.
        ViewSource.View.MoveCurrentToFirst()
        Begegnungsliste = SpielPartienListe.SpielPartienView
    End Sub



    Private Sub BegegnungenFiltern_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        If BegegnungenView.View Is Nothing Then Return
        BegegnungenView.View.Refresh()
    End Sub




    Private Sub Refresh_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)

        'Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist
        ViewSource.View.Refresh()
        ViewSource.View.MoveCurrentToFirst()
    End Sub

    Private WithEvents Begegnungsliste As ListBox

    Private Sub NeuePartieAusgewählt(sender As Object, args As EventArgs) Handles Begegnungsliste.SelectionChanged, Begegnungsliste.MouseDown
        If BegegnungenView.View IsNot Nothing Then
            BegegnungenView.View.Refresh()
            Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
            SpielerView.View.Refresh()
        End If
        DetailGrid.SetFocus()
    End Sub

End Class
