Imports Moq

<TestFixture>
Public Class PPC15_Turnier_Klasse_D

    <SetUp>
    Sub CompetitionInit()
        Dim KlassementD = (From x In XDocument.Parse(My.Resources.PPC_15_Anmeldungen).Root.<competition>
                           Where x.Attribute("age-group").Value = "D-Klasse").First
        Dim regeln = New SpielRegeln(3, True, False)
        Dim r As New SpielRunden
        Dim s As New Spielverlauf(r.SelectMany(Function(m) m),
                                  r.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs),
                                  New Spielstand(3))
        Dim habenGegeinanderGespielt = Function(a As SpielerInfo, b As SpielerInfo) s.Habengegeneinandergespielt(a, b)

        Dim ausgeschiedeneSpielerIds = r.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs)
        AktuelleCompetition = AusXML.CompetitionFromXML("D:\dummy.xml", KlassementD, regeln, r)
        Dim AktiveListe = From x In AktuelleCompetition.SpielerListe
                          Where Not ausgeschiedeneSpielerIds.Contains(x.Id)
                          Select x
        Dim OrganisierePakete = Function()
                                    Dim spielverlaufCache = New SpielverlaufCache(s)
                                    Dim comparer = New SpielerInfoComparer(spielverlaufCache, regeln.SatzDifferenz, regeln.SonneBornBerger)
                                    Dim paarungsSuche = New PaarungsSuche(Of SpielerInfo)(AddressOf comparer.Compare, habenGegeinanderGespielt)
                                    Dim begegnungen = New PaketBildung(Of SpielerInfo)(spielverlaufCache, AddressOf paarungsSuche.SuchePaarungen)
                                    Dim l = AktiveListe.ToList()
                                    l.Sort(comparer)
                                    l.Reverse()
                                    Return begegnungen.organisierePakete(l, r.Count - 1)
                                End Function

        Dim druckFabrik = Mock.Of(Of IFixedPageFabrik)
        _Controller = New MainWindowController(Sub()

                                               End Sub,
                                               Mock.Of(Of IReportFactory),
                                               OrganisierePakete,
                                               druckFabrik, 3)
    End Sub

    Dim AktuelleCompetition As Competition
    Private _SuchePaarungenFunc As Func(Of Predicate(Of Spieler), SuchePaarungen(Of Spieler))
    Private _Controller As MainWindowController

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
        Dim NachnamenResult = (From x In tatsächlich Order By x.Nachname Descending Select x.Nachname).ToList
        Assert.That(Nachnamen, [Is].EquivalentTo(NachnamenResult))
    End Sub

    <Test>
    Sub Runde_1()
        With AktuelleCompetition
            Dim ergebnisse = _Controller.NächsteRunde("Runde 1")
            .SpielRunden.Push(ergebnisse)
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
            Dim ergebnisse = _Controller.NächsteRunde("Runde 2")
            .SpielRunden.Push(ergebnisse)
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
            Dim ergebnisse = _Controller.NächsteRunde("Runde 3")
            .SpielRunden.Push(ergebnisse)
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
            Dim ergebnisse = _Controller.NächsteRunde("Runde 4")
            .SpielRunden.Push(ergebnisse)
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
            Dim ergebnisse = _Controller.NächsteRunde("Runde 5")
            .SpielRunden.Push(ergebnisse)
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
            .SpielRunden.Peek.AusgeschiedeneSpielerIDs.Add(Ausgeschieden.Id)
            Dim ergebnisse = _Controller.NächsteRunde("Runde 6")
            .SpielRunden.Push(ergebnisse)
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
                ergebnis.spielPartie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
            Next
            For Each Satz In Enumerable.Range(0, ergebnis.SätzeRechts)
                ergebnis.spielPartie.Add(New Satz() With {.PunkteLinks = 0, .PunkteRechts = 11})
            Next
        Next
    End Sub

    Private Function BegegnungenZuVergleicher(begegnungen As IEnumerable(Of SpielPartie)) As IEnumerable(Of BegegnungsVergleicher)
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