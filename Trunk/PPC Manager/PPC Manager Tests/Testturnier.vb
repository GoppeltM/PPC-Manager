﻿Imports System.Text
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
        Dim AktuelleCompetition = AusXML.CompetitionFromXML("D:\dummy.xml", _JungenU18, regeln)
        _AktiveListe = AktuelleCompetition.SpielerListe
        AktuelleCompetition.SpielRunden.Clear()
        _Controller = New MainWindowController(AktuelleCompetition, Sub()
                                                                    End Sub, Mock.Of(Of IReportFactory))
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

        _Controller.NächsteRunde_Execute()
        Dim paarungen = _Controller.AktiveCompetition.SpielRunden.First()

        Assert.AreEqual(paarungen.Count, ErwarteteErgebnisse.Count)

        For Each Paar In paarungen.Zip(ErwarteteErgebnisse, Function(x, y) New With {.Aktuell = x, .Erwartet = y})
            Assert.AreEqual(Paar.Aktuell.SpielerLinks, Paar.Erwartet.SpielerLinks)
            Assert.AreEqual(Paar.Aktuell.SpielerRechts, Paar.Erwartet.SpielerRechts)
            For Each satz In Paar.Erwartet
                If satz.GewonnenLinks Then
                    _Controller.SatzEintragen(satz.PunkteRechts, True, Paar.Aktuell)
                End If
                If satz.GewonnenRechts Then
                    _Controller.SatzEintragen(satz.PunkteLinks, False, Paar.Aktuell)
                End If
            Next
        Next

    End Sub

End Class