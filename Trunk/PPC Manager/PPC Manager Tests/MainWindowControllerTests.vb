Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports Moq
Imports System.Windows.Documents
Imports System.Windows

<TestFixture()> Public Class MainWindowControllerTests

    <Test>
    Public Sub NächsteRunde_verpackt_organisierePakete()
        Dim a = New SpielerInfo("A")
        Dim b = New SpielerInfo("B")
        Dim c = New SpielerInfo("C")
        Dim organisierePakete = Function() New PaarungsContainer(Of SpielerInfo) With {
                                    .Partien = New List(Of Tuple(Of SpielerInfo, SpielerInfo)) _
                                        From {Tuple.Create(a, b)},
                                    .Übrig = c
            }
        Dim controller = New MainWindowController(Sub()

                                                  End Sub,
                                         Mock.Of(Of IReportFactory),
                                         organisierePakete,
                                         Mock.Of(Of IFixedPageFabrik))
        Dim result = controller.NächsteRunde("Runde 3")
        Assert.That(result.First.SpielerLinks, [Is].EqualTo(a))
        Assert.That(result.First.SpielerRechts, [Is].EqualTo(b))
        Assert.That(result.Last.SpielerLinks, [Is].EqualTo(c))
        Assert.That(result.Last.GetType(), [Is].EqualTo(GetType(FreiLosSpiel)))
    End Sub

    <Test, Apartment(System.Threading.ApartmentState.STA)>
    Public Sub DruckeRangliste_ruft_Fabrik_mit_Seiteneinstellungen_und_druckt()
        Dim druckFabrik = New Mock(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(Sub()

                                                  End Sub,
                                                  Mock.Of(Of IReportFactory),
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  druckFabrik.Object)
        Dim seiteneinstellung = New SeitenEinstellung
        Controller.DruckeRangliste(seiteneinstellung)
        druckFabrik.Verify(Sub(m) m.ErzeugeRanglisteSeiten(seiteneinstellung), Times.Once)
    End Sub

    <Test, Apartment(System.Threading.ApartmentState.STA)>
    Public Sub DruckeSpielergebnisse_ruft_Fabrik_mit_Seiteneinstellungen_und_druckt()
        Dim druckFabrik = New Mock(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(Sub()

                                                  End Sub,
                                                  Mock.Of(Of IReportFactory),
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  druckFabrik.Object)
        Dim seiteneinstellung = New SeitenEinstellung
        Controller.DruckeSpielergebnisse(seiteneinstellung)
        druckFabrik.Verify(Sub(m) m.ErzeugeSpielErgebnisse(seiteneinstellung), Times.Once)
    End Sub

    <Test>
    Public Sub SaveXML_ruft_Speichern_Delegate()
        Dim aufgerufen = False
        Dim Controller = New MainWindowController(Sub()
                                                      aufgerufen = True
                                                  End Sub,
                                                  Mock.Of(Of IReportFactory),
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  Mock.Of(Of IFixedPageFabrik))
        Controller.SaveXML()
        Assert.That(aufgerufen, [Is].True)
    End Sub

    <Test>
    Public Sub SaveExcel_ruft_ReportFactory()
        Dim reportFactory = New Mock(Of IReportFactory)
        Dim Controller = New MainWindowController(Sub()

                                                  End Sub,
                                                  reportFactory.Object,
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  Mock.Of(Of IFixedPageFabrik))
        Controller.SaveExcel()
        reportFactory.Verify(Sub(m) m.AutoSave(), Times.Once)
    End Sub

    <Test>
    Public Sub DruckeNeuePaarungen_druckt_Paarungen()
        Dim reportFactory = New Mock(Of IReportFactory)
        Dim fixedPage = New Mock(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(Sub()

                                                  End Sub,
                                                  reportFactory.Object,
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  fixedPage.Object)
        Dim seitenEinstellung = New SeitenEinstellung
        Controller.DruckeNeuePaarungen(seitenEinstellung)
        fixedPage.Verify(Sub(m) m.ErzeugePaarungen(seitenEinstellung), Times.Once)
    End Sub

    <Test>
    Public Sub DruckeSchiedsrichterZettel_druckt_Schiedsrichterzettel()
        Dim reportFactory = New Mock(Of IReportFactory)
        Dim fixedPage = New Mock(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(Sub()

                                                  End Sub,
                                                  reportFactory.Object,
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  fixedPage.Object)
        Dim seitenEinstellung = New SeitenEinstellung
        Controller.DruckeSchiedsrichterzettel(seitenEinstellung)
        fixedPage.Verify(Sub(m) m.ErzeugeSchiedsrichterZettelSeiten(seitenEinstellung), Times.Once)
    End Sub

End Class
