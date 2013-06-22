Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class Paket_Tests

    <TestMethod>
    Sub Spieler_Sortieren()
        MainWindow.AktiveCompetition = New Competition
        Dim XNode = <players>
                        <player type="single" id="PLAYER114">
                            <person licence-nr="49790" club-federation-nickname="BaTTV" club-name="VfR Rheinsheim" sex="1"
                                ttr-match-count="181" lastname="Westermann" ttr="1284" internal-nr="NU439379" club-nr="128"
                                firstname="Gerd" birthyear="1973"/>
                        </player>
                        <player type="single" id="PLAYER115">
                            <person licence-nr="143323" club-federation-nickname="BaTTV" club-name="TTF 1955 Ispringen e.V."
                                sex="1" ttr-match-count="46" lastname="Wild" ttr="1009" internal-nr="NU1072486" club-nr="713"
                                firstname="Christian" birthyear="1993"/>
                        </player>
                        <player type="single" id="PLAYER116">
                            <person licence-nr="108866" club-federation-nickname="BaTTV" club-name="TV 1897 Forst eV"
                                sex="1" ttr-match-count="178" lastname="Wolfert" ttr="1310" internal-nr="NU489193"
                                club-nr="108" firstname="Till" birthyear="1996"/>
                        </player>
                        <ppc:player xmlns:ppc="http://www.ttc-langensteinbach.de/" id="PLAYER-1">
                            <person club-name="TTC Langensteinbach" sex="1" ttr-match-count="0" firstname="Marius"
                                lastname="Goppelt" ttr="1102" licence-nr="-1"/>
                        </ppc:player>
                    </players>

        Dim Spielerliste = (From x In XNode.Elements Select Spieler.FromXML(x)).ToList
        Spielerliste.Sort()
        CollectionAssert.AreEqual(New String() {"Wolfert", "Westermann", "Goppelt", "Wild"}, (From x In Spielerliste Select x.Nachname).ToList)

    End Sub

    <TestMethod>
    Sub Gerade_StandardPaarung()
        MainWindow.AktiveCompetition = New Competition
        Dim p As New Paket(1, "Runde 1")
        p.SpielerListe = New List(Of Spieler) From
                {New Spieler With {.Nachname = "Alpha", .TTRating = 50},
                New Spieler With {.Nachname = "Beta", .TTRating = 40},
                New Spieler With {.Nachname = "Gamma", .TTRating = 30},
                New Spieler With {.Nachname = "Delta", .TTRating = 20}}

        Dim suche = New PaarungsSuche("Runde 1").StandardPaarung(p.SpielerListe, 2, p)
        Assert.IsNotNull(suche)
        Assert.IsNull(suche.aktuellerSchwimmer)
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.AreEqual("Runde 1", suche.Partien.First.RundenName)
    End Sub

    <TestMethod>
    Sub Ungerade_StandardPaarung()
        MainWindow.AktiveCompetition = New Competition
        Dim p As New Paket(1, "Runde xyz")
        p.SpielerListe = New List(Of Spieler) From
                {New Spieler With {.Nachname = "Alpha", .TTRating = 50},
                New Spieler With {.Nachname = "Beta", .TTRating = 40},
                New Spieler With {.Nachname = "Gamma", .TTRating = 30},
                New Spieler With {.Nachname = "Delta", .TTRating = 20},
                New Spieler With {.Nachname = "Epsilon", .TTRating = 20}}

        Dim suche = New PaarungsSuche("Runde xyz").StandardPaarung(p.SpielerListe, 2, p)
        Assert.IsNotNull(suche)
        Assert.IsNotNull(suche.aktuellerSchwimmer)
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.AreEqual("Runde xyz", suche.Partien.First.RundenName)
    End Sub

    <TestMethod>
    Sub FreilosSpiel()
        MainWindow.AktiveCompetition = New Competition
        Dim SpielerA = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"}
        Dim Partie = New FreiLosSpiel("Runde 1", SpielerA)
        MainWindow.AktiveCompetition.SpielRunden.Push(New SpielRunde From {Partie})
        Assert.AreEqual(Partie.SpielerLinks, SpielerA)
        Assert.AreEqual(Partie.SpielerRechts, SpielerA)
        Assert.AreEqual(1, SpielerA.Punkte)
        Assert.AreEqual(0, SpielerA.SätzeGewonnen)
        Assert.AreEqual(0, SpielerA.SätzeVerloren)
    End Sub

    <TestMethod>
    Sub StandardSpiel()
        MainWindow.AktiveCompetition = New Competition With {.Gewinnsätze = 3}
        Dim SpielerA = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"}
        Dim SpielerB = New Spieler With {.Vorname = "Hartmut", .Nachname = "Seiter", .Id = "PLAYER291"}
        Dim SpielerC = New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Id = "PLAYER150"}
        Dim Partie1 = New SpielPartie("Runde 1", SpielerA, SpielerB) From {
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteRechts = 11},
                New Satz With {.PunkteLinks = 11}}

        Dim Partie2 = New SpielPartie("Runde 2", SpielerC, SpielerA) From {
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11}}

        Dim Partie3 = New SpielPartie("Runde 3", SpielerC, SpielerB) From {
               New Satz With {.PunkteLinks = 11}
        }

        With MainWindow.AktiveCompetition.SpielRunden
            .Push(New SpielRunde From {Partie1, New FreiLosSpiel("Runde 1", SpielerC)})
            .Push(New SpielRunde From {Partie2, New FreiLosSpiel("Runde 2", SpielerB)})
            .Push(New SpielRunde From {Partie3, New FreiLosSpiel("Runde 3", SpielerA)})
        End With

        Assert.AreEqual(Partie1.SpielerLinks, SpielerA)
        Assert.AreEqual(Partie1.SpielerRechts, SpielerB)
        With SpielerA
            Assert.AreEqual(2, .Punkte)
            Assert.AreEqual(1, .ExportPunkte)
            Assert.AreEqual(3, .BuchholzPunkte)
            Assert.AreEqual(3, .ExportBHZ)
            Assert.AreEqual(1, .SonneBornBergerPunkte)
            Assert.AreEqual(1, .ExportSonneborn)
            Assert.AreEqual(3, .SätzeGewonnen)
            Assert.AreEqual(4, .SätzeVerloren)
            Assert.AreEqual(3, .ExportSätzeGewonnen)
            Assert.AreEqual(4, .ExportSätzeVerloren)
        End With

        With SpielerB
            Assert.AreEqual(1, .Punkte)
            Assert.AreEqual(1, .ExportPunkte)
            Assert.AreEqual(4, .BuchholzPunkte)
            Assert.AreEqual(2, .ExportBHZ)
            Assert.AreEqual(0, .SonneBornBergerPunkte)
            Assert.AreEqual(0, .ExportSonneborn)
            Assert.AreEqual(1, .SätzeGewonnen)
            Assert.AreEqual(4, .SätzeVerloren)
            Assert.AreEqual(1, .ExportSätzeGewonnen)
            Assert.AreEqual(3, .ExportSätzeVerloren)
        End With

        With SpielerC
            Assert.AreEqual(2, .Punkte)
            Assert.AreEqual(2, .ExportPunkte)
            Assert.AreEqual(3, .BuchholzPunkte)
            Assert.AreEqual(2, .ExportBHZ)
            Assert.AreEqual(2, .SonneBornBergerPunkte)
            Assert.AreEqual(2, .ExportSonneborn)
            Assert.AreEqual(4, .SätzeGewonnen)
            Assert.AreEqual(0, .SätzeVerloren)
            Assert.AreEqual(3, .ExportSätzeGewonnen)
            Assert.AreEqual(0, .ExportSätzeVerloren)
        End With
        
    End Sub

End Class
