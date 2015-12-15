Imports System.Windows

Public Class PrinterTests

    <Test, Explicit, STAThread>
    Public Sub DruckerEinstellungen()
        Dim w As New Window
        w.Show()
        Dim p As New Printer
        Dim result = p.Konfigurieren()
    End Sub
End Class
