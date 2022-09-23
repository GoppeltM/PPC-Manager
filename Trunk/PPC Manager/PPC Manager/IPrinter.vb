Imports PPC_Manager

Public Interface IPrinter

    Function LeseKonfiguration() As SeitenEinstellung

    Sub Drucken(doc As FixedDocument, titel As String)

End Interface

Public Class SeitenEinstellung

    Public Property AbstandX As Double
    Public Property AbstandY As Double
    Public Property Breite As Double
    Public Property Höhe As Double

End Class

Public Class Printer
    Implements IPrinter

    Private ReadOnly _PrintDialog As PrintDialog
    Public Sub New(printDialog As PrintDialog)
        _PrintDialog = printDialog
    End Sub

    Public Function LeseKonfiguration() As SeitenEinstellung Implements IPrinter.LeseKonfiguration
        Dim area = _PrintDialog.PrintQueue.GetPrintCapabilities.PageImageableArea
        Return New SeitenEinstellung() With {
            .AbstandX = area.OriginWidth,
            .AbstandY = area.OriginHeight,
            .Breite = area.ExtentWidth,
            .Höhe = area.ExtentHeight
        }
    End Function

    Public Sub Drucken(doc As FixedDocument, titel As String) Implements IPrinter.Drucken
        doc.PrintTicket = _PrintDialog.PrintTicket
        If doc.Pages.Any Then
            _PrintDialog.PrintDocument(doc.DocumentPaginator, titel)
        End If
    End Sub
End Class
