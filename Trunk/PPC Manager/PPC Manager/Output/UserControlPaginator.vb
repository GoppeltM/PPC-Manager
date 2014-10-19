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
        EmptyPage = ErzeugeFixedPagedUserControl(0, New Object() {}, 1)
    End Sub


    Private ReadOnly EmptyPage As T


    Public Overrides Function GetPage(ByVal pageNumber As Integer) As DocumentPage
        Return GetUserControlPage(pageNumber).Item2
    End Function

    Public Function GetUserControlPage(pageNumber As Integer) As Tuple(Of T, DocumentPage)
        Dim start = pageNumber * ElementsPerPage
        Dim currentElements = _begegnungen.Skip(start).Take(ElementsPerPage).ToList
        Dim page = ErzeugeFixedPagedUserControl(start, currentElements, pageNumber)
        Dim visibleArea = New Rect(PageSize)
        Dim doc = New DocumentPage(page, PageSize, visibleArea, visibleArea)        
        Return Tuple.Create(page, doc)
    End Function

    Private Function ErzeugeFixedPagedUserControl(startIndex As Integer, ByVal elements As IEnumerable(Of Object), pageNumber As Integer) As T
        Dim UserControl = _factory()
        UserControl.SetSource(startIndex, elements)
        UserControl.PageNumber = pageNumber
        Dim Page As New FixedPage
        Page.Width = PageSize.Width
        Page.Height = PageSize.Height
        UserControl.Width = Page.Width
        UserControl.Height = Page.Height
        Page.Children.Add(UserControl)
        Page.Measure(PageSize)
        Page.Arrange(New Rect(New Point(0, 0), PageSize))
        Page.UpdateLayout()
        Return UserControl
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
