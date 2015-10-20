Imports PPC_Manager

Public Class FixedPageFabrik
    Friend Function ErzeugeRanglisteSeiten(spielerListe As IEnumerable(Of Spieler), format As Size,
                                            altersGruppe As String, rundenNummer As Integer) As IEnumerable(Of FixedPage)

        Dim grids As New List(Of DataGrid)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer, el As IEnumerable(Of Spieler))
                                     Dim seite = New RanglisteSeite(altersGruppe, rundenNummer, seitenNr, eOffset, spielerListe)
                                     grids.Add(seite.SpielerRangListe)
                                     Return seite
                                 End Function

        Dim leerControl = ErzeugeUserControl(1, 1, New List(Of Spieler))
        Dim leerSeite = SeiteErstellen(leerControl, format)
        Dim elemente = spielerListe.ToList
        If Not elemente.Any Then
            Return New List(Of FixedPage)
        End If
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)

        SpaltenAngleichen(grids)

        Return pages
    End Function

    Private Shared Sub SpaltenAngleichen(seiten As IEnumerable(Of DataGrid))
        Dim ColumnsOberesLimit As New Dictionary(Of Integer, Double)
        For Each seite In seiten
            For Each column In seite.Columns
                If Not ColumnsOberesLimit.ContainsKey(column.DisplayIndex) Then
                    ColumnsOberesLimit.Add(column.DisplayIndex, column.ActualWidth)
                End If
                If column.ActualWidth > ColumnsOberesLimit.Item(column.DisplayIndex) Then
                    ColumnsOberesLimit.Item(column.DisplayIndex) = column.ActualWidth
                End If
            Next
        Next

        For Each seite In seiten
            For Each column In ColumnsOberesLimit
                seite.Columns(column.Key).MinWidth = column.Value
            Next
            seite.UpdateLayout()
        Next
    End Sub

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
            Return New List(Of FixedPage)
        End If
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)
        Return pages
    End Function

    Friend Function ErzeugeNeuePaarungen(partien As IEnumerable(Of SpielPartie), format As Size,
                                         klassementName As String, rundenNummer As Integer) As IEnumerable(Of FixedPage)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of SpielPartie)) New NeuePaarungen(el, klassementName, rundenNummer, seitenNr)
        Dim leerControl = ErzeugeUserControl(1, 1, New List(Of SpielPartie))
        Dim leerSeite = SeiteErstellen(leerControl, format)
        Dim elemente = partien.ToList
        If Not elemente.Any Then
            Return New List(Of FixedPage)
        End If
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)
        Return pages
    End Function

    Friend Function ErzeugeSpielErgebnisse(partien As IEnumerable(Of SpielPartie), format As Size, klassementName As String, rundenNummer As Integer) As IEnumerable(Of FixedPage)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of SpielPartie)) New SpielErgebnisse(el, klassementName, rundenNummer, seitenNr)
        Dim leerControl = ErzeugeUserControl(1, 1, New List(Of SpielPartie))
        Dim leerSeite = SeiteErstellen(leerControl, format)
        Dim elemente = partien.ToList
        If Not elemente.Any Then
            Return New List(Of FixedPage)
        End If
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)
        Return pages
    End Function
End Class
