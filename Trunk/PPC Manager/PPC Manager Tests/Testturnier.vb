Imports System.Text
Imports System.Xml.Schema

''' <summary>
''' Ein Turnier wo jeder gegen jeden spielt, d.h. 11 Spieler mit 11 Runden.
''' Dies ist quasi der komplexeste Paarungstest. Danke an Flo für die Mühe.
''' </summary>
''' <remarks></remarks>
<TestFixture()> Public Class Testturnier

    <SetUp>
    Sub CompetitionInit()
        JungenU18 = (From x In XDocument.Parse(My.Resources.Testturnier).Root.<competition>
                           Where x.Attribute("age-group").Value = "Jungen U 18").First
        Dim regeln = New SpielRegeln(3, True, True)
        AktuelleCompetition = AusXML.CompetitionFromXML("D:\dummy.xml", JungenU18, regeln)
        AktuelleCompetition.SpielRunden.Clear()

        AktiveListe = AktuelleCompetition.SpielerListe.ToList
        For Each Ausgeschieden In AktuelleCompetition.SpielRunden.AusgeschiedeneSpieler
            AktiveListe.Remove(Ausgeschieden.Spieler)
        Next
    End Sub

    Dim JungenU18 As XElement
    Dim AktuelleCompetition As Competition
    Dim AktiveListe As List(Of Spieler)

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

        Dim AktuellePaarungen = New PaketBildung(rundenName, 3).organisierePakete(AktiveListe.ToList, rundenNummer)

        Dim XMLPartien = From x In JungenU18.<matches>.Elements Where x.@group = rundenName
        Dim ErwarteteErgebnisse = From x In AusXML.SpielRundeFromXML(AktiveListe, XMLPartien, AktuelleCompetition.SpielRegeln.Gewinnsätze) Order By x.GetType.Name Descending

        Assert.AreEqual(AktuellePaarungen.Count, ErwarteteErgebnisse.Count)

        For Each Paar In AktuellePaarungen.Zip(ErwarteteErgebnisse, Function(x, y) New With {.Aktuell = x, .Erwartet = y})
            Assert.AreEqual(Paar.Aktuell.SpielerLinks, Paar.Erwartet.SpielerLinks)
            Assert.AreEqual(Paar.Aktuell.SpielerRechts, Paar.Erwartet.SpielerRechts)
            For Each Satz In Paar.Erwartet
                Paar.Aktuell.Add(Satz)
            Next
        Next

        Dim runde As New SpielRunde

        For Each spielPartie In AktuellePaarungen
            runde.Add(spielPartie)
        Next
        AktuelleCompetition.SpielRunden.Push(runde)
    End Sub

End Class