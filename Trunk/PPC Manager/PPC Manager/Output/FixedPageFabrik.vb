Public Class FixedPageFabrik
    Friend Function ErzeugeRanglisteSeiten(spielerListe As IEnumerable(Of Spieler), format As Size,
                                            altersGruppe As String, rundenNummer As Integer) As IEnumerable(Of FixedPage)
        Dim seite = New RanglisteSeite(altersGruppe, rundenNummer, spielerListe)
        With seite
            Dim canvas = New Canvas
            canvas.Children.Add(seite)
            canvas.Measure(New Size(format.Width, Double.MaxValue))
            canvas.UpdateLayout()
        End With

        Dim gesamtGröße = seite.DesiredSize
        seite = New RanglisteSeite(altersGruppe, rundenNummer, spielerListe)
        Return ErzeugeSeiten(seite, gesamtGröße, format)
    End Function

    Friend Function ErzeugeSeiten(v As Visual, gesamtGröße As Size, seitengröße As Size) As IEnumerable(Of FixedPage)
        Dim brush As New VisualBrush(v) With {.Stretch = Stretch.None, .AlignmentX = AlignmentX.Left, .AlignmentY = AlignmentY.Top}

        Dim aktuelleHöhe = 0.0
        Dim seiten As New List(Of FixedPage)

        While aktuelleHöhe < gesamtGröße.Height
            brush.Transform = New TranslateTransform(0, aktuelleHöhe * -1)
            Dim page = New FixedPage With {.Height = seitengröße.Height, .Width = seitengröße.Width}
            Dim canvas As New Canvas With {.Height = gesamtGröße.Height, .Width = gesamtGröße.Width, .Background = brush.Clone}
            page.Children.Add(canvas)
            seiten.Add(page)
            aktuelleHöhe += seitengröße.Height
            ' Wir erzeugen einen zusätzlichen überlappenden Rand, um halb zerschnittene Zeilen zu kompensieren
            aktuelleHöhe -= 20
        End While

        Return seiten
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
