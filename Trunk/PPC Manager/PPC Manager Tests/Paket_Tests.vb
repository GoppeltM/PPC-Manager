Imports System.Text

<TestFixture()> Public Class Paket_Tests

    <SetUp>
    Sub Init()
        c = New Competition(New SpielRegeln(3, True, True))
    End Sub

    Private c As Competition



    Private Function CreateSpieler(Optional nachname As String = "", Optional vorname As String = "",
                                   Optional ttrating As Integer = 0, Optional id As String = "") As Spieler
        Dim s = New Spieler(c.SpielRunden, c.SpielRegeln)
        s.Nachname = nachname
        s.Vorname = vorname
        s.TTRating = ttrating
        s.Id = id
        Return s
    End Function



    <Test>
    Sub HatFreilos_FreilosPush_True()
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim Partie = New FreiLosSpiel("Runde 1", SpielerA, 3)
        Dim runde = New SpielRunde
        c.SpielRunden.Push(runde)
        runde.Add(Partie)
        Assert.IsTrue(SpielerA.HatFreilos)
    End Sub

    <Test>
    Sub SpielerLinks_GleichSpielerRechts_GleichSpielerA()
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim Partie = New FreiLosSpiel("Runde 1", SpielerA, 3)
        Assert.AreEqual(Partie.SpielerLinks, SpielerA)
        Assert.AreEqual(Partie.SpielerRechts, SpielerA)
    End Sub

    <Test>
    Sub HatFreilos_NormaleRunde_False()
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim SpielerB = CreateSpieler(vorname:="Marius", nachname:="Goppelt", id:="PLAYER294")
        Dim runde = New SpielRunde
        runde.Add(New SpielPartie("Runde 1", SpielerA, SpielerB, 3))
        c.SpielRunden.Push(runde)
        Assert.IsFalse(SpielerA.HatFreilos)
        Assert.IsFalse(SpielerB.HatFreilos)
    End Sub

    <Test>
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

    <Test>
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

    <Test>
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
            Assert.AreEqual(1, .BuchholzPunkte)
            Assert.AreEqual(0, .SonneBornBergerPunkte)
            Assert.AreEqual(3, .SätzeGewonnen)
            Assert.AreEqual(4, .SätzeVerloren)
        End With

        With SpielerB
            Assert.AreEqual(1, .Punkte)
            Assert.AreEqual(2, .BuchholzPunkte)
            Assert.AreEqual(0, .SonneBornBergerPunkte)
            Assert.AreEqual(1, .SätzeGewonnen)
            Assert.AreEqual(4, .SätzeVerloren)
        End With

        With SpielerC
            Assert.AreEqual(2, .Punkte)
            Assert.AreEqual(1, .BuchholzPunkte)
            Assert.AreEqual(1, .SonneBornBergerPunkte)
            Assert.AreEqual(4, .SätzeGewonnen)
            Assert.AreEqual(0, .SätzeVerloren)
        End With

        Assert.IsTrue(SpielerC.CompareTo(SpielerB) > 0)
        Assert.IsTrue(SpielerC.CompareTo(SpielerA) > 0)
        Assert.IsTrue(SpielerA.CompareTo(SpielerB) > 0)
    End Sub

    <Test>
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
                Assert.AreEqual(.BuchholzPunkte, 0)
            End With

            .AusgeschiedeneSpieler.Add(New Ausgeschieden With {.Spieler = SpielerB, .Runde = 1})
            .Push(New SpielRunde From {Partie2})

            With SpielerB
                Assert.AreEqual(.Punkte, 1)
                Assert.AreEqual(.BuchholzPunkte, 1)
            End With

            .Push(New SpielRunde From {Partie3})

            With SpielerB
                Assert.AreEqual(.Punkte, 1)
                Assert.AreEqual(.BuchholzPunkte, 2)
            End With

        End With
    End Sub

End Class
