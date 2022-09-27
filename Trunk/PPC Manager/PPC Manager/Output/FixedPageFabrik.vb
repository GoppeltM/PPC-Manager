Imports PPC_Manager

Public Class FixedPageFabrik
    Implements IFixedPageFabrik
    Private ReadOnly _Spielerliste As IEnumerable(Of Spieler)
    Private ReadOnly _SpielRunden As SpielRunden
    Private ReadOnly _KlassementName As String
    Private ReadOnly _Spielstand As ISpielstand

    Public Sub New(spielerliste As IEnumerable(Of Spieler),
                   spielRunden As SpielRunden,
                   klassementName As String,
                   spielstand As ISpielstand)
        _Spielerliste = spielerliste
        _SpielRunden = spielRunden
        _KlassementName = klassementName
        _Spielstand = spielstand
    End Sub

    Function GetDataGridRows(grid As DataGrid) As IList(Of DataGridRow)
        Dim items = grid.ItemsSource
        Dim rows As IList(Of DataGridRow) = New List(Of DataGridRow)
        For Each row In items
            rows.Add(CType(grid.ItemContainerGenerator.ContainerFromItem(row), DataGridRow))
        Next
        Return rows
    End Function

    Friend Function ErzeugeRanglisteSeiten(seitenEinstellungen As SeitenEinstellung) As IEnumerable(Of FixedPage) Implements IFixedPageFabrik.ErzeugeRanglisteSeiten
        Dim AusgeschiedenInRunde0 = Function(s As SpielerInfo) As Boolean
                                        Return Aggregate x In _SpielRunden.Last.AusgeschiedeneSpielerIDs
                                               Where x = s.Id Into Any()
                                    End Function
        Dim AlleSpieler = (From x In _Spielerliste
                           Where Not AusgeschiedenInRunde0(x)
                           Select x).ToList
        AlleSpieler.Sort()
        AlleSpieler.Reverse()

        Dim seite = New RanglisteSeite(_KlassementName, AlleSpieler, _SpielRunden, _Spielstand, seitenEinstellungen)
        Dim pageContent As New PageContent()
        Dim fixedPage As New FixedPage()
        Dim sz As New Size(seitenEinstellungen.Breite, seitenEinstellungen.Höhe)

        fixedPage.Width = seitenEinstellungen.Breite
        fixedPage.Height = seitenEinstellungen.Höhe
        FixedPage.SetLeft(fixedPage, seitenEinstellungen.AbstandX)
        FixedPage.SetTop(fixedPage, seitenEinstellungen.AbstandY)
        fixedPage.Children.Add(seite)

        fixedPage.Measure(sz)
        fixedPage.Arrange(New Rect(New Point(), sz))
        fixedPage.UpdateLayout()

        'Diese Action erzwingt dass alle anstehen asynchronen Actions wie zB UpdateLayout ausgeführt werden, wodurch wir anschließend
        'korrekte Messungen in der FixedPage vornehmen können
        Threading.Dispatcher.CurrentDispatcher.Invoke(New Action(Function() As Boolean
                                                                     Return True
                                                                 End Function), Threading.DispatcherPriority.ContextIdle)

        Dim liste = CType(seite.FindName("SpielerRangListe"), DataGrid)
        Dim rows = GetDataGridRows(liste)
        Dim seiten = New List(Of FixedPage)

        Dim row = 0
        While (row < AlleSpieler.Count)
            Dim spielerAktuellSeite = New List(Of Spieler)
            Dim contentHeight = 80.0 'Höhe von Rand und Header + 30 Puffer
            While (row < rows.Count AndAlso rows.ElementAt(row).ActualHeight + contentHeight < liste.ActualHeight)
                contentHeight += rows.ElementAt(row).ActualHeight
                spielerAktuellSeite.Add(AlleSpieler.ElementAt(row))
                row += 1
            End While

            Dim page = New FixedPage
            page.Children.Add(New RanglisteSeite(_KlassementName, spielerAktuellSeite, _SpielRunden, _Spielstand, seitenEinstellungen))
            page.Height = seitenEinstellungen.Höhe
            page.Width = seitenEinstellungen.Breite
            FixedPage.SetLeft(page, seitenEinstellungen.AbstandX)
            FixedPage.SetTop(page, seitenEinstellungen.AbstandY)
            seiten.Add(page)
        End While

        Return seiten.AsEnumerable
    End Function

    Friend Function ErzeugeSchiedsrichterZettelSeiten(seitenEinstellung As SeitenEinstellung) As IEnumerable(Of FixedPage) Implements IFixedPageFabrik.ErzeugeSchiedsrichterZettelSeiten
        Dim format = New Size(seitenEinstellung.Breite, seitenEinstellung.Höhe)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of SpielPartie)) New SchiedsrichterZettel(el, _KlassementName, _SpielRunden.Count - 1, seitenNr)
        Dim leerControl = ErzeugeUserControl(1, 1, New List(Of SpielPartie))
        Dim leerSeite = SeiteErstellen(leerControl, format)
        Dim elemente = _SpielRunden.Peek.ToList
        Dim maxElemente = leerControl.GetMaxItemCount

        Dim pages = ElementePaketieren(maxElemente, format, elemente, ErzeugeUserControl)
        Return pages
    End Function

    Friend Function ErzeugePaarungen(seitenEinstellung As SeitenEinstellung) As IEnumerable(Of FixedPage) Implements IFixedPageFabrik.ErzeugePaarungen
        Dim format = New Size(seitenEinstellung.Breite, seitenEinstellung.Höhe)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of SpielPartie)) New NeuePaarungen(el,
                                                                                               _KlassementName,
                                                                                               _SpielRunden.Count - 1,
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

    Friend Function ErzeugeSpielErgebnisse(seitenEinstellung As SeitenEinstellung) As IEnumerable(Of FixedPage) Implements IFixedPageFabrik.ErzeugeSpielErgebnisse
        Dim format = New Size(seitenEinstellung.Breite, seitenEinstellung.Höhe)
        Dim ErzeugeUserControl = Function(seitenNr As Integer, eOffset As Integer,
                                          el As IEnumerable(Of SpielPartie)) New SpielErgebnisse(el,
                                                                                                 _KlassementName,
                                                                                                 _SpielRunden.Count - 1,
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
