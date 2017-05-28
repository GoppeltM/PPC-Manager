Imports System.Globalization

Public Class PrintDialogZuStringConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim p = TryCast(value, PrintDialog)
        If p Is Nothing Then Return "Keine Einstellung"
        Dim format As String
        Select Case p.PrintTicket.PageOrientation.Value
            Case Printing.PageOrientation.Landscape
                format = "Querformat"
            Case Printing.PageOrientation.Portrait
                format = "Hochformat"
            Case Else
                format = "Unbekannt"
        End Select
        Return String.Format("Drucker: {0} - Papier: {1} ({2})", p.PrintQueue.Name, p.PrintTicket.PageMediaSize.PageMediaSizeName, format)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
