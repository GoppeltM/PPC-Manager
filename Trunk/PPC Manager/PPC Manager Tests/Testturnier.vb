Imports System.Text
Imports System.Xml.Schema
Imports Moq

''' <summary>
''' Ein Turnier wo jeder gegen jeden spielt, d.h. 11 Spieler mit 11 Runden.
''' Dies ist quasi der komplexeste Paarungstest. Danke an Flo für die Mühe.
''' </summary>
''' <remarks></remarks>
<TestFixture()> Public Class Testturnier
    Private _Controller As MainWindowController
    Private _JungenU18 As XElement
    Private _AktiveListe As SpielerListe

    <SetUp>
    Sub CompetitionInit()
        _JungenU18 = (From x In XDocument.Parse(My.Resources.Testturnier).Root.<competition>
                      Where x.Attribute("age-group").Value = "Jungen U 18").First
        Dim regeln = New SpielRegeln(3, True, True)
        Dim r As New SpielRunden
        Dim s As New Spielverlauf(r.SelectMany(Function(m) m), New List(Of SpielerInfo), New SpielRegeln(3, True, True))
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
        Dim AktuelleCompetition = AusXML.CompetitionFromXML("D:\dummy.xml",
                                                            _JungenU18,
                                                            regeln, s, r)
        _AktiveListe = AktuelleCompetition.SpielerListe
        AktuelleCompetition.SpielRunden.Clear()
        _Controller = New MainWindowController(r, AktuelleCompetition, Sub()
                                                                       End Sub,
                                               Mock.Of(Of IReportFactory),
                                               OrganisierePakete)
    End Sub


    <Test> Public Sub Schema_Validierung()
        Dim doc = XDocument.Parse(My.Resources.Testturnier)
        Dim schema As XmlSchema
        Dim Pfad = IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly.Location)
        Pfad &= "\..\..\..\PPC Manager\SpeicherStandSchema.xsd"
        Using stream = New IO.FileStream(Pfad, IO.FileMode.Open)
            schema = XmlSchema.Read(stream, Nothing)
        End Using

        Dim schemaSet As New XmlSchemaSet
        schemaSet.Add(schema)

        schemaSet.Compile()

        doc.Validate(schemaSet, Nothing)
    End Sub

    <Test>
    Sub Jugend_Runde_1()
        NächsteRunde("Runde 1", 0)
    End Sub

    <Test>
    Sub Jugend_Runde_2()
        Jugend_Runde_1()
        NächsteRunde("Runde 2", 1)
    End Sub


    <Test>
    Sub Jugend_Runde_3()
        Jugend_Runde_2()
        NächsteRunde("Runde 3", 2)
    End Sub


    <Test>
    Sub Jugend_Runde_4()
        Jugend_Runde_3()
        NächsteRunde("Runde 4", 3)
    End Sub

    <Test>
    Sub Jugend_Runde_5()
        Jugend_Runde_4()
        NächsteRunde("Runde 5", 4)
    End Sub

    <Test>
    Sub Jugend_Runde_6()
        Jugend_Runde_5()
        NächsteRunde("Runde 6", 5)
    End Sub

    <Test>
    Sub Jugend_Runde_7()
        Jugend_Runde_6()
        NächsteRunde("Runde 7", 6)
    End Sub

    <Test>
    Sub Jugend_Runde_8()
        Jugend_Runde_7()
        NächsteRunde("Runde 8", 7)
    End Sub

    <Test>
    Sub Jugend_Runde_9()
        Jugend_Runde_8()
        NächsteRunde("Runde 9", 8)
    End Sub

    <Test>
    Sub Jugend_Runde_10()
        Jugend_Runde_9()
        NächsteRunde("Runde 10", 9)
    End Sub

    <Test>
    Sub Jugend_Runde_11()
        Jugend_Runde_10()
        NächsteRunde("Runde 11", 10)
    End Sub

    Private Sub NächsteRunde(rundenName As String, rundenNummer As Integer)

        Dim XMLPartien = From x In _JungenU18.<matches>.Elements Where x.@group = rundenName
        Dim ErwarteteErgebnisse = From x In AusXML.SpielRundeFromXML(_AktiveListe, XMLPartien, 3) Order By x.GetType.Name Descending

        _Controller.NächsteRunde()
        Dim paarungen = _Controller.AktiveCompetition.SpielRunden.First()

        Assert.AreEqual(paarungen.Count, ErwarteteErgebnisse.Count)

        For Each Paar In paarungen.Zip(ErwarteteErgebnisse, Function(x, y) New With {.Aktuell = x, .Erwartet = y})
            Assert.AreEqual(Paar.Aktuell.SpielerLinks, Paar.Erwartet.SpielerLinks)
            Assert.AreEqual(Paar.Aktuell.SpielerRechts, Paar.Erwartet.SpielerRechts)
            For Each satz In Paar.Erwartet
                Paar.Aktuell.Add(satz)
            Next
        Next

    End Sub

End Class