Imports PPC_Manager

Public Class FixedPageFabrik
    Friend Function ErzeugeRanglisteSeiten(spielerListe As List(Of Spieler), format As Size,
                                            altersGruppe As String, rundenNummer As Integer) As IEnumerable(Of FixedPage)
        Dim maxElemente = SeiteErstellen(New List(Of Spieler), format, altersGruppe, rundenNummer, 1, 1).Item2.GetMaxItemCount

        Dim elemente = spielerListe.ToList

        Dim pages = New List(Of FixedPage)
        Dim content = New List(Of IPaginatibleUserControl)
        Dim seitenNummer = 0
        Dim ElementOffset = 0
        While elemente.Any
            Dim currentElements = elemente.Take(maxElemente).ToList
            Dim Erstellen = SeiteErstellen(currentElements, format, altersGruppe, rundenNummer, seitenNummer, ElementOffset)
            pages.Add(Erstellen.Item1)
            content.Add(Erstellen.Item2)
            elemente = elemente.Skip(maxElemente).ToList
            seitenNummer += 1
            ElementOffset += currentElements.Count
        End While

        ' DruckenTools.SpaltenAngleichen(content, "SpielerRangListe")

        Return pages
    End Function

    Private Shared Function SeiteErstellen(spielerListe As List(Of Spieler), format As Size,
                                           altersGruppe As String, rundenNummer As Integer,
                                           seitenNummer As Integer, offset As Integer) As Tuple(Of FixedPage, IPaginatibleUserControl)
        Dim UserControl = New RanglisteSeite(altersGruppe, rundenNummer, seitenNummer, offset, spielerListe)
        Dim Page As New FixedPage
        Page.Width = format.Width
        Page.Height = format.Height
        UserControl.Width = Page.Width
        UserControl.Height = Page.Height
        Page.Children.Add(UserControl)
        Page.Measure(format)
        Page.Arrange(New Rect(New Point(0, 0), format))
        Page.UpdateLayout()
        Return Tuple.Create(Of FixedPage, IPaginatibleUserControl)(Page, UserControl)
    End Function
End Class
