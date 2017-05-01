Public Class SpielverlaufTests
    Private _Partien As List(Of SpielPartie)
    Private _S As Spielverlauf
    Private _Ausgeschieden As List(Of String)
    Private _Spielstand As Mock(Of ISpielstand)

    <SetUp>
    Public Sub Init()
        _Partien = New List(Of SpielPartie)
        _Ausgeschieden = New List(Of String)
        _Spielstand = New Mock(Of ISpielstand)
        _S = New Spielverlauf(_Partien,
                              _Ausgeschieden,
                              _Spielstand.Object)
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
        Dim Partie1 = New SpielPartie("Runde 1", SpielerA, SpielerB, 3)
        _Spielstand.Setup(Function(m) m.HatPartieGewonnen(It.Is(Of SpielPartie)(Function(a) a = Partie1), SpielerB)).Returns(True)
        Dim Partie2 = New SpielPartie("Runde 2", SpielerA, SpielerC, 3)
        _Spielstand.Setup(Function(m) m.HatPartieGewonnen(It.Is(Of SpielPartie)(Function(a) a = Partie2), SpielerA)).Returns(True)

        Dim Partie3 = New SpielPartie("Runde 3", SpielerA, SpielerC, 3)
        _Spielstand.Setup(Function(m) m.HatPartieGewonnen(It.Is(Of SpielPartie)(Function(a) a = Partie3), SpielerA)).Returns(True)

        _Partien.Add(Partie1)
        _Ausgeschieden.Add(SpielerB.Id)

        Assert.That(_S.BerechnePunkte(SpielerB), [Is].EqualTo(1))
        Assert.That(_S.BerechneBuchholzPunkte(SpielerB), Iz.EqualTo(0))

        _Partien.Add(Partie2)

        Assert.That(_S.BerechnePunkte(SpielerB), Iz.EqualTo(1))
        Assert.That(_S.BerechneBuchholzPunkte(SpielerB), Iz.EqualTo(1))

        _Partien.Add(Partie3)
        Assert.That(_S.BerechnePunkte(SpielerB), Iz.EqualTo(1))
        Assert.That(_S.BerechneBuchholzPunkte(SpielerB), Iz.EqualTo(2))

    End Sub

    <Test>
    Sub StandardSpiel()
        Dim SpielerA = CreateSpieler(vorname:="Florian", nachname:="Ewald", id:="PLAYER293")
        Dim SpielerB = CreateSpieler(vorname:="Hartmut", nachname:="Seiter", id:="PLAYER291")
        Dim SpielerC = CreateSpieler(vorname:="Marius", nachname:="Goppelt", id:="PLAYER150")
        Dim Partie1 = New SpielPartie("Runde 1", SpielerA, SpielerB, 3)
        _Spielstand.Setup(Function(m) m.HatPartieGewonnen(It.Is(Of SpielPartie)(Function(a) a = Partie1), SpielerA)).Returns(True)

        Dim Partie2 = New SpielPartie("Runde 2", SpielerC, SpielerA, 3)
        _Spielstand.Setup(Function(m) m.HatPartieGewonnen(It.Is(Of SpielPartie)(Function(a) a = Partie2), SpielerC)).Returns(True)

        Dim Partie3 = New SpielPartie("Runde 3", SpielerC, SpielerB, 3)

        Dim f1 = New FreiLosSpiel("Runde 1", SpielerC, 3)
        Dim f2 = New FreiLosSpiel("Runde 2", SpielerB, 3)
        Dim f3 = New FreiLosSpiel("Runde 3", SpielerA, 3)
        _Partien.AddRange({Partie1, f1,
                          Partie2, f2,
                          Partie3, f3})

        _Spielstand.Setup(Function(m) m.HatPartieGewonnen(It.Is(Of FreiLosSpiel)(Function(a) a = f1), SpielerC)).Returns(True)
        _Spielstand.Setup(Function(m) m.HatPartieGewonnen(It.Is(Of FreiLosSpiel)(Function(a) a = f2), SpielerB)).Returns(True)
        _Spielstand.Setup(Function(m) m.HatPartieGewonnen(It.Is(Of FreiLosSpiel)(Function(a) a = f3), SpielerA)).Returns(True)

        Assert.AreEqual(Partie1.SpielerLinks, SpielerA)
        Assert.AreEqual(Partie1.SpielerRechts, SpielerB)
        With SpielerA
            Assert.AreEqual(2, _S.BerechnePunkte(SpielerA))
            Assert.AreEqual(1, _S.BerechneBuchholzPunkte(SpielerA))
            Assert.AreEqual(0, _S.BerechneSonnebornBergerPunkte(SpielerA))
        End With

        With SpielerB
            Assert.AreEqual(1, _S.BerechnePunkte(SpielerB))
            Assert.AreEqual(2, _S.BerechneBuchholzPunkte(SpielerB))
            Assert.AreEqual(0, _S.BerechneSonnebornBergerPunkte(SpielerB))
        End With

        With SpielerC
            Assert.AreEqual(2, _S.BerechnePunkte(SpielerC))
            Assert.AreEqual(1, _S.BerechneBuchholzPunkte(SpielerC))
            Assert.AreEqual(1, _S.BerechneSonnebornBergerPunkte(SpielerC))
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
        Dim p = New SpielPartie("Runde 1", spielerA, spielerB, 3)
        _Partien.Add(p)
        _Spielstand.Setup(Function(x) x.HatPartieGewonnen(p, spielerA)).Returns(True)
        Dim punkte = _S.BerechnePunkte(spielerA)
        Assert.That(punkte, Iz.EqualTo(1))
    End Sub

End Class

