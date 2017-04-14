Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports Moq
Imports System.Windows.Documents
Imports System.Windows

<TestFixture()> Public Class MainWindowControllerTests

    <Test>
    Public Sub ZweiController_RundeUndo_Controller2geändert()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)

        For Each Spieler In doc.Root...<person>
            Spieler.@ppc:anwesend = "true"
        Next
        Dim spielregeln = New SpielRegeln(3, True, True)
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim c = AusXML.CompetitionFromXML("D:\temp.xml", doc, "A-Klasse", spielregeln, spielverlauf, New SpielRunden)
        Dim cD = AusXML.CompetitionFromXML("D:\temp.xml", doc, "D-Klasse", spielregeln, spielverlauf, New SpielRunden)

        Dim ControllerA = New MainWindowController(c.SpielRunden, c, Sub()

                                                                     End Sub, Mock.Of(Of IReportFactory), Function() New PaarungsContainer(Of SpielerInfo))
        Dim ControllerD = New MainWindowController(cD.SpielRunden, cD, Sub()

                                                                       End Sub, Mock.Of(Of IReportFactory), Function() New PaarungsContainer(Of SpielerInfo))

        ControllerA.NächsteRunde()
        For Each partie In ControllerA.AktiveCompetition.SpielRunden.Last
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
        Next
        ControllerD.NächsteRunde()
        For Each partie In ControllerD.AktiveCompetition.SpielRunden.Last
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
        Next
        ControllerA.NächsteRunde()
        ControllerD.NächsteRunde()
        ControllerD.RundeVerwerfen()

        Assert.AreEqual(2, ControllerA.AktiveCompetition.SpielRunden.Count)
        Assert.AreEqual(1, ControllerD.AktiveCompetition.SpielRunden.Count)

    End Sub

    <Test, Explicit, Apartment(System.Threading.ApartmentState.STA)>
    Public Sub UIDummy_RundenEndeDrucken_2Seiten()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        For Each Spieler In doc.Root...<person>
            Spieler.@ppc:anwesend = "true"
        Next
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim c = AusXML.CompetitionFromXML("D:\temp.xml",
                                          doc,
                                          "D-Klasse",
                                          New SpielRegeln(3, True, True), spielverlauf, New SpielRunden)
        Dim Controller = New MainWindowController(c.SpielRunden, c, Sub()

                                                                    End Sub, Mock.Of(Of IReportFactory), Function() New PaarungsContainer(Of SpielerInfo))
        Dim window = New Window
        window.Show()
        Controller.RundenendeDrucken(New Printer)
        window.Close()
    End Sub

    <Test, Apartment(System.Threading.ApartmentState.STA)>
    Public Sub RundenendeDrucken_DKlasse_2Seiten()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        For Each Spieler In doc.Root...<person>
            Spieler.@ppc:anwesend = "true"
        Next
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim c = AusXML.CompetitionFromXML("D:\temp.xml",
                                          doc,
                                          "D-Klasse",
                                          New SpielRegeln(3, True, True), spielverlauf, New SpielRunden)
        Dim Controller = New MainWindowController(c.SpielRunden, c, Sub()

                                                                    End Sub, Mock.Of(Of IReportFactory), Function() New PaarungsContainer(Of SpielerInfo))
        Dim DruckenMock = New Mock(Of IPrinter)
        Dim a = Sub(d As FixedDocument, s As String)
                    Assert.AreEqual(2, d.DocumentPaginator.PageCount)
                End Sub
        Dim einstellungenMock = New Mock(Of ISeiteneinstellung)()
        einstellungenMock.SetupAllProperties()
        einstellungenMock.Object.Breite = 800
        einstellungenMock.Object.Höhe = 1000
        DruckenMock.Setup(Function(m) m.Konfigurieren()).Returns(einstellungenMock.Object)
        DruckenMock.Setup(Sub(m) m.Drucken(It.IsAny(Of FixedDocument), It.IsAny(Of String))).Callback(a).Verifiable()

        Controller.RundenendeDrucken(DruckenMock.Object)
        DruckenMock.VerifyAll()
    End Sub


End Class
