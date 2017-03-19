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
        Dim seitenEinstellungen = New Mock(Of ISeiteneinstellung)
        seitenEinstellungen.SetupAllProperties()
        With seitenEinstellungen.Object
            .AbstandX = 30
            .AbstandY = 30
            .Breite = 600
            .Höhe = 1000
        End With
        _SeitenEinstellungen = seitenEinstellungen.Object
    End Sub

    Private _SeitenEinstellungen As ISeiteneinstellung

    <Test>
    Public Sub ErzeugeRanglisteSeiten_leer_enthält_eine_Seite()
        Dim spielerListe As New List(Of Spieler)
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeRanglisteSeiten(spielerListe, _SeitenEinstellungen, "Altersgruppe", 24, New List(Of SpielPartie))
        Assert.That(seiten.Count, [Is].EqualTo(1))
    End Sub

    <Test>
    Public Sub ErzeugeRanglisteSeiten_mit_200_spielern_ergibt_mehrere_Seiten()
        Dim spielerListe As New List(Of Spieler)
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        For Each nummer In Enumerable.Range(1, 200)
            Dim s As New Spieler(spielverlauf) With {.Id = "Spieler" & nummer}
            spielerListe.Add(s)
        Next
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeRanglisteSeiten(spielerListe, _SeitenEinstellungen, "Altersgruppe", 24, New List(Of SpielPartie))
        Assert.That(seiten.Count, [Is].GreaterThan(1))
    End Sub

    <Test, Explicit>
    Public Sub UIDummy_ErzeugeRanglisteSeiten_mit_200_Spielern()
        Dim spielerListe As New List(Of Spieler)
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        For Each nummer In Enumerable.Range(1, 200)
            Dim s As New Spieler(spielverlauf) With {.Id = "Spieler" & nummer}
            spielerListe.Add(s)
        Next
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeRanglisteSeiten(spielerListe, _SeitenEinstellungen, "Altersgruppe", 24, New List(Of SpielPartie))
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
    Public Sub Ellipsen_Druck_Test()

        Dim document As New FixedDocument
        Dim ellipse = New Ellipse With {.Stroke = Brushes.Black, .Fill = Brushes.Red, .Height = 1300, .Width = 300}

        Dim höhe = ellipse.Height

        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeSeiten(ellipse, 1300, _SeitenEinstellungen)
        For Each seite In seiten
            document.Pages.Add(New PageContent() With {.Child = seite})
        Next
        Dim docViewer = New DocumentViewer()
        docViewer.Document = document
        Dim w As New Window With {.Content = docViewer}
        w.ShowDialog()

    End Sub

    <Test, Explicit>
    Public Sub UIDummy_Reales_Layout()
        Dim doc = XDocument.Load("D:\Turnierteilnehmer_PPC_20150913.xml")
        Dim spieler = doc.Root.<competition>.First.<players>
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim l = From x In AusXML.SpielerListeFromXML(spieler) Select New Spieler(spielverlauf)

        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeRanglisteSeiten(l, _SeitenEinstellungen, "Altersgruppe", 24, New List(Of SpielPartie))
        Dim vorschau As New Druckvorschau(seiten)
        vorschau.ShowDialog()
    End Sub

    <Test>
    Public Sub ErzeugeSchiedsrichterzettel_leer_enthält_keine_Seite()
        Dim f As New FixedPageFabrik
        Dim spielPartien = New List(Of SpielPartie)
        Dim seiten = f.ErzeugeSchiedsrichterZettelSeiten(spielPartien, New Size(300, 500), "AltersGruppe", 3)
        Assert.That(seiten.Count, [Is].EqualTo(0))
    End Sub

    <Test>
    Public Sub ErzeugeSchiedsrichterzettel_mit_200_Partien_600_mal_1000_enthält_34_Seiten()
        Dim f As New FixedPageFabrik
        Dim spielPartien = New List(Of SpielPartie)
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        For Each nummer In Enumerable.Range(1, 200)
            Dim s As New Spieler(spielverlauf) With {.Id = "Spieler" & nummer}
            spielPartien.Add(New FreiLosSpiel("Runde XY", s, 3))
        Next

        Dim seiten = f.ErzeugeSchiedsrichterZettelSeiten(spielPartien, New Size(600, 1000), "AltersGruppe", 3)
        Assert.That(seiten.Count, [Is].EqualTo(34))
    End Sub

    <Test>
    Public Sub ErzeugeSpielergebnisse_keine_Partien_ergibt_0_Seiten()
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeSpielErgebnisse(New List(Of SpielPartie), New Size(300, 500), "AltersGruppe", 3)
        Assert.That(seiten.Count, [Is].EqualTo(0))
    End Sub

    <Test>
    Public Sub ErzeugeNeuePaarungen_keine_Partien_ergibt_0_Seiten()
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeNeuePaarungen(New List(Of SpielPartie), New Size(300, 500), "AltersGruppe", 3)
        Assert.That(seiten.Count, [Is].EqualTo(0))
    End Sub


End Class
