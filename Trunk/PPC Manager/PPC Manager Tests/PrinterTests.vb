Imports System.Windows

Public Class PrinterTests

    <Test, Explicit, Apartment(System.Threading.ApartmentState.STA)>
    Public Sub DruckerEinstellungen()
        Dim w As New Window
        w.Show()
        Dim p As New Printer(New Controls.PrintDialog)
        Dim result = p.LeseKonfiguration()
    End Sub
End Class
