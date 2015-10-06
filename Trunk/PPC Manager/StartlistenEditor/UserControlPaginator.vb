
Public Class UserControlPaginator

    Private ReadOnly _Spieler As IEnumerable(Of Spieler)
    Private ReadOnly _pageSize As Size

    Public Sub New(spieler As IEnumerable(Of Spieler), ByVal pageSize As Size)
        _Spieler = spieler
        _pageSize = pageSize
    End Sub

    Public Function ErzeugePageContent() As IEnumerable(Of PageContent)
        Dim elemente = _Spieler
        If Not elemente.Any Then
            Return New List(Of PageContent)
        End If
        Dim leerControl = CreateVisual(New List(Of Spieler))

        Dim maxElemente = leerControl.Item2.GetMaxItemCount
        Dim pages = New List(Of PageContent)
        Dim seitenNummer = 0
        Dim ElementOffset = 0

        While elemente.Any
            Dim currentElements = elemente.Take(maxElemente).ToList
            Dim UserControl = CreateVisual(currentElements)
            pages.Add(New PageContent() With {.Child = UserControl.Item1})
            elemente = elemente.Skip(maxElemente).ToList
            seitenNummer += 1
            ElementOffset += currentElements.Count
        End While
        Return pages
    End Function

    Private Function CreateVisual(ByVal spielerListe As IEnumerable(Of Spieler)) As Tuple(Of FixedPage, StartlisteSeite)
        Dim visual = New StartlisteSeite(spielerListe)
        Dim page As New FixedPage
        page.Width = _pageSize.Width
        page.Height = _pageSize.Height
        visual.Width = page.Width
        visual.Height = page.Height
        page.Children.Add(visual)
        page.Measure(_pageSize)
        page.Arrange(New Rect(New Point(0, 0), _pageSize))
        page.UpdateLayout()
        Return Tuple.Create(page, visual)
    End Function

End Class
