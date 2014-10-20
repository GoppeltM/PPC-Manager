Public Interface IPrinter

    ReadOnly Property PrintableAreaWidth As Double

    ReadOnly Property PrintableAreaHeight As Double

    Sub PrintDocument(paginator As DocumentPaginator, p2 As String)

End Interface

Public Class Printer
    Implements IPrinter

    Private ReadOnly _PrintDialog As PrintDialog
    Private ReadOnly _Height As Double
    Private ReadOnly _Width As Double

    Private _Sicherung As Boolean?

    Public Sub New()
        _PrintDialog = New PrintDialog
        _PrintDialog.UserPageRangeEnabled = True
        _Sicherung = _PrintDialog.ShowDialog
        _Height = _PrintDialog.PrintableAreaHeight
        _Width = _PrintDialog.PrintableAreaWidth
    End Sub

    Public ReadOnly Property PrintableAreaHeight As Double Implements IPrinter.PrintableAreaHeight
        Get
            Return _Height
        End Get
    End Property

    Public ReadOnly Property PrintableAreaWidth As Double Implements IPrinter.PrintableAreaWidth
        Get
            Return _Width
        End Get
    End Property

    Public Sub PrintDocument(paginatingPaginator As DocumentPaginator, beschreibung As String) Implements IPrinter.PrintDocument
        If _Sicherung <> True Then Return
        _PrintDialog.PrintDocument(paginatingPaginator, beschreibung)
    End Sub

End Class
