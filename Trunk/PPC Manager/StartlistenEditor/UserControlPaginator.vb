Public Interface IPaginatibleUserControl

    Sub SetSource(ByVal elements As IEnumerable(Of Object))
    Function GetMaxItemCount() As Integer

End Interface

Public Class UserControlPaginator(Of T As {IPaginatibleUserControl, New, UserControl})
    Inherits DocumentPaginator

    Dim _begegnungen As IEnumerable(Of Object)
    Dim _pageSize As Size

    Public Sub New(ByVal begegnungen As IEnumerable(Of Object), ByVal pageSize As Size)
        _begegnungen = begegnungen
        _pageSize = pageSize
        EmptyPage = CreateVisual(New Object() {})
    End Sub


    Private ReadOnly EmptyPage As T


    Public Overrides Function GetPage(ByVal pageNumber As Integer) As System.Windows.Documents.DocumentPage
        Dim start = pageNumber * ElementsPerPage
        Dim currentElements = _begegnungen.Skip(start).Take(ElementsPerPage).ToList
        Dim page = CreateVisual(currentElements)
        Dim visibleArea = New Rect(_pageSize)
        Dim doc = New DocumentPage(page, _pageSize, visibleArea, visibleArea)
        Return doc
    End Function

    Public Function CreateVisual(ByVal elements As IEnumerable(Of Object)) As T
        Dim visual = New T()
        visual.SetSource(elements)
        Dim page As New FixedPage
        page.Width = _pageSize.Width
        page.Height = _pageSize.Height
        visual.Width = page.Width
        visual.Height = page.Height
        page.Children.Add(visual)
        page.Measure(_pageSize)
        page.Arrange(New Rect(New Point(0, 0), _pageSize))
        page.UpdateLayout()
        Return visual
    End Function


    Public Overrides ReadOnly Property IsPageCountValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property PageCount As Integer
        Get
            Return CInt(Math.Ceiling(_begegnungen.Count / ElementsPerPage))
        End Get
    End Property

    Public ReadOnly Property ElementsPerPage As Integer
        Get
            Return (EmptyPage.GetMaxItemCount)
        End Get
    End Property

    Public Overrides Property PageSize As System.Windows.Size
        Get
            Return _pageSize
        End Get
        Set(ByVal value As System.Windows.Size)
            _pageSize = value
        End Set
    End Property

    Public Overrides ReadOnly Property Source As IDocumentPaginatorSource
        Get
            'Dim doc As New FixedDocument
            'For i = 0 To PageCount - 1
            '    Dim content As New PageContent
            '    Dim page As New FixedPage
            '    page.Children.Add(GetPage(i).Visual)
            '    content.Child = page                
            '    doc.Pages.Add(content)
            'Next
            'Return doc
            Return Nothing

        End Get
    End Property
End Class
