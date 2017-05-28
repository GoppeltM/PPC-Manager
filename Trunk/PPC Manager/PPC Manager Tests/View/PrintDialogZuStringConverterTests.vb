Imports System.Windows
Imports System.Windows.Controls

Public Class PrintDialogZuStringConverterTests
    <Test, Explicit, Apartment(System.Threading.ApartmentState.STA)>
    Public Sub Convert_Dummy()
        Dim x = New PrintDialogZuStringConverter
        Dim w = New Window
        w.Show()
        Dim dialog = New PrintDialog
        dialog.ShowDialog()
        Dim result = x.Convert(dialog, Nothing, Nothing, Nothing)

    End Sub
End Class
