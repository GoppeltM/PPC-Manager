Imports System.Text
Imports Moq
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
        _reference = AusXML.CompetitionFromXML("D:\dummy.xml",
                                               xml,
                                               _regeln,
                                               Moq.Mock.Of(Of ISpielverlauf(Of SpielerInfo)),
                                               New SpielRunden)
    End Sub


    Private _reference As Competition
    Private _regeln As SpielRegeln

    <Test>
    Public Sub SpielRegeln_SameAsInit()
        Assert.AreEqual(_regeln, _reference.SpielRegeln)
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
    Public Sub Hat_nullte_und_erste_Spielrunde()
        Assert.AreEqual(2, _reference.SpielRunden.Count)
    End Sub

    <Test>
    Public Sub SpielRunden_Top_Hat1FreilosSpiel()
        Assert.AreEqual(1, _reference.SpielRunden.Peek.OfType(Of FreiLosSpiel).Count)
    End Sub

    <Test>
    Public Sub SpielRunden_Peek_GleichRunde1()
        For Each partie In _reference.SpielRunden.Peek
            Assert.AreEqual("Runde 1", partie.RundenName)
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
    Public Sub Nullte_Runde_hat_einen_ausgeschiedenen_Spieler()
        Assert.That(_reference.SpielRunden.First.AusgeschiedeneSpielerIDs, Contains.Item("PLAYER127"))
    End Sub

    <Test>
    Public Sub NächsteRunde_Execute_OffeneSpieler_SpielDatenUnvollständigException()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        Dim node = (From x In doc.Root.<competition>
                    Where x.Attribute("age-group").Value = "A-Klasse"
                    Select x).Single
        Dim s = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Assert.That(Sub()
                        Dim x = AusXML.CompetitionFromXML("D:\temp.xml", node,
                                              New SpielRegeln(3, True, True),
                                              s,
                                              New SpielRunden)
                    End Sub, Throws.InstanceOf(Of SpielDatenUnvollständigException))

    End Sub

End Class