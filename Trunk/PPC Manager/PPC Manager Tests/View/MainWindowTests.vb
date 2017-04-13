Public Class MainWindowTests
    <Test, Explicit, Apartment(Threading.ApartmentState.STA)>
    Sub MainWindowUIDummy()
        Dim controller = New Moq.Mock(Of IController)
        Dim window = New MainWindow(controller.Object, New SpielerListe, New SpielRunden, "Hallo Welt")
        window.ShowDialog()
    End Sub
End Class
