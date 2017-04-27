Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports Moq
Imports System.Windows.Documents
Imports System.Windows

<TestFixture()> Public Class MainWindowControllerTests

    <Test>
    Public Sub NächsteRunde()

    End Sub

    <Test, Apartment(System.Threading.ApartmentState.STA)>
    Public Sub RundenendeDrucken_druckt_Ranglisten_und_Spielergebnisse()
        Dim druckFabrik = New Mock(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(Sub()

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
