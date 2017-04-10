Public Class MainWindowTests
    <Test, Explicit, Apartment(Threading.ApartmentState.STA)>
    Sub MainWindowUIDummy()
        Dim controller = New Moq.Mock(Of IController)
        Dim c As New Competition(New SpielRegeln(3, True, True), New SpielRunden, New SpielerListe, "B-Klasse")
        controller.Setup(Function(m) m.AktiveCompetition).Returns(c)
        Dim window = New MainWindow(controller.Object)
        window.ShowDialog()
    End Sub
End Class
