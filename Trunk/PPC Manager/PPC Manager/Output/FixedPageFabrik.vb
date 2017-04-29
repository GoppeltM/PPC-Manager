Imports PPC_Manager

Public Class FixedPageFabrik
    Implements IFixedPageFabrik
    Private ReadOnly _Spielerliste As IEnumerable(Of SpielerInfo)
    Private ReadOnly _SpielRunden As SpielRunden
    Private ReadOnly _KlassementName As String
    Private ReadOnly _Spielverlauf As ISpielverlauf(Of SpielerInfo)
    Private ReadOnly _Spielstand As ISpielstand

    Public Sub New(spielerliste As IEnumerable(Of SpielerInfo),
                   spielRunden As SpielRunden,
                   spielverlauf As ISpielverlauf(Of SpielerInfo),
                   klassementName As String,
                   spielstand As ISpielstand)
        _Spielerliste = spielerliste
        _SpielRunden = spielRunden
        _Spielverlauf = spielverlauf
        _KlassementName = klassementName
        _Spielstand = spielstand
    End Sub

    Friend Function ErzeugeRanglisteSeiten(seitenEinstellungen As ISeiteneinstellung) As IEnumerable(Of FixedPage) Implements IFixedPageFabrik.ErzeugeRanglisteSeiten
        Dim AusgeschiedenInRunde0 = Function(s As SpielerInfo) As Boolean
                                        Return Aggregate x In _SpielRunden.First.AusgeschiedeneSpielerIDs
                                               Where x = s.Id Into Any()
                                    End Function
        Dim l = (From x In _Spielerliste
                 Where Not AusgeschiedenInRunde0(x)
                 Select New Spieler(x, _Spielverlauf)).ToList
        l.Sort()
        l.Reverse()
        Dim spielPartien = New List(Of SpielPartie)
        If _SpielRunden.Any Then
            spielPartien = _SpielRunden.Peek.Where(Function(m) Not TypeOf m Is FreiLosSpiel).ToList
        End If
        Dim seite = New RanglisteSeite(_KlassementName, _SpielRunden.Count, l, spielPartien)
        With seite
            Dim canvas = New Canvas
            canvas.Children.Add(seite)
            canvas.Measure(New Size(seitenEinstellungen.Breite, Double.MaxValue))
            canvas.Arrange(New Rect(0, 0, seitenEinstellungen.Breite, Double.MaxValue))
        End With

        Dim gesamtLänge = seite.RenderSize.Height

        seite = New RanglisteSeite(_KlassementName,
                                   _SpielRunden.Count, l, spielPartien)
        Return ErzeugeSeiten(seite, gesamtLänge, seitenEinstellungen)
    End Function

    Friend Function ErzeugeSchiedsrichterZettelSeiten(seitenEinstellung As ISeiteneinstellung) As IEnumerable(Of FixedPage) Implements IFixedPageFabrik.ErzeugeSchiedsrichterZettelSeiten
        Dim format = New Size(seitenEinstellung.Breite, seitenEinstellung.Höhe)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of SpielPartie)) New SchiedsrichterZettel(el, _KlassementName, _SpielRunden.Count, seitenNr)
        Dim leerControl = ErzeugeUserControl(1, 1, New List(Of SpielPartie))
        Dim leerSeite = SeiteErstellen(leerControl, format)
        Dim elemente = _SpielRunden.Peek.ToList
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)
        Return pages
    End Function

    Friend Function ErzeugePaarungen(seitenEinstellung As ISeiteneinstellung) As IEnumerable(Of FixedPage) Implements IFixedPageFabrik.ErzeugePaarungen
        Dim format = New Size(seitenEinstellung.Breite, seitenEinstellung.Höhe)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of SpielPartie)) New NeuePaarungen(el,
                                                                                               _KlassementName,
                                                                                               _SpielRunden.Count,
                                                                                               seitenNr,
                                                                                               _Spielstand)
        Dim leerControl = ErzeugeUserControl(1, 1, New List(Of SpielPartie))
        Dim leerSeite = SeiteErstellen(leerControl, format)
        Dim elemente = _SpielRunden.Peek.ToList
        If Not elemente.Any Then
            Return New List(Of FixedPage)
        End If
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)
        Return pages
    End Function

    Friend Function ErzeugeSpielErgebnisse(seitenEinstellung As ISeiteneinstellung) As IEnumerable(Of FixedPage) Implements IFixedPageFabrik.ErzeugeSpielErgebnisse
        Dim format = New Size(seitenEinstellung.Breite, seitenEinstellung.Höhe)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of SpielPartie)) New SpielErgebnisse(el,
                                                                                                 _KlassementName,
                                                                                                 _SpielRunden.Count,
                                                                                                 seitenNr,
                                                                                                 _Spielstand)
        Dim leerControl = ErzeugeUserControl(1, 1, New List(Of SpielPartie))
        Dim leerSeite = SeiteErstellen(leerControl, format)

        If Not _SpielRunden.Any Then
            Return New List(Of FixedPage)
        End If
        Dim elemente = _SpielRunden.Peek.ToList
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)
        Return pages
    End Function

    Private Function ErzeugeSeiten(v As Visual, gesamtLänge As Double,
                                   seitengröße As ISeiteneinstellung) As IEnumerable(Of FixedPage)
        Dim brush As New VisualBrush(v) With {.Stretch = Stretch.None, .AlignmentX = AlignmentX.Left, .AlignmentY = AlignmentY.Top}

        Dim aktuelleHöhe = 0.0
        Dim seiten As New List(Of FixedPage)

        While aktuelleHöhe < gesamtLänge
            brush.Transform = New TranslateTransform(0, aktuelleHöhe * -1)
            Dim page = New FixedPage With {
                .Height = seitengröße.Höhe + (seitengröße.AbstandY * 2),
                .Width = seitengröße.Breite + (seitengröße.AbstandX * 2),
                .Margin = New Thickness(seitengröße.AbstandX, seitengröße.AbstandY, seitengröße.AbstandX, seitengröße.AbstandY)}
            Dim canvas As New Canvas With {
                .Height = gesamtLänge,
                .Width = seitengröße.Breite,
                .Background = brush.Clone}
            page.Children.Add(canvas)
            seiten.Add(page)
            aktuelleHöhe += seitengröße.Höhe
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


End Class
