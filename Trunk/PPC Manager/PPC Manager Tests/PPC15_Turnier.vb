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
                                  New List(Of SpielerInfo),
                                  regeln)
        Dim habenGegeinanderGespielt = Function(a As SpielerInfo, b As SpielerInfo) s.Habengegeneinandergespielt(a, b)

        Dim OrganisierePakete = Function(spielerListe As IEnumerable(Of SpielerInfo), spielrunde As Integer)
                                    Dim spielverlaufCache = New SpielverlaufCache(s)
                                    Dim comparer = New SpielerInfoComparer(spielverlaufCache)
                                    Dim paarungsSuche = New PaarungsSuche(Of SpielerInfo)(AddressOf comparer.Compare, habenGegeinanderGespielt)
                                    Dim begegnungen = New PaketBildung(Of SpielerInfo)(spielverlaufCache, AddressOf paarungsSuche.SuchePaarungen)
                                    Dim l = spielerListe.ToList()
                                    l.Sort(comparer)
                                    l.Reverse()
                                    Return begegnungen.organisierePakete(l, spielrunde)
                                End Function
        AktuelleCompetition = AusXML.CompetitionFromXML("D:\dummy.xml", KlassementD, regeln, s, r)
        _Controller = New MainWindowController(AktuelleCompetition, Sub()

                                                                    End Sub,
                                               Mock.Of(Of IReportFactory),
                                               OrganisierePakete)
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
        tatsächlich.Sort()
        tatsächlich.Reverse()
        Dim NachnamenResult = (From x In tatsächlich Select x.Nachname).ToList
        CollectionAssert.AreEqual(Nachnamen, NachnamenResult)
    End Sub

    <Test>
    Sub Runde_1()
        With AktuelleCompetition
            _Controller.NächsteRunde_Execute()
            Dim ergebnisse = .SpielRunden.First
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
            _Controller.NächsteRunde_Execute()
            Dim ergebnisse = .SpielRunden.First
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
            _Controller.NächsteRunde_Execute()
            Dim ergebnisse = .SpielRunden.First
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
            _Controller.NächsteRunde_Execute()
            Dim ergebnisse = .SpielRunden.First
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
            _Controller.NächsteRunde_Execute()
            Dim ergebnisse = .SpielRunden.First
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
            .SpielRunden.AusgeschiedeneSpieler.Add(New Ausgeschieden(Of SpielerInfo) With {.Spieler = Ausgeschieden, .Runde = 6})
            Dim AktiveListe = .SpielerListe.OfType(Of SpielerInfo).ToList
            For Each a In .SpielRunden.AusgeschiedeneSpieler
                AktiveListe.Remove(a.Spieler)
            Next
            _Controller.NächsteRunde_Execute()
            Dim ergebnisse = .SpielRunden.First
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
                _Controller.SatzEintragen(0, True, ergebnis.spielPartie)
            Next
            For Each Satz In Enumerable.Range(0, ergebnis.SätzeRechts)
                _Controller.SatzEintragen(0, False, ergebnis.spielPartie)
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