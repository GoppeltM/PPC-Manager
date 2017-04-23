Imports System.Windows

<Apartment(System.Threading.ApartmentState.STA), Explicit>
Public Class Sandbox

    <Test>
    Public Sub Foo()
        Dim x = New Test
        x.DataContext = New List(Of String) From {"Marius", "Goppelt"}
        Dim w = New Window
        w.Content = x
        w.ShowDialog()
    End Sub
End Class
