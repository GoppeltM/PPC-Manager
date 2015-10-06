Public Class FixedPageFabrik
    Friend Function ErzeugeRanglisteSeiten(spielerListe As List(Of Spieler), format As Size,
                                            altersGruppe As String, rundenNummer As Integer) As IEnumerable(Of FixedPage)

        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of Spieler)) _
                                          New RanglisteSeite(altersGruppe, rundenNummer, seitenNr, eOffset, el)
        Dim leerControl = ErzeugeUserControl(1, 1, New List(Of Spieler))
        Dim leerSeite = SeiteErstellen(leerControl, format)
        Dim elemente = spielerListe.ToList
        If Not elemente.Any Then
            Return New FixedPage() {leerSeite}
        End If
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)
        ' DruckenTools.SpaltenAngleichen(content, "SpielerRangListe")

        Return pages
                                 End Function

    Private Shared Function ElementePaketieren(Of T)(maxElemente As Integer, format As Size,
                                                     elemente As IEnumerable(Of T),
                                                     erzeugeUserControl As Func(Of Integer, Integer, IEnumerable(Of T), UserControl)) _
                                                     As IEnumerable(Of FixedPage)
        Dim pages = New List(Of FixedPage)
        Dim seitenNummer = 0
        Dim ElementOffset = 0

        While elemente.Any
            Dim currentElements = elemente.Take(maxElemente).ToList
            Dim UserControl = erzeugeUserControl(seitenNummer, ElementOffset, currentElements)
            Dim Erstellen = SeiteErstellen(UserControl, format)
            pages.Add(Erstellen)
            elemente = elemente.Skip(maxElemente).ToList
            seitenNummer += 1
            ElementOffset += currentElements.Count
        End While
        Return pages
    End Function


    Private Shared Function SeiteErstellen(Of T As UserControl)(control As T, format As Size) As FixedPage
        Dim Page As New FixedPage
        Page.Width = format.Width
        Page.Height = format.Height
        control.Width = Page.Width
        control.Height = Page.Height
        Page.Children.Add(control)
        Page.Measure(format)
        Page.Arrange(New Rect(New Point(0, 0), format))
        Page.UpdateLayout()
        Return Page
    End Function

    Friend Function ErzeugeSchiedsrichterZettelSeiten(partien As IEnumerable(Of SpielPartie), format As Size,
                                                      altersGruppe As String, rundenNummer As Integer) As IEnumerable(Of FixedPage)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of SpielPartie)) New SchiedsrichterZettel(el, altersGruppe, rundenNummer, seitenNr)
        Dim leerControl = ErzeugeUserControl(1, 1, New List(Of SpielPartie))
        Dim leerSeite = SeiteErstellen(leerControl, format)
        Dim elemente = partien.ToList
        If Not elemente.Any Then
            Return New FixedPage() {leerSeite}
        End If
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)
        Return pages
    End Function
End Class
