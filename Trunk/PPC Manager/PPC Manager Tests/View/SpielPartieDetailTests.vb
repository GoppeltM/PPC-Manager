Imports System.Windows

<Apartment(System.Threading.ApartmentState.STA), Explicit>
Public Class SpielPartieDetailTests

    <Test>
    Public Sub UIDummy()
        Dim x = New SpielPartieDetail()
        x.DataContext = New SpielPartie("Runde 1", New SpielerInfo("1"), New SpielerInfo("2"))
        Dim w = New Window
        w.Content = x
        w.ShowDialog()
    End Sub
End Class
