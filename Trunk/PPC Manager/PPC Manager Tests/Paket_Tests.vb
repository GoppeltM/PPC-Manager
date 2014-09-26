Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class Paket_Tests

    <TestInitialize>
    Sub Init()
        c = New Competition(New SpielRegeln(3, True, True))
    End Sub

    Private c As Competition

    <TestMethod>
    Sub Spieler_Sortieren()        
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
        Dim Spielerliste = (From x In XNode.Elements Select Spieler.FromXML(x, New SpielRunden, New SpielRegeln(3, True, True))).ToList
        Spielerliste.Sort()
        Spielerliste.Reverse()
        CollectionAssert.AreEqual(New String() {"Wolfert", "Westermann", "Goppelt", "Wild"}, (From x In Spielerliste Select x.Nachname).ToList)

    End Sub

    Private Function CreateSpieler(Optional nachname As String = "", Optional vorname As String = "", _
                                   Optional ttrating As Integer = 0, Optional id As String = "") As Spieler
        Dim s = New Spieler(c.SpielRunden, c.SpielRegeln)
        s.Nachname = nachname
        s.Vorname = vorname
        s.TTRating = ttrating
        s.Id = id
        Return s
    End Function

    <TestMethod>
    Sub Gerade_StandardPaarung()        
        Dim p As New Paket(1, "Runde 1", 3)
        p.SpielerListe = New List(Of Spieler) From
                {CreateSpieler(nachname:="Alpha", ttrating:=50),
                CreateSpieler(nachname:="Beta", ttrating:=40),
                CreateSpieler(nachname:="Gamma", ttrating:=30),
                CreateSpieler(nachname:="Delta", ttrating:=20)}

        Dim suche = New PaarungsSuche("Runde 1", 3).StandardPaarung(p.SpielerListe, 2, p)
        Assert.IsNotNull(suche)
        Assert.IsNull(suche.aktuellerSchwimmer)
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.AreEqual("Runde 1", suche.Partien.First.RundenName)
    End Sub

    <TestMethod>
    Sub Ungerade_StandardPaarung()        
        Dim p As New Paket(1, "Runde xyz", 3)
        p.SpielerListe = New List(Of Spieler) From
                {CreateSpieler(nachname:="Alpha", ttrating:=50),
                CreateSpieler(nachname:="Beta", ttrating:=40),
                CreateSpieler(nachname:="Gamma", ttrating:=30),
                CreateSpieler(nachname:="Delta", ttrating:=20),
                CreateSpieler(nachname:="Epsilon", ttrating:=20)}

        Dim suche = New PaarungsSuche("Runde xyz", 3).StandardPaarung(p.SpielerListe, 2, p)
        Assert.IsNotNull(suche)
        Assert.IsNotNull(suche.aktuellerSchwimmer)
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.AreEqual("Runde xyz", suche.Partien.First.RundenName)
    End Sub

    <TestMethod>
    Sub HatFreilos_FreilosPush_True()
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim Partie = New FreiLosSpiel("Runde 1", SpielerA, 3)
        Dim runde = New SpielRunde
        c.SpielRunden.Push(runde)
        runde.Add(Partie)
        Assert.IsTrue(SpielerA.HatFreilos)
    End Sub

    <TestMethod>
    Sub SpielerLinks_GleichSpielerRechts_GleichSpielerA()
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim Partie = New FreiLosSpiel("Runde 1", SpielerA, 3)
        Assert.AreEqual(Partie.SpielerLinks, SpielerA)
        Assert.AreEqual(Partie.SpielerRechts, SpielerA)
    End Sub

    <TestMethod>
    Sub HatFreilos_NormaleRunde_False()
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim SpielerB = CreateSpieler(vorname:="Marius", nachname:="Goppelt", id:="PLAYER294")
        Dim runde = New SpielRunde
        runde.Add(New SpielPartie("Runde 1", SpielerA, SpielerB, 3))
        c.SpielRunden.Push(runde)
        Assert.IsFalse(SpielerA.HatFreilos)
        Assert.IsFalse(SpielerB.HatFreilos)
    End Sub

    <TestMethod>
    Sub Punkte_VonFreilos_Gleich1()
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim Partie = New FreiLosSpiel("Runde 1", SpielerA, 3)
        Dim runde = New SpielRunde
        runde.Add(Partie)
        c.SpielRunden.Push(runde)
        Assert.IsTrue(SpielerA.HatFreilos)
        Assert.AreEqual(1, SpielerA.Punkte)
        Assert.AreEqual(0, SpielerA.SätzeGewonnen)
        Assert.AreEqual(0, SpielerA.SätzeVerloren)
    End Sub

    <TestMethod>
    Sub HatFreilos()        
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim SpielerB = CreateSpieler(vorname:="Marius", nachname:="Goppelt", id:="PLAYER111")
        Dim runde = New SpielRunde
        c.SpielRunden.Push(runde)        
        Dim Freilos = New FreiLosSpiel("Runde 1", SpielerA, 3)
        runde.Add(Freilos)
        Assert.IsTrue(SpielerA.HatFreilos)
        Assert.IsFalse(SpielerB.HatFreilos)
    End Sub

    <TestMethod>
    Sub StandardSpiel()        
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim SpielerB = CreateSpieler(vorname:="Hartmut", nachname:="Seiter", id:="PLAYER291")
        Dim SpielerC = CreateSpieler(vorname:="Marius", nachname:="Goppelt", id:="PLAYER150")
        Dim Partie1 = New SpielPartie("Runde 1", SpielerA, SpielerB, 3) From {
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteRechts = 11},
                New Satz With {.PunkteLinks = 11}}

        Dim Partie2 = New SpielPartie("Runde 2", SpielerC, SpielerA, 3) From {
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11}}

        Dim Partie3 = New SpielPartie("Runde 3", SpielerC, SpielerB, 3) From {
               New Satz With {.PunkteLinks = 11}
        }

        With c.SpielRunden
            .Push(New SpielRunde From {Partie1, New FreiLosSpiel("Runde 1", SpielerC, 3)})
            .Push(New SpielRunde From {Partie2, New FreiLosSpiel("Runde 2", SpielerB, 3)})
            .Push(New SpielRunde From {Partie3, New FreiLosSpiel("Runde 3", SpielerA, 3)})
        End With

        Assert.AreEqual(Partie1.SpielerLinks, SpielerA)
        Assert.AreEqual(Partie1.SpielerRechts, SpielerB)
        With SpielerA
            Assert.AreEqual(2, .Punkte)
            Assert.AreEqual(1, .ExportPunkte)
            Assert.AreEqual(1, .BuchholzPunkte)
            Assert.AreEqual(1, .ExportBHZ)
            Assert.AreEqual(0, .SonneBornBergerPunkte)
            Assert.AreEqual(0, .ExportSonneborn)
            Assert.AreEqual(3, .SätzeGewonnen)
            Assert.AreEqual(4, .SätzeVerloren)
            Assert.AreEqual(3, .ExportSätzeGewonnen)
            Assert.AreEqual(4, .ExportSätzeVerloren)
        End With

        With SpielerB
            Assert.AreEqual(1, .Punkte)
            Assert.AreEqual(1, .ExportPunkte)
            Assert.AreEqual(2, .BuchholzPunkte)
            Assert.AreEqual(1, .ExportBHZ)
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
            Assert.AreEqual(1, .BuchholzPunkte)
            Assert.AreEqual(1, .ExportBHZ)
            Assert.AreEqual(1, .SonneBornBergerPunkte)
            Assert.AreEqual(1, .ExportSonneborn)
            Assert.AreEqual(4, .SätzeGewonnen)
            Assert.AreEqual(0, .SätzeVerloren)
            Assert.AreEqual(3, .ExportSätzeGewonnen)
            Assert.AreEqual(0, .ExportSätzeVerloren)
        End With

        Assert.IsTrue(SpielerC.CompareTo(SpielerB) > 0)
        Assert.IsTrue(SpielerC.CompareTo(SpielerA) > 0)
        Assert.IsTrue(SpielerA.CompareTo(SpielerB) > 0)


    End Sub

    <TestMethod>
    Sub Spieler_Ausscheiden()        
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim SpielerB = CreateSpieler(vorname:="Hartmut", nachname:="Seiter", id:="PLAYER291")
        Dim SpielerC = CreateSpieler(vorname:="Marius", nachname:="Goppelt", id:="PLAYER150")
        Dim Partie1 = New SpielPartie("Runde 1", SpielerA, SpielerB, 3) From {
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteRechts = 11},
                New Satz With {.PunkteRechts = 11},
                New Satz With {.PunkteRechts = 11}}

        Dim Partie2 = New SpielPartie("Runde 2", SpielerA, SpielerC, 3) From {
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11}}

        Dim Partie3 = New SpielPartie("Runde 3", SpielerA, SpielerC, 3) From {
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11},
                New Satz With {.PunkteLinks = 11}}

        With c.SpielRunden
            .Push(New SpielRunde From {Partie1})
            With SpielerB
                Assert.AreEqual(.Punkte, 1)
                Assert.AreEqual(.ExportPunkte, 0)
                Assert.AreEqual(.BuchholzPunkte, 0)
            End With

            .AusgeschiedeneSpieler.Add(New Ausgeschieden With {.Spieler = SpielerB, .Runde = 1})
            .Push(New SpielRunde From {Partie2})

            With SpielerB
                Assert.AreEqual(.Punkte, 1)
                Assert.AreEqual(.ExportPunkte, 1)
                Assert.AreEqual(.BuchholzPunkte, 1)
                Assert.AreEqual(.ExportBHZ, 0)
            End With

            .Push(New SpielRunde From {Partie3})

            With SpielerB
                Assert.AreEqual(.Punkte, 1)
                Assert.AreEqual(.ExportPunkte, 1)
                Assert.AreEqual(.BuchholzPunkte, 2)
                Assert.AreEqual(.ExportBHZ, 1)
            End With

        End With
    End Sub

End Class
