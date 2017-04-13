﻿Public Class SpielverlaufTests
    Private _Partien As List(Of SpielPartie)
    Private _S As Spielverlauf

    <SetUp>
    Public Sub Init()
        _Partien = New List(Of SpielPartie)
        _S = New Spielverlauf(_Partien,
                              New List(Of SpielerInfo),
                              New SpielRegeln(3, True, True))
    End Sub

    <Test>
    Public Sub Habengegeneinandergespielt_ist_falsch_wenn_spieler_noch_nie_gespielt_haben()


        Dim spielerA = New SpielerInfo With {.Id = "A"}
        Dim spielerB = New SpielerInfo With {.Id = "B"}
        Dim spielerC = New SpielerInfo With {.Id = "C"}
        _Partien.Add(New SpielPartie("Runde 1", spielerA, spielerB, 3))
        Dim result = _S.Habengegeneinandergespielt(spielerA, spielerC)
        Dim result2 = _S.Habengegeneinandergespielt(spielerC, spielerA)
        Assert.That(result, [Is].False)
        Assert.That(result2, [Is].False)
    End Sub

    <Test>
    Public Sub Habengegeneinandergespielt_ist_wahr_wenn_spieler_gegeneinander_gespielt_haben()
        Dim spielerA = New SpielerInfo With {.Id = "A"}
        Dim spielerB = New SpielerInfo With {.Id = "B"}
        Dim spielerC = New SpielerInfo With {.Id = "C"}
        _Partien.Add(New SpielPartie("Runde 1", spielerA, spielerB, 3))

        Dim result = _S.Habengegeneinandergespielt(spielerA, spielerB)
        Assert.That(result, [Is].True)
    End Sub

    <Test>
    Public Sub HatFreilos_ist_wahr_wenn_Freilos_von_Spieler_in_Spielpartien()
        Dim spielerA = New SpielerInfo With {.Id = "A"}
        _Partien.Add(New FreiLosSpiel("Runde 1", spielerA, 3))

        Dim result = _S.HatFreilos(spielerA)
        Assert.That(result, Iz.True)
    End Sub

    <Test>
    Public Sub HatFreilos_ist_falsch_wenn_Freilos_anderen_Spieler_hat()
        Dim spielerA = New SpielerInfo With {.Id = "A"}
        Dim spielerB = New SpielerInfo With {.Id = "B"}
        _Partien.Add(New FreiLosSpiel("Runde 1", spielerA, 3))
        _Partien.Add(New SpielPartie("Runde 1", spielerA, spielerB, 3))
        Dim result = _S.HatFreilos(spielerB)
        Assert.That(result, Iz.False)
    End Sub

    <Test>
    Public Sub BerechnePunkte_ignoriert_nicht_abgeschlossene_Partien()
        Dim spielerA = New SpielerInfo With {.Id = "A"}
        Dim spielerB = New SpielerInfo With {.Id = "B"}
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
        Dim spielerA = New SpielerInfo With {.Id = "A"}
        Dim spielerB = New SpielerInfo With {.Id = "B"}
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
