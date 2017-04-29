Public Class SpielverlaufTests
    Private _Partien As List(Of SpielPartie)
    Private _S As Spielverlauf
    Private _Ausgeschieden As List(Of String)

    <SetUp>
    Public Sub Init()
        _Partien = New List(Of SpielPartie)
        _Ausgeschieden = New List(Of String)
        _S = New Spielverlauf(_Partien,
                              _Ausgeschieden,
                              New SpielRegeln(3, True, True), New Spielstand(3))
    End Sub

    Private Function CreateSpieler(Optional nachname As String = "", Optional vorname As String = "",
                                   Optional ttrating As Integer = 0, Optional id As String = "") As SpielerInfo
        Dim s = New SpielerInfo(id)
        s.Nachname = nachname
        s.Vorname = vorname
        s.TTRating = ttrating
        Return s
    End Function

    <Test>
    Sub AusgeschiedeneSpieler_werden_bei_BHZ_ignoriert()
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

        _Partien.Add(Partie1)
        _Ausgeschieden.Add(SpielerB.Id)

        Assert.AreEqual(_S.BerechnePunkte(SpielerB), 1)
        Assert.AreEqual(_S.BerechneBuchholzPunkte(SpielerB), 0)

        _Partien.Add(Partie2)

        Assert.AreEqual(_S.BerechnePunkte(SpielerB), 1)
        Assert.AreEqual(_S.BerechneBuchholzPunkte(SpielerB), 1)

        _Partien.Add(Partie3)
        Assert.AreEqual(_S.BerechnePunkte(SpielerB), 1)
        Assert.AreEqual(_S.BerechneBuchholzPunkte(SpielerB), 2)

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

        _Partien.AddRange({Partie1, New FreiLosSpiel("Runde 1", SpielerC, 3),
                          Partie2, New FreiLosSpiel("Runde 2", SpielerB, 3),
                          Partie3, New FreiLosSpiel("Runde 3", SpielerA, 3)})

        Assert.AreEqual(Partie1.SpielerLinks, SpielerA)
        Assert.AreEqual(Partie1.SpielerRechts, SpielerB)
        With SpielerA
            Assert.AreEqual(2, _S.BerechnePunkte(SpielerA))
            Assert.AreEqual(1, _S.BerechneBuchholzPunkte(SpielerA))
            Assert.AreEqual(0, _S.BerechneSonnebornBergerPunkte(SpielerA))
            Assert.AreEqual(3, _S.BerechneGewonneneSätze(SpielerA))
            Assert.AreEqual(4, _S.BerechneVerloreneSätze(SpielerA))
        End With

        With SpielerB
            Assert.AreEqual(1, _S.BerechnePunkte(SpielerB))
            Assert.AreEqual(2, _S.BerechneBuchholzPunkte(SpielerB))
            Assert.AreEqual(0, _S.BerechneSonnebornBergerPunkte(SpielerB))
            Assert.AreEqual(1, _S.BerechneGewonneneSätze(SpielerB))
            Assert.AreEqual(4, _S.BerechneVerloreneSätze(SpielerB))
        End With

        With SpielerC
            Assert.AreEqual(2, _S.BerechnePunkte(SpielerC))
            Assert.AreEqual(1, _S.BerechneBuchholzPunkte(SpielerC))
            Assert.AreEqual(1, _S.BerechneSonnebornBergerPunkte(SpielerC))
            Assert.AreEqual(4, _S.BerechneGewonneneSätze(SpielerC))
            Assert.AreEqual(0, _S.BerechneVerloreneSätze(SpielerC))
        End With
    End Sub

    <Test>
    Public Sub Habengegeneinandergespielt_ist_falsch_wenn_spieler_noch_nie_gespielt_haben()


        Dim spielerA = New SpielerInfo("A")
        Dim spielerB = New SpielerInfo("B")
        Dim spielerC = New SpielerInfo("C")
        _Partien.Add(New SpielPartie("Runde 1", spielerA, spielerB, 3))
        Dim result = _S.Habengegeneinandergespielt(spielerA, spielerC)
        Dim result2 = _S.Habengegeneinandergespielt(spielerC, spielerA)
        Assert.That(result, [Is].False)
        Assert.That(result2, [Is].False)
    End Sub

    <Test>
    Public Sub Habengegeneinandergespielt_ist_wahr_wenn_spieler_gegeneinander_gespielt_haben()
        Dim spielerA = New SpielerInfo("A")
        Dim spielerB = New SpielerInfo("B")
        Dim spielerC = New SpielerInfo("C")
        _Partien.Add(New SpielPartie("Runde 1", spielerA, spielerB, 3))

        Dim result = _S.Habengegeneinandergespielt(spielerA, spielerB)
        Assert.That(result, [Is].True)
    End Sub

    <Test>
    Public Sub HatFreilos_ist_wahr_wenn_Freilos_von_Spieler_in_Spielpartien()
        Dim spielerA = New SpielerInfo("A")
        _Partien.Add(New FreiLosSpiel("Runde 1", spielerA, 3))

        Dim result = _S.HatFreilos(spielerA)
        Assert.That(result, Iz.True)
    End Sub

    <Test>
    Public Sub HatFreilos_ist_falsch_wenn_Freilos_anderen_Spieler_hat()
        Dim spielerA = New SpielerInfo("A")
        Dim spielerB = New SpielerInfo("B")
        _Partien.Add(New FreiLosSpiel("Runde 1", spielerA, 3))
        _Partien.Add(New SpielPartie("Runde 1", spielerA, spielerB, 3))
        Dim result = _S.HatFreilos(spielerB)
        Assert.That(result, Iz.False)
    End Sub

    <Test>
    Public Sub BerechnePunkte_ignoriert_nicht_abgeschlossene_Partien()
        Dim spielerA = New SpielerInfo("A")
        Dim spielerB = New SpielerInfo("B")
        Dim p = New SpielPartie("Runde 1", spielerA, spielerB, 3) From {
            New Satz With {.PunkteLinks = 11},
            New Satz With {.PunkteLinks = 11}
            }
        _Partien.Add(p)
        Dim punkte = _S.BerechnePunkte(spielerA)
        Assert.That(punkte, Iz.EqualTo(0))
    End Sub

    <Test>
    Public Sub BerechnePunkte_berücksichtigt_abgeschlossene_Partien()
        Dim spielerA = New SpielerInfo("A")
        Dim spielerB = New SpielerInfo("B")
        Dim p = New SpielPartie("Runde 1", spielerA, spielerB, 3) From {
            New Satz With {.PunkteLinks = 11},
            New Satz With {.PunkteLinks = 11},
            New Satz With {.PunkteLinks = 11}
            }
        _Partien.Add(p)
        Dim punkte = _S.BerechnePunkte(spielerA)
        Assert.That(punkte, Iz.EqualTo(1))
    End Sub

End Class

