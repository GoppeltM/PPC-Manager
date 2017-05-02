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
    Public Sub RundenendeDrucken_druckt_Ranglisten_und_Spielergebnisse()
        Dim druckFabrik = New Mock(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(Sub()

                                                  End Sub,
                                                  Mock.Of(Of IReportFactory),
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  druckFabrik.Object)
        Dim DruckenMock = New Mock(Of IPrinter)
        Controller.RundenendeDrucken(DruckenMock.Object)
        druckFabrik.Verify(Sub(m) m.ErzeugeRanglisteSeiten(It.IsAny(Of ISeiteneinstellung)), Times.Once)
        druckFabrik.Verify(Sub(m) m.ErzeugeSpielErgebnisse(It.IsAny(Of ISeiteneinstellung)), Times.Once)
        DruckenMock.Verify(Sub(m) m.Drucken(It.IsAny(Of FixedDocument), "Rundenende - Aushang und Rangliste"))
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
    Public Sub RundenbeginnDrucken_druckt_Paarungen_und_SchiriZettel()
        Dim reportFactory = New Mock(Of IReportFactory)
        Dim fixedPage = New Mock(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(Sub()

                                                  End Sub,
                                                  reportFactory.Object,
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  fixedPage.Object)
        Dim seitenEinstellung = Mock.Of(Of ISeiteneinstellung)
        Dim drucker = Mock.Of(Of IPrinter)(Function(m) m.Konfigurieren Is seitenEinstellung)
        Controller.RundenbeginnDrucken(drucker)
        fixedPage.Verify(Sub(m) m.ErzeugePaarungen(seitenEinstellung), Times.Once)
        fixedPage.Verify(Sub(m) m.ErzeugeSchiedsrichterZettelSeiten(seitenEinstellung), Times.Once)
    End Sub

    <Test>
    Public Sub RundenEndeDrucken_druckt_Rangliste_und_Ergebnisse()
        Dim reportFactory = New Mock(Of IReportFactory)
        Dim fixedPage = New Mock(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(Sub()

                                                  End Sub,
                                                  reportFactory.Object,
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  fixedPage.Object)
        Dim seitenEinstellung = Mock.Of(Of ISeiteneinstellung)
        Dim drucker = Mock.Of(Of IPrinter)(Function(m) m.Konfigurieren Is seitenEinstellung)
        Controller.RundenendeDrucken(drucker)
        fixedPage.Verify(Sub(m) m.ErzeugeSpielErgebnisse(seitenEinstellung), Times.Once)
        fixedPage.Verify(Sub(m) m.ErzeugeRanglisteSeiten(seitenEinstellung), Times.Once)
    End Sub

End Class
