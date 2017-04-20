Imports System.Globalization

Class Begegnungen


    Public Property BegegnungenFiltern As Boolean = False

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub BegegnungenListView_Filter(ByVal sender As System.Object, ByVal e As System.Windows.Data.FilterEventArgs)
        If Not BegegnungenFiltern Then
            e.Accepted = True
            Return
        End If

        Dim partie As SpielPartie = CType(e.Item, SpielPartie)
        e.Accepted = Not partie.Abgeschlossen
    End Sub

    Private WithEvents Begegnungsliste As ListBox

    Private Sub NeuePartieAusgewählt(sender As Object, args As EventArgs) Handles Begegnungsliste.SelectionChanged, Begegnungsliste.MouseDown

        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        SpielerView.View.Refresh()
        DetailGrid.SetFocus()
    End Sub

    Friend Sub Update()
        Dim c = CType(FindResource("PartieView"), CollectionViewSource)
        c.View.Refresh()
    End Sub
End Class
