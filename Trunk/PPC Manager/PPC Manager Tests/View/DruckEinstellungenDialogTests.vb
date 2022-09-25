<Explicit, Apartment(Threading.ApartmentState.STA)>
Public Class DruckEinstellungenDialogTests

    <Test>
    Public Sub DruckEinstellungen_Dummy()
        Dim einstellungen As New DruckEinstellungen
        einstellungen.DruckeNeuePaarungen = True
        einstellungen.DruckeSpielergebnisse = True
        Dim druckFabrik = New DruckerFabrik()
        Dim pageFabrik = Mock.Of(Of IFixedPageFabrik)
        Dim Controller = New MainWindowController(Sub()

                                                  End Sub,
                                                  Mock.Of(Of IReportFactory),
                                                  Function() New PaarungsContainer(Of SpielerInfo),
                                                  pageFabrik)
        Dim d = New DruckEinstellungenDialog(Controller, druckFabrik)
        d.DataContext = einstellungen
        Dim result = d.ShowDialog()
    End Sub
End Class
