Imports System.Text
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports System.Windows.Controls
Imports System.Printing
Imports Moq
Imports System.Windows.Documents

<TestFixture()> Public Class MainWindowControllerTests

    <Test>
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

    <Test, Explicit, RequiresSTA>
    Public Sub UIDummy_RundenEndeDrucken_2Seiten()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        For Each Spieler In doc.Root...<person>
            Spieler.@ppc:anwesend = "true"
        Next

        Dim c = Competition.FromXML("D:\temp.xml", doc, "D-Klasse", New SpielRegeln(3, True, True))
        Dim Controller = New MainWindowController(c)
        Dim window = New Windows.Window
        window.Show()
        Controller.RundenendeDrucken(New Printer)
        window.Close()
    End Sub

    <Test, RequiresSTA>
    Public Sub RundenendeDrucken_DKlasse_2Seiten()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        For Each Spieler In doc.Root...<person>
            Spieler.@ppc:anwesend = "true"
        Next

        Dim c = Competition.FromXML("D:\temp.xml", doc, "D-Klasse", New SpielRegeln(3, True, True))
        Dim Controller = New MainWindowController(c)
        Dim DruckenMock = New Mock(Of IPrinter)
        Dim a = Sub(d As DocumentPaginator, s As String)
                    Assert.AreEqual(2, d.PageCount)
                End Sub
        DruckenMock.Setup(Sub(m) m.PrintDocument(It.IsAny(Of DocumentPaginator), It.IsAny(Of String))).Callback(a).Verifiable()
        DruckenMock.Setup(Function(m) m.PrintableAreaHeight).Returns(1000.0)
        DruckenMock.Setup(Function(m) m.PrintableAreaWidth).Returns(800.0)
        Controller.RundenendeDrucken(DruckenMock.Object)
        DruckenMock.VerifyAll()
    End Sub


End Class
