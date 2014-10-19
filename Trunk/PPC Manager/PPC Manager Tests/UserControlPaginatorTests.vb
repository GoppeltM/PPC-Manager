Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Controls

<TestClass()> Public Class UserControlPaginatorTests

    <TestInitialize>
    Public Sub Init()
        Dim ranglisteFactory = Function() New RanglisteSeite("Herren D", 3)
        Dim size = New Size() With {.Height = 200, .Width = 300}
        _SpielerListe = New List(Of Spieler)
        _RangListePaginator = New UserControlPaginator(Of RanglisteSeite)(_SpielerListe, size, ranglisteFactory)
    End Sub

    Private _RangListePaginator As UserControlPaginator(Of RanglisteSeite)
    Private _SpielerListe As New List(Of Spieler)

    <TestMethod()>
    Public Sub GetUserControlPage_Seite0KeinInhalt_StandardSpaltenBreite()        
        Dim p1 = _RangListePaginator.GetUserControlPage(0).Item1
        Dim datagrid = FindChild(Of DataGrid)(p1, "SpielerRangListe")
        Dim width = datagrid.Columns(1).ActualWidth
        Assert.AreEqual(58.16, width, 0.01)
    End Sub

    <TestMethod()>
    Public Sub GetUserControlPage_Seite0BreiterVorname_BreiteSpalte()        
        Dim s As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        s.Id = "-1"
        s.Vorname = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"        
        _SpielerListe.Add(s)        
        Dim p1 = _RangListePaginator.GetUserControlPage(0).Item1
        Dim datagrid = FindChild(Of DataGrid)(p1, "SpielerRangListe")
        Dim width = datagrid.Columns(1).ActualWidth
        Assert.AreEqual(219.73, width, 0.01)
    End Sub

    <TestMethod()>
    Public Sub GetUserControlPage_EinBreiterVornameNachname_BeideSeitenGleichBreit()
        Dim Erster As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        Erster.Id = "-1"
        Erster.Vorname = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        _SpielerListe.Add(Erster)
        For Each i In Enumerable.Range(0, 10)
            Dim x = New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
            x.Id = i.ToString
            _SpielerListe.Add(x)
        Next

        Dim Letzter As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        Letzter.Id = "-2"
        Letzter.Nachname = "bbbbbbbbbbbbbbbbbbbbbbb"
        _SpielerListe.Add(Letzter)
        Dim p1 = _RangListePaginator.GetUserControlPage(0).Item1        
        Dim p3 = _RangListePaginator.GetUserControlPage(2).Item1
        SpaltenAngleichen(New RanglisteSeite() {p1, p3})        
        Dim d1 = FindChild(Of DataGrid)(p1, "SpielerRangListe")
        Dim d3 = FindChild(Of DataGrid)(p3, "SpielerRangListe")
        Dim width1 = d1.Columns(1).ActualWidth
        Dim width3 = d3.Columns(2).ActualWidth
        Assert.AreEqual(219.73, width1, 0.01)
        Assert.AreEqual(168.22, width3, 0.01)
    End Sub

    Private Sub SpaltenAngleichen(seiten As IEnumerable(Of RanglisteSeite))
        Dim ColumnsOberesLimit As New Dictionary(Of Integer, Double)
        For Each seite In seiten
            Dim datagrid = FindChild(Of DataGrid)(seite, "SpielerRangListe")
            For Each column In datagrid.Columns
                If Not ColumnsOberesLimit.ContainsKey(column.DisplayIndex) Then
                    ColumnsOberesLimit.Add(column.DisplayIndex, column.ActualWidth)
                End If
                If column.ActualWidth > ColumnsOberesLimit.Item(column.DisplayIndex) Then
                    ColumnsOberesLimit.Item(column.DisplayIndex) = column.ActualWidth
                End If
            Next
        Next

        For Each seite In seiten
            Dim datagrid = FindChild(Of DataGrid)(seite, "SpielerRangListe")
            For Each column In ColumnsOberesLimit
                datagrid.Columns(column.Key).MinWidth = column.Value
            Next
            seite.UpdateLayout()
        Next

    End Sub

    Private Function FindChild(Of T As FrameworkElement)(item As DependencyObject, name As String) As T
        Dim value = item.GetValue(Controls.Control.NameProperty).ToString
        If TypeOf item Is T And value = name Then
            Return CType(item, T)
        End If
        Dim childCount = VisualTreeHelper.GetChildrenCount(item)
        For Each index In Enumerable.Range(0, childCount)
            Dim child = VisualTreeHelper.GetChild(item, index)
            Dim c = FindChild(Of T)(child, name)
            If c IsNot Nothing Then
                Return c
            End If
        Next
        Return Nothing
    End Function

End Class