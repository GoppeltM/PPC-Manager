Public Class PaginatingPaginator
    Inherits DocumentPaginator

    Private _paginators As IEnumerable(Of DocumentPaginator)
   
    Public Sub New(ByVal paginators As IEnumerable(Of DocumentPaginator))
        _paginators = paginators        
    End Sub

    Public Overrides Function GetPage(pageNumber As Integer) As DocumentPage

        Dim current = 0
        For Each pag In _paginators
            If pag.PageCount > pageNumber - current Then
                Return pag.GetPage(pageNumber - current)
            End If
            current += pag.PageCount
        Next
        Throw New IndexOutOfRangeException
    End Function

    Public Overrides ReadOnly Property IsPageCountValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property PageCount As Integer
        Get
            Dim C = Aggregate x In _paginators Into Sum(x.PageCount)

            Return C
        End Get
    End Property

    Public Overrides Property PageSize As Size

    Public Overrides ReadOnly Property Source As IDocumentPaginatorSource
        Get
            Return Nothing
        End Get
    End Property
End Class
