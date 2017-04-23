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


        Dim druckFabrik = Mock.Of(Of IFixedPageFabrik)
        Dim ControllerA = New MainWindowController(c.SpielerListe, c.SpielRunden, Sub()

                                                                                  End Sub,
                                                   Mock.Of(Of IReportFactory),
                                                   Function() New PaarungsContainer(Of SpielerInfo),
                                                   druckFabrik, 3)
        Dim ControllerD = New MainWindowController(cD.SpielerListe, cD.SpielRunden, Sub()

                                                                                    End Sub,
                                                   Mock.Of(Of IReportFactory),
                                                   Function() New PaarungsContainer(Of SpielerInfo),
                                                   druckFabrik, 3)

        ControllerA.NächsteRunde()
        For Each partie In c.SpielRunden.Last
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
        Next
        ControllerD.NächsteRunde()
        For Each partie In cD.SpielRunden.Last
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz() With {.PunkteLinks = 11, .PunkteRechts = 0})
        Next
        ControllerA.NächsteRunde()
        ControllerD.NächsteRunde()
        cD.SpielRunden.Pop()

        Assert.AreEqual(2, c.SpielRunden.Count)
        Assert.AreEqual(1, cD.SpielRunden.Count)

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
        Dim druckFabrik = Mock.Of(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(c.SpielerListe, c.SpielRunden, Sub()

                                                                                 End Sub,
                                                  Mock.Of(Of IReportFactory),
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  druckFabrik, 3)
        Dim window = New Window
        window.Show()
        Controller.RundenendeDrucken(New Printer)
        window.Close()
    End Sub

    <Test, Apartment(System.Threading.ApartmentState.STA)>
    Public Sub RundenendeDrucken_druckt_Ranglisten_und_Spielergebnisse()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        For Each Spieler In doc.Root...<person>
            Spieler.@ppc:anwesend = "true"
        Next
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim c = AusXML.CompetitionFromXML("D:\temp.xml",
                                          doc,
                                          "D-Klasse",
                                          New SpielRegeln(3, True, True), spielverlauf, New SpielRunden)
        Dim druckFabrik = New Mock(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(c.SpielerListe, c.SpielRunden, Sub()

                                                                                 End Sub,
                                                  Mock.Of(Of IReportFactory),
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  druckFabrik.Object, 3)
        Dim DruckenMock = New Mock(Of IPrinter)
        Controller.RundenendeDrucken(DruckenMock.Object)
        druckFabrik.Verify(Sub(m) m.ErzeugeRanglisteSeiten(It.IsAny(Of ISeiteneinstellung)), Times.Once)
        druckFabrik.Verify(Sub(m) m.ErzeugeSpielErgebnisse(It.IsAny(Of ISeiteneinstellung)), Times.Once)
        DruckenMock.Verify(Sub(m) m.Drucken(It.IsAny(Of FixedDocument), "Rundenende - Aushang und Rangliste"))
    End Sub


End Class
