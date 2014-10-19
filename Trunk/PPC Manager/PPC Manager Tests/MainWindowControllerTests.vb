Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports System.Windows.Controls
Imports System.Printing

<TestClass()> Public Class MainWindowControllerTests

    <TestMethod()>
    Public Sub ZweiController_RundeUndo_Controller2geändert()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)

        For Each Spieler In doc.Root...<person>
            Spieler.@ppc:anwesend = "true"
        Next

        Dim c = Competition.FromXML("D:\temp.xml", doc, "A-Klasse", New SpielRegeln(3, True, True))
        Dim cD = Competition.FromXML("D:\temp.xml", doc, "D-Klasse", New SpielRegeln(3, True, True))

        Dim ControllerA = New MainWindowController(c)
        Dim ControllerD = New MainWindowController(cD)

        ControllerA.NächsteRunde_Execute()
        For Each partie In ControllerA.AktiveCompetition.SpielRunden.Last
            ControllerA.SatzEintragen(11, True, partie)
            ControllerA.SatzEintragen(11, True, partie)
            ControllerA.SatzEintragen(11, True, partie)
        Next
        ControllerD.NächsteRunde_Execute()
        For Each partie In ControllerD.AktiveCompetition.SpielRunden.Last
            ControllerD.SatzEintragen(11, True, partie)
            ControllerD.SatzEintragen(11, True, partie)
            ControllerD.SatzEintragen(11, True, partie)
        Next
        ControllerA.NächsteRunde_Execute()
        ControllerD.NächsteRunde_Execute()
        ControllerD.RundeVerwerfen()

        Assert.AreEqual(2, ControllerA.AktiveCompetition.SpielRunden.Count)
        Assert.AreEqual(1, ControllerD.AktiveCompetition.SpielRunden.Count)

    End Sub

    <TestMethod>
    Public Sub DruckenUIDummy()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        For Each Spieler In doc.Root...<person>
            Spieler.@ppc:anwesend = "true"
        Next

        Dim c = Competition.FromXML("D:\temp.xml", doc, "D-Klasse", New SpielRegeln(3, True, True))
        Dim Controller = New MainWindowController(c)
        Dim p = New PrintDialog()
        p.PrintQueue = New PrintQueue(New PrintServer(), "Microsoft XPS Document Writer")        
        Controller.RundenendeDrucken(p)
    End Sub


End Class
