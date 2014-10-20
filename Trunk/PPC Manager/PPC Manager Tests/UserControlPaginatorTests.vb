Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Controls

<TestClass()> Public Class UserControlPaginatorTests

    <TestInitialize>
    Public Sub Init()        
    End Sub

    Private Function CreateRanglistePaginator(spielerListe As IEnumerable(Of Spieler)) As UserControlPaginator(Of RanglisteSeite)
        Dim ranglisteFactory = Function() New RanglisteSeite("Herren D", 3)
        Dim size = New Size() With {.Height = 200, .Width = 300}
        Return New UserControlPaginator(Of RanglisteSeite)(spielerListe, size, ranglisteFactory)
    End Function


    <TestMethod()>
    <ExpectedException(GetType(ArgumentOutOfRangeException))>
    Public Sub GetUserControlPage_Seite0KeinInhalt_ArgumentOutOfRangeException()
        Dim paginator = CreateRanglistePaginator(New List(Of Spieler))
        Dim p1 = paginator.GetUserControlPage(0)
    End Sub

    <TestMethod()>
    Public Sub GetUserControlPage_Seite0EinEintrag_StandardSpaltenBreite()
        Dim s As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        s.Id = "-1"

        Dim paginator = CreateRanglistePaginator(New List(Of Spieler) From {s})
        Dim p1 = paginator.GetUserControlPage(0)
        Dim datagrid = DruckenTools.FindChild(Of DataGrid)(p1, "SpielerRangListe")
        Dim width = datagrid.Columns(1).ActualWidth
        Assert.AreEqual(58.16, width, 0.01)
    End Sub

    <TestMethod()>
    Public Sub GetUserControlPage_Seite0BreiterVorname_BreiteSpalte()        
        Dim s As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        s.Id = "-1"
        s.Vorname = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"        

        Dim paginator = CreateRanglistePaginator(New Spieler() {s})
        Dim p1 = paginator.GetUserControlPage(0)
        Dim datagrid = DruckenTools.FindChild(Of DataGrid)(p1, "SpielerRangListe")
        Dim width = datagrid.Columns(1).ActualWidth
        Assert.AreEqual(219.73, width, 0.01)
    End Sub

    <TestMethod()>
    Public Sub GetUserControlPage_EinBreiterVornameNachname_BeideSeitenGleichBreit()
        Dim Erster As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        Erster.Id = "-1"
        Erster.Vorname = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        Dim spielerListe As New List(Of Spieler)
        spielerListe.Add(Erster)
        For Each i In Enumerable.Range(0, 10)
            Dim x = New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
            x.Id = i.ToString
            spielerListe.Add(x)
        Next

        Dim Letzter As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        Letzter.Id = "-2"
        Letzter.Nachname = "bbbbbbbbbbbbbbbbbbbbbbb"
        spielerListe.Add(Letzter)

        Dim paginator = CreateRanglistePaginator(spielerListe)
        Dim p1 = paginator.GetUserControlPage(0)
        Dim last = paginator.GetUserControlPage(2)
        DruckenTools.SpaltenAngleichen(New RanglisteSeite() {p1, last}, "SpielerRangListe")
        Dim d1 = DruckenTools.FindChild(Of DataGrid)(p1, "SpielerRangListe")
        Dim dLast = DruckenTools.FindChild(Of DataGrid)(last, "SpielerRangListe")
        Dim width1 = d1.Columns(1).ActualWidth
        Dim width3 = dLast.Columns(2).ActualWidth
        Assert.AreEqual(219.73, width1, 0.01)
        Assert.AreEqual(168.22, width3, 0.01)
    End Sub


End Class