Public Interface IPaginatibleUserControl

    Sub SetSource(startIndex As Integer, ByVal elements As IEnumerable(Of Object))
    Function GetMaxItemCount() As Integer
    Property PageNumber As Integer

End Interface

Public Class UserControlPaginator(Of T As {IPaginatibleUserControl, UserControl})
    Inherits DocumentPaginator

    Private ReadOnly _Elemente As List(Of Object)
    Private ReadOnly _factory As Func(Of T)

    Public Sub New(ByVal elemente As IEnumerable(Of Object), ByVal pageSize As Size, factory As Func(Of T))
        _Elemente = elemente.ToList
        Me.PageSize = pageSize
        _factory = factory
        Dim EmptyPage = ErzeugeFixedPagedUserControl(0, New Object() {}, 1)
        Dim ElementsPerPage = EmptyPage.GetMaxItemCount
        _Pages = New List(Of T)
        Dim start = 0
        While elemente.Any
            Dim currentElements = elemente.Take(ElementsPerPage).ToList
            Dim page = ErzeugeFixedPagedUserControl(start, currentElements, _Pages.Count)
            _Pages.Add(page)

            elemente = elemente.Skip(ElementsPerPage).ToList
            start += currentElements.Count
        End While
    End Sub

    Private ReadOnly _Pages As List(Of T)

    Public ReadOnly Property Pages As IEnumerable(Of T)
        Get
            Return _Pages
        End Get
    End Property

    Public Overrides Function GetPage(ByVal pageNumber As Integer) As DocumentPage
        Dim visibleArea = New Rect(PageSize)
        Dim page = GetUserControlPage(pageNumber)
        Dim doc = New DocumentPage(Page, PageSize, visibleArea, visibleArea)
        Return doc
    End Function

    Public Function GetUserControlPage(pageNumber As Integer) As T
        Return _Pages.Item(pageNumber)
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
            Return _Pages.Count > 0
        End Get
    End Property

    Public Overrides ReadOnly Property PageCount As Integer
        Get
            Return _Pages.Count
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
