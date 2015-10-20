Imports PPC_Manager

Public Interface IPrinter

    Function Konfigurieren() As Size

    Sub Drucken(doc As FixedDocument, titel As String)

End Interface

Public Class Printer
    Implements IPrinter

    Private ReadOnly _PrintDialog As PrintDialog
    Private _DialogResult As Boolean?
    Public Sub New()
        _PrintDialog = New PrintDialog
        _PrintDialog.UserPageRangeEnabled = False
    End Sub

    Public Function Konfigurieren() As Size Implements IPrinter.Konfigurieren
        _DialogResult = _PrintDialog.ShowDialog()
        Return New Size(_PrintDialog.PrintableAreaWidth, _PrintDialog.PrintableAreaHeight)
    End Function

    Public Sub Drucken(doc As FixedDocument, titel As String) Implements IPrinter.Drucken
        If _DialogResult <> True Then Return
        doc.PrintTicket = _PrintDialog.PrintTicket
        _PrintDialog.PrintDocument(doc.DocumentPaginator, titel)
    End Sub
End Class
