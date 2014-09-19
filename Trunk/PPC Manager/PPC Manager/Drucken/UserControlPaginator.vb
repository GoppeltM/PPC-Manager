Public Interface IPaginatibleUserControl

    Sub SetSource(startIndex As Integer, ByVal elements As IEnumerable(Of Object))
    Function GetMaxItemCount() As Integer
    Property PageNumber As Integer

End Interface

Public Class UserControlPaginator(Of T As {IPaginatibleUserControl, UserControl})
    Inherits DocumentPaginator

    Private ReadOnly _begegnungen As IEnumerable(Of Object)
    Private ReadOnly _factory As Func(Of T)

    Public Sub New(ByVal begegnungen As IEnumerable(Of Object), ByVal pageSize As Size, factory As Func(Of T))
        _begegnungen = begegnungen
        Me.PageSize = pageSize
        _factory = factory
        EmptyPage = CreateVisual(0, New Object() {}, 1)
    End Sub


    Private ReadOnly EmptyPage As T


    Public Overrides Function GetPage(ByVal pageNumber As Integer) As System.Windows.Documents.DocumentPage
        Dim start = pageNumber * ElementsPerPage
        Dim currentElements = _begegnungen.Skip(start).Take(ElementsPerPage).ToList
        Dim page = CreateVisual(start, currentElements, pageNumber)
        Dim visibleArea = New Rect(PageSize)
        Dim doc = New DocumentPage(page, PageSize, visibleArea, visibleArea)
        Return doc
    End Function

    Public Function CreateVisual(startIndex As Integer, ByVal elements As IEnumerable(Of Object), pageNumber As Integer) As T
        Dim visual = _factory()
        visual.SetSource(startIndex, elements)
        visual.PageNumber = pageNumber
        Dim page As New FixedPage
        page.Width = PageSize.Width
        page.Height = PageSize.Height
        visual.Width = page.Width
        visual.Height = page.Height
        page.Children.Add(visual)
        page.Measure(PageSize)
        page.Arrange(New Rect(New Point(0, 0), PageSize))
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
