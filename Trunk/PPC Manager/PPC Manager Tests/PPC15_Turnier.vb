﻿<TestFixture>
Public Class PPC15_Turnier_Klasse_D

    <Setup>
    Sub CompetitionInit()
        Dim KlassementD = (From x In XDocument.Parse(My.Resources.PPC_15_Anmeldungen).Root.<competition>
                           Where x.Attribute("age-group").Value = "D-Klasse").First
        Dim regeln = New SpielRegeln(3, True, False)
        AktuelleCompetition = AusXML.CompetitionFromXML("D:\dummy.xml", KlassementD, regeln)
        Dim spielpartien = AktuelleCompetition.SpielRunden.SelectMany(Function(m) m.AsEnumerable)
        Dim spielverlauf As New Spielverlauf(spielpartien)
        Dim habenGegeinanderGespielt = Function(a As Spieler, b As Spieler) spielverlauf.Habengegeneinandergespielt(a, b)
        _SuchePaarungenFunc = Function(istAltschwimmer As Predicate(Of Spieler)) As SuchePaarungen(Of Spieler)
                                  Return Function(spielerliste, absteigend) _
                                     New PaarungsSuche(Of Spieler)(habenGegeinanderGespielt, istAltschwimmer).SuchePaarungen(spielerliste, absteigend)
                              End Function
    End Sub

    Dim AktuelleCompetition As Competition
    Private _SuchePaarungenFunc As Func(Of Predicate(Of Spieler), SuchePaarungen(Of Spieler))

    <Test>
    Sub SpielerAnmeldungen()

        Assert.AreEqual(56, AktuelleCompetition.SpielerListe.Count)

        For Each Spieler In AktuelleCompetition.SpielerListe
            Assert.IsTrue(Spieler.Fremd)
        Next

        Dim Nachnamen = New List(Of String) From {
        "Horner",
        "Ullrich",
        "Pastorini",
        "Fabricius",
        "Wolfert",
        "Errerd",
        "Hausmann",
        "Biendl",
        "Brecht",
        "Flörchinger",
        "Münzing",
        "Scherer",
        "Ewald",
        "Biendl",
        "Bernhardt",
        "Sturm",
        "Westermann",
        "Storzum",
        "Haug",
        "Schweikhardt",
        "Lessing",
        "Kullack",
        "Kühn",
        "Neumann",
        "Scholl",
        "Holzwarth",
        "Mühleck",
        "Benko",
        "Breitner",
        "Dengel",
        "Kahle",
        "Kocon",
        "Spindler",
        "Riffel",
        "Kling",
        "Schmidt",
        "Gross",
        "Prautzsch",
        "Kellenberger",
        "Reichardt",
        "Klett",
        "Friedrich",
        "Breitner",
        "Abdulkarim",
        "Maus",
        "Hefner",
        "Stucky",
        "Schmidt",
        "Bardoll",
        "Kumb",
        "Fauth",
        "Jäkel",
        "Mayer",
        "Kullack",
        "Krüger",
        "Wild"
        }
        Dim tatsächlich = AktuelleCompetition.SpielerListe.ToList
        tatsächlich.Sort()
        tatsächlich.Reverse()
        Dim NachnamenResult = (From x In tatsächlich Select x.Nachname).ToList
        CollectionAssert.AreEqual(Nachnamen, NachnamenResult)
    End Sub

    <Test>
    Sub Runde_1()
        With AktuelleCompetition
            Dim ergebnisse = New PaketBildung(_SuchePaarungenFunc, "Runde 0", 3).organisierePakete(.SpielerListe.ToList, 0)
            Dim tatsächlich = BegegnungenZuVergleicher(ergebnisse)
            Dim erwartet = XmlRundezuVergleicher(1)
            CollectionAssert.AreEquivalent(erwartet.ToList, tatsächlich.ToList)
            TrageSätzeEin(ergebnisse, erwartet)
        End With

    End Sub

    <Test>
    Sub Runde_2()
        Runde_1()
        With AktuelleCompetition
            Dim ergebnisse = New PaketBildung(_SuchePaarungenFunc, "Runde 1", 3).organisierePakete(.SpielerListe.ToList, 1)
            Dim tatsächlich = BegegnungenZuVergleicher(ergebnisse)
            Dim erwartet = XmlRundezuVergleicher(2)
            CollectionAssert.AreEquivalent(erwartet.ToList, tatsächlich.ToList)
            TrageSätzeEin(ergebnisse, erwartet)
        End With

    End Sub

    <Test>
    Sub Runde_3()
        Runde_2()
        With AktuelleCompetition
            Dim ergebnisse = New PaketBildung(_SuchePaarungenFunc, "Runde 2", 3).organisierePakete(.SpielerListe.ToList, 2)
            Dim tatsächlich = BegegnungenZuVergleicher(ergebnisse)
            Dim erwartet = XmlRundezuVergleicher(3)
            CollectionAssert.AreEquivalent(erwartet.ToList, tatsächlich.ToList)
            TrageSätzeEin(ergebnisse, erwartet)
        End With

    End Sub

    <Test>
    Sub Runde_4()
        Runde_3()
        With AktuelleCompetition
            Dim ergebnisse = New PaketBildung(_SuchePaarungenFunc, "Runde 3", 3).organisierePakete(.SpielerListe.ToList, 3)
            Dim tatsächlich = BegegnungenZuVergleicher(ergebnisse)
            Dim erwartet = XmlRundezuVergleicher(4)
            CollectionAssert.AreEquivalent(erwartet.ToList, tatsächlich.ToList)
            TrageSätzeEin(ergebnisse, erwartet)
        End With

    End Sub

    <Test>
    Sub Runde_5()
        Runde_4()
        With AktuelleCompetition
            Dim ergebnisse = New PaketBildung(_SuchePaarungenFunc, "Runde 4", 3).organisierePakete(.SpielerListe.ToList, 4)
            Dim tatsächlich = BegegnungenZuVergleicher(ergebnisse)
            Dim erwartet = XmlRundezuVergleicher(5)
            CollectionAssert.AreEquivalent(erwartet.ToList, tatsächlich.ToList)
            TrageSätzeEin(ergebnisse, erwartet)
        End With

    End Sub

    <Test>
    Sub Runde_6()
        Runde_5()
        With AktuelleCompetition
            Dim Ausgeschieden = (From x In AktuelleCompetition.SpielerListe Where x.Nachname = "Haug").Single
            .SpielRunden.AusgeschiedeneSpieler.Add(New Ausgeschieden With {.Spieler = Ausgeschieden, .Runde = 6})
            Dim AktiveListe = .SpielerListe.ToList
            For Each a In .SpielRunden.AusgeschiedeneSpieler
                AktiveListe.Remove(a.Spieler)
            Next
            Dim ergebnisse = New PaketBildung(_SuchePaarungenFunc, "Runde 5", 3).organisierePakete(AktiveListe.ToList, 5)
            Dim tatsächlich = BegegnungenZuVergleicher(ergebnisse)
            Dim erwartet = XmlRundezuVergleicher(6)
            CollectionAssert.AreEquivalent(erwartet.ToList, tatsächlich.ToList)
            TrageSätzeEin(ergebnisse, erwartet)
        End With

    End Sub

    Private Sub TrageSätzeEin(ergebnisse As IEnumerable(Of SpielPartie), erwartet As IEnumerable(Of BegegnungsVergleicher))

        Dim VollständigeErgebnisse = From spielPartie In ergebnisse Join y In erwartet
           On spielPartie.SpielerLinks.Nachname & spielPartie.SpielerLinks.Vorname Equals y.NachnameLinks & y.VornameLinks
           Select spielPartie, SätzeLinks = y.SätzeLinks, SätzeRechts = y.SätzeRechts

        Assert.IsTrue(VollständigeErgebnisse.Any)

        For Each ergebnis In VollständigeErgebnisse
            ergebnis.spielPartie.Clear()
            For Each Satz In Enumerable.Range(0, ergebnis.SätzeLinks)
                ergebnis.spielPartie.Add(New Satz With {.PunkteLinks = 11})
            Next
            For Each Satz In Enumerable.Range(0, ergebnis.SätzeRechts)
                ergebnis.spielPartie.Add(New Satz With {.PunkteRechts = 11})
            Next
        Next

        Dim spielRunde As New SpielRunde

        For Each spielPartie In ergebnisse
            spielRunde.Add(spielPartie)
        Next
            AktuelleCompetition.SpielRunden.Push(spielRunde)
    End Sub

    Private Function BegegnungenZuVergleicher(begegnungen As List(Of SpielPartie)) As IEnumerable(Of BegegnungsVergleicher)
        Return From x In begegnungen Select New BegegnungsVergleicher With {
           .NachnameLinks = x.SpielerLinks.Nachname,
           .NachnameRechts = x.SpielerRechts.Nachname,
           .VornameLinks = x.SpielerLinks.Vorname,
           .VornameRechts = x.SpielerRechts.Vorname
        }
    End Function

    Private Function XmlRundezuVergleicher(runde As Integer) As IEnumerable(Of BegegnungsVergleicher)
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Ergebnisse)
        Dim KlassementD = From x In doc.Root.<Klassement> Where x.@Name = "D" Select x
        Dim AktuelleRunde = From x In KlassementD.<Runde> Where x.@Nummer = runde.ToString
        Dim result = From y In AktuelleRunde.<Ergebnis>
           Select New BegegnungsVergleicher With {
               .VornameLinks = y.@VornameA,
               .VornameRechts = y.@VornameB,
               .NachnameLinks = y.@SpielerA,
               .NachnameRechts = y.@SpielerB,
               .SätzeLinks = Integer.Parse(y.@SätzeA),
               .SätzeRechts = Integer.Parse(y.@SätzeB)
        }
        Dim Freispiel = From x In AktuelleRunde.<Freilos>
                        Select New BegegnungsVergleicher With {
        .VornameLinks = x.@Vorname,
        .VornameRechts = x.@Vorname,
        .NachnameLinks = x.@Nachname,
        .NachnameRechts = x.@Nachname
        }
        Return result.Concat(Freispiel)
    End Function

End Class

Class BegegnungsVergleicher
    Implements IEquatable(Of BegegnungsVergleicher)

    Property VornameLinks As String
    Property VornameRechts As String
    Property NachnameLinks As String
    Property NachnameRechts As String
    Property SätzeLinks As Integer
    Property SätzeRechts As Integer


    Public Function Equals1(other As BegegnungsVergleicher) As Boolean Implements IEquatable(Of BegegnungsVergleicher).Equals
        If other Is Nothing Then Return False
        If NachnameLinks <> other.NachnameLinks Then
            other.Spiegeln()
            If NachnameLinks <> other.NachnameLinks Then Return False
        End If
        If NachnameRechts <> other.NachnameRechts Then Return False
        Return True
    End Function

    Private Sub Spiegeln()
        Dim temp = NachnameLinks
        NachnameLinks = NachnameRechts
        NachnameRechts = temp
    End Sub

    Public Overrides Function Equals(obj As Object) As Boolean
        Return Equals1(TryCast(obj, BegegnungsVergleicher))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return NachnameLinks.GetHashCode Xor NachnameRechts.GetHashCode
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0}:{1}", NachnameLinks, NachnameRechts)
    End Function
End Class