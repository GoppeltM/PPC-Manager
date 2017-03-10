Imports System.Text
Imports NUnit.Framework
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

<TestFixture()> Public Class CompetitionTests_FromXML

    <SetUp>
    Public Sub Init()
        _regeln = New SpielRegeln(4, False, False)
        Dim xml = XDocument.Parse(My.Resources.Competition).Root.<competition>.First
        For Each Spieler In xml...<person>
            Spieler.@ppc:anwesend = "true"
        Next
        _reference = AusXML.CompetitionFromXML("D:\dummy.xml", xml, _regeln)
    End Sub


    <Test, Explicit, Apartment(Threading.ApartmentState.STA)>
    Sub MainWindowUIDummy()
        Dim controller = New Moq.Mock(Of IController)
        controller.Setup(Function(m) m.AktiveCompetition).Returns(_reference)
        controller.Setup(Function(m) m.FilterSpieler(Moq.It.IsAny(Of Spieler))).Returns(True)
        Dim window = New MainWindow(controller.Object)
        window.ShowDialog()
    End Sub

    Private _reference As Competition
    Private _regeln As SpielRegeln

    <Test>
    Public Sub DateiPfad_Is_dummyXML()
        Assert.AreEqual("D:\dummy.xml", _reference.DateiPfad)
    End Sub

    <Test>
    Public Sub SpielRegeln_SameAsInit()
        Assert.AreEqual(_regeln, _reference.SpielRegeln)
    End Sub

    <Test>
    Public Sub StartDatum_Gleich_XMLDatum()
        Assert.AreEqual("2012-09-08 11:00", _reference.StartDatum)
    End Sub

    <Test>
    Public Sub AltersGruppe_ErsteGleich_MädchenU13()
        Assert.AreEqual("Mädchen U 13", _reference.Altersgruppe)
    End Sub

    <Test>
    Public Sub SpielerListe_Count_4()
        Assert.AreEqual(4, _reference.SpielerListe.Count)
    End Sub

    <Test>
    Public Sub SpielRunden_Count_2()
        Assert.AreEqual(2, _reference.SpielRunden.Count)
    End Sub

    <Test>
    Public Sub SpielRunden_Top_Hat1FreilosSpiel()
        Assert.AreEqual(1, _reference.SpielRunden.Peek.OfType(Of FreiLosSpiel).Count)
    End Sub

    <Test>
    Public Sub SpielRunden_Peek_GleichRunde2()
        For Each partie In _reference.SpielRunden.Peek
            Assert.AreEqual("Runde 2", partie.RundenName)
        Next
    End Sub

    <Test>
    Public Sub SpielRunden_Last_GleichRunde1()
        For Each partie In _reference.SpielRunden.Last
            Assert.AreEqual("Runde 1", partie.RundenName)
        Next
    End Sub

    <Test>
    Public Sub SpielerListe_IDs_NachgemeldetZuletzt()
        CollectionAssert.AreEqual({"PLAYER72", "PLAYER126", "PLAYER127", "PLAYER-1"}, (From x In _reference.SpielerListe Select x.Id).ToList)
    End Sub

    <Test>
    Public Sub SpielRunden_AusgeschiedeneSpieler_HatPlayer127()
        CollectionAssert.AreEqual({"PLAYER127"}, (From x In _reference.SpielRunden.AusgeschiedeneSpieler Select x.Spieler.Id).ToList)
    End Sub

    <Test>
    Public Sub NächsteRunde_Execute_OffeneSpieler_SpielDatenUnvollständigException()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        Dim node = (From x In doc.Root.<competition>
                   Where x.Attribute("age-group").Value = "A-Klasse"
                   Select x).Single
        Try
            Dim c = AusXML.CompetitionFromXML("D:\temp.xml", node, New SpielRegeln(3, True, True))
            Assert.Fail()
        Catch ex As SpielDatenUnvollständigException

        End Try
    End Sub

End Class