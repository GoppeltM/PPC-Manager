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

    Sub SeitenEinstellungenAnwenden(page As FixedPage, einstellungen As SeitenEinstellung)
        With einstellungen
            Dim sz As New Size(.Breite, .Höhe)
            page.Width = .Breite
            page.Height = .Höhe
            FixedPage.SetLeft(page, .AbstandX)
            FixedPage.SetTop(page, .AbstandY)
            page.Measure(sz)
            page.Arrange(New Rect(New Point(), sz))
            page.UpdateLayout()
        End With
    End Sub

    Friend Function ErzeugeRanglisteSeiten(seitenEinstellungen As SeitenEinstellung) As IEnumerable(Of FixedPage) Implements IFixedPageFabrik.ErzeugeRanglisteSeiten
        Dim AusgeschiedenInRunde0 = Function(s As SpielerInfo) As Boolean
                                        Return Aggregate x In _SpielRunden.Last.AusgeschiedeneSpielerIDs
                                               Where x = s.Id Into Any()
                                    End Function
        Dim ÜbrigeSpieler = (From x In _Spielerliste
                             Where Not AusgeschiedenInRunde0(x)
                             Select x).ToList
        ÜbrigeSpieler.Sort()
        ÜbrigeSpieler.Reverse()

        Dim seiten = New List(Of FixedPage)
        Dim RangOffset = 1

        While (ÜbrigeSpieler.Count > 0)
            Dim row = 0

            Dim seite = New RanglisteSeite(_KlassementName, ÜbrigeSpieler, _SpielRunden, _Spielstand, seitenEinstellungen, RangOffset)
            Dim pageContent As New PageContent()
            Dim fixedPage As New FixedPage()
            'Dim sz As New Size(seitenEinstellungen.Breite, seitenEinstellungen.Höhe)

            fixedPage.Children.Add(seite)
            SeitenEinstellungenAnwenden(fixedPage, seitenEinstellungen)

            'Diese Action erzwingt dass alle anstehen asynchronen Actions wie zB UpdateLayout ausgeführt werden, wodurch wir anschließend
            'korrekte Messungen in der FixedPage vornehmen können
            Threading.Dispatcher.CurrentDispatcher.Invoke(New Action(Function() As Boolean
                                                                         Return True
                                                                     End Function), Threading.DispatcherPriority.ContextIdle)

            Dim liste = CType(seite.FindName("SpielerRangListe"), DataGrid)
            Dim rows = GetDataGridRows(liste)
            Dim spielerAktuellSeite = New List(Of Spieler)
            Dim contentHeight = 100.0 'Höhe von Rand und Header und Puffer
            While (row < rows.Count AndAlso rows.ElementAt(row).ActualHeight + contentHeight < seitenEinstellungen.Höhe)
                contentHeight += rows.ElementAt(row).ActualHeight
                spielerAktuellSeite.Add(ÜbrigeSpieler.ElementAt(row))
                row += 1
            End While
            ÜbrigeSpieler.RemoveRange(0, row)

            Dim page = New FixedPage
            page.Children.Add(New RanglisteSeite(_KlassementName, spielerAktuellSeite, _SpielRunden, _Spielstand, seitenEinstellungen, RangOffset))
            SeitenEinstellungenAnwenden(page, seitenEinstellungen)
            seiten.Add(page)
            RangOffset += row
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
