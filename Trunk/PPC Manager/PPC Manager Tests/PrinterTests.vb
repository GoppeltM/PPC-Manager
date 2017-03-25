Imports System.Windows

Public Class PrinterTests

    <Test, Explicit, Apartment(System.Threading.ApartmentState.STA)>
    Public Sub DruckerEinstellungen()
        Dim w As New Window
        w.Show()
        Dim p As New Printer
        Dim result = p.Konfigurieren()
    End Sub
End Class
