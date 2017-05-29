Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports Moq

<TestFixture, Apartment(System.Threading.ApartmentState.STA)>
Public Class FixedPageFabrikTests

    <SetUp>
    Public Sub init()
        _SeitenEinstellungen = New SeitenEinstellung
        With _SeitenEinstellungen
            .AbstandX = 30
            .AbstandY = 30
            .Breite = 600
            .Höhe = 1000
        End With

        _SpielerListe = New List(Of SpielerInfo)
        Dim spielerWrapped = From x In _SpielerListe
                             Select New Spieler(x,
                                 Mock.Of(Of ISpielverlauf(Of SpielerInfo)),
                                 Mock.Of(Of IComparer(Of SpielerInfo)))
        _Runden = New SpielRunden
        _Runden.Push(New SpielRunde)
        f = New FixedPageFabrik(spielerWrapped,
                                _Runden,
                                "Altersgruppe", New Spielstand(3))
    End Sub

    Private _SeitenEinstellungen As SeitenEinstellung
    Private f As FixedPageFabrik
    Private _SpielerListe As IList(Of SpielerInfo)
    Private _Runden As SpielRunden

    <Test>
    Public Sub ErzeugeRanglisteSeiten_leer_enthält_eine_Seite()
        Dim seiten = f.ErzeugeRanglisteSeiten(_SeitenEinstellungen)
        Assert.That(seiten.Count, [Is].EqualTo(1))
    End Sub

    <Test>
    Public Sub ErzeugeRanglisteSeiten_mit_200_spielern_ergibt_mehrere_Seiten()

        For Each nummer In Enumerable.Range(1, 200)
            Dim s As New SpielerInfo("Spieler" & nummer)
            _SpielerListe.Add(s)
        Next
        Dim seiten = f.ErzeugeRanglisteSeiten(_SeitenEinstellungen)
        Assert.That(seiten.Count, [Is].GreaterThan(1))
    End Sub

    <Test, Explicit>
    Public Sub UIDummy_ErzeugeRanglisteSeiten_mit_200_Spielern()
        For Each nummer In Enumerable.Range(1, 200)
            Dim s As New SpielerInfo("Spieler" & nummer)
            _SpielerListe.Add(s)
        Next
        Dim seiten = f.ErzeugeRanglisteSeiten(_SeitenEinstellungen)
        Dim doc = New FixedDocument
        For Each seite In seiten
            doc.Pages.Add(New PageContent() With {.Child = seite})
        Next
        Dim w As New Window
        Dim viewer = New DocumentViewer With {.Document = doc}
        viewer.UpdateLayout()
        w.Content = viewer
        w.ShowDialog()
    End Sub

    <Test, Explicit>
    Public Sub UIDummy_Reales_Layout()
        Dim doc = XDocument.Load("D:\Turnierteilnehmer_PPC_20150913.xml")
        Dim spieler = doc.Root.<competition>.First.<players>
        Dim l = From x In AusXML.SpielerListeFromXML(spieler) Select New SpielerInfo(x.Id)

        Dim seiten = f.ErzeugeRanglisteSeiten(_SeitenEinstellungen)
        Dim vorschau As New Druckvorschau(seiten)
        vorschau.ShowDialog()
    End Sub

    <Test>
    Public Sub ErzeugeSchiedsrichterzettel_leer_enthält_keine_Seite()
        Dim spielPartien = New List(Of SpielPartie)
        Dim s As New SeitenEinstellung With {.Höhe = 300, .Breite = 500}
        Dim seiten = f.ErzeugeSchiedsrichterZettelSeiten(s)
        Assert.That(seiten.Count, [Is].EqualTo(0))
    End Sub

    <Test>
    Public Sub ErzeugeSchiedsrichterzettel_mit_200_Partien_600_mal_1000_enthält_34_Seiten()
        Dim r = New SpielRunde
        For Each nummer In Enumerable.Range(1, 200)
            Dim s As New SpielerInfo("Spieler" & nummer)
            r.Add(New FreiLosSpiel("Runde XY", s))
        Next
        _Runden.Push(r)
        Dim e As New SeitenEinstellung With {.Höhe = 1000, .Breite = 600}
        Dim seiten = f.ErzeugeSchiedsrichterZettelSeiten(e)
        Assert.That(seiten.Count, [Is].EqualTo(34))
    End Sub

    <Test, Explicit>
    Public Sub Druckdialog_Einstellungen_sind_übertragbar()
        Dim l = New List(Of FixedPage)
        Dim p = New PrintDialog
        p.ShowDialog()
        Dim einstellungen = p.PrintTicket
        Dim pageA = New FixedPage
        pageA.Height = p.PrintableAreaHeight
        pageA.Width = p.PrintableAreaWidth
        pageA.Children.Add(New Rectangle With {.Height = 100, .Width = 50, .Stroke = Brushes.Red})
        Dim pageB = New FixedPage
        pageB.Children.Add(New Rectangle With {.Height = 200, .Width = 250, .Stroke = Brushes.Blue})
        pageB.Height = p.PrintableAreaHeight
        pageB.Width = p.PrintableAreaWidth
        l.Add(pageA)
        l.Add(pageB)
        Dim doc = New FixedDocument
        doc.Pages.Add(New PageContent With {.Child = pageA})
        doc.Pages.Add(New PageContent With {.Child = pageB})

        Dim neuerDrucker = New PrintDialog
        neuerDrucker.PrintTicket = einstellungen
        neuerDrucker.PrintDocument(doc.DocumentPaginator, "Hello World")
    End Sub

    <Test>
    Public Sub ErzeugeSpielergebnisse_keine_Partien_ergibt_0_Seiten()
        Dim e As New SeitenEinstellung With {.Höhe = 500, .Breite = 300}
        Dim seiten = f.ErzeugeSpielErgebnisse(e)
        Assert.That(seiten.Count, [Is].EqualTo(0))
    End Sub


End Class
