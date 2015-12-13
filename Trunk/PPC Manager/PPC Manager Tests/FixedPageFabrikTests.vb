Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports Moq

<TestFixture>
Public Class FixedPageFabrikTests

    <Test, STAThread>
    Public Sub ErzeugeRanglisteSeiten_leer_enthält_eine_Seite()
        Dim spielerListe As New List(Of Spieler)
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeRanglisteSeiten(spielerListe, New Size(300, 500), "Altersgruppe", 24)
        Assert.That(seiten.Count, [Is].EqualTo(1))
    End Sub

    <Test, STAThread>
    Public Sub ErzeugeRanglisteSeiten_mit_200_spielern_ergibt_mehrere_Seiten()
        Dim spielerListe As New List(Of Spieler)
        For Each nummer In Enumerable.Range(1, 200)
            Dim s As New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "Spieler" & nummer}
            spielerListe.Add(s)
        Next
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeRanglisteSeiten(spielerListe, New Size(300, 500), "Altersgruppe", 24)
        Assert.That(seiten.Count, [Is].GreaterThan(1))
    End Sub

    <Test, STAThread, Explicit>
    Public Sub UIDummy_ErzeugeRanglisteSeiten_mit_200_Spielern()
        Dim spielerListe As New List(Of Spieler)
        For Each nummer In Enumerable.Range(1, 200)
            Dim s As New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "Spieler" & nummer}
            spielerListe.Add(s)
        Next
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeRanglisteSeiten(spielerListe, New Size(1000, 600), "Altersgruppe", 24)
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

    <Test, STAThread, Explicit>
    Public Sub UIDummy_Reales_Layout()
        Dim doc = XDocument.Load("D:\Turnierteilnehmer_PPC_20150913.xml")
        Dim spieler = doc.Root.<competition>.First.<players>
        Dim l = SpielerListe.FromXML(spieler, New SpielRunden, New SpielRegeln(3, True, True))

        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeRanglisteSeiten(l, New Size(600, 1000), "Altersgruppe", 24)
        Dim vorschau As New Druckvorschau(seiten)
        vorschau.ShowDialog()
    End Sub

    <Test, STAThread>
    Public Sub ErzeugeSchiedsrichterzettel_leer_enthält_keine_Seite()
        Dim f As New FixedPageFabrik
        Dim spielPartien = New List(Of SpielPartie)
        Dim seiten = f.ErzeugeSchiedsrichterZettelSeiten(spielPartien, New Size(300, 500), "AltersGruppe", 3)
        Assert.That(seiten.Count, [Is].EqualTo(0))
    End Sub

    <Test, STAThread>
    Public Sub ErzeugeSchiedsrichterzettel_mit_200_Partien_600_mal_1000_enthält_34_Seiten()
        Dim f As New FixedPageFabrik
        Dim spielPartien = New List(Of SpielPartie)
        For Each nummer In Enumerable.Range(1, 200)
            Dim s As New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "Spieler" & nummer}
            spielPartien.Add(New FreiLosSpiel("Runde XY", s, 3))
        Next

        Dim seiten = f.ErzeugeSchiedsrichterZettelSeiten(spielPartien, New Size(600, 1000), "AltersGruppe", 3)
        Assert.That(seiten.Count, [Is].EqualTo(34))
    End Sub

    <Test, STAThread>
    Public Sub ErzeugeSpielergebnisse_keine_Partien_ergibt_0_Seiten()
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeSpielErgebnisse(New List(Of SpielPartie), New Size(300, 500), "AltersGruppe", 3)
        Assert.That(seiten.Count, [Is].EqualTo(0))
    End Sub

    <Test, STAThread>
    Public Sub ErzeugeNeuePaarungen_keine_Partien_ergibt_0_Seiten()
        Dim f As New FixedPageFabrik
        Dim seiten = f.ErzeugeNeuePaarungen(New List(Of SpielPartie), New Size(300, 500), "AltersGruppe", 3)
        Assert.That(seiten.Count, [Is].EqualTo(0))
    End Sub


End Class
