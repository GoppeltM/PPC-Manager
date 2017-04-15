Imports PPC_Manager

Public Interface IPrinter

    Function Konfigurieren() As ISeiteneinstellung

    Sub Drucken(doc As FixedDocument, titel As String)

End Interface

Public Interface ISeiteneinstellung
    Property Höhe As Double
    Property Breite As Double
    Property AbstandX As Double
    Property AbstandY As Double
End Interface

Public Class SeitenEinstellung
    Implements ISeiteneinstellung

    Public Property AbstandX As Double Implements ISeiteneinstellung.AbstandX
    Public Property AbstandY As Double Implements ISeiteneinstellung.AbstandY
    Public Property Breite As Double Implements ISeiteneinstellung.Breite
    Public Property Höhe As Double Implements ISeiteneinstellung.Höhe

End Class

Public Class Printer
    Implements IPrinter

    Private ReadOnly _PrintDialog As PrintDialog
    Private _DialogResult As Boolean?
    Public Sub New()
        _PrintDialog = New PrintDialog
        _PrintDialog.UserPageRangeEnabled = False
    End Sub

    Public Function Konfigurieren() As ISeiteneinstellung Implements IPrinter.Konfigurieren
        _DialogResult = _PrintDialog.ShowDialog()
        Dim area = _PrintDialog.PrintQueue.GetPrintCapabilities.PageImageableArea
        Dim einstellung = New SeitenEinstellung() With {
            .AbstandX = area.OriginWidth,
            .AbstandY = area.OriginHeight,
            .Breite = area.ExtentWidth,
            .Höhe = area.ExtentHeight}

        Return einstellung
    End Function

    Public Sub Drucken(doc As FixedDocument, titel As String) Implements IPrinter.Drucken
        If _DialogResult <> True Then Return
        doc.PrintTicket = _PrintDialog.PrintTicket
        _PrintDialog.PrintDocument(doc.DocumentPaginator, titel)
    End Sub
End Class
