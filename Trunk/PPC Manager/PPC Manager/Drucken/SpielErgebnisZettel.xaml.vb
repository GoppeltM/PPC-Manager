Public Class SpielErgebnisZettel
    Implements IPaginatibleUserControl

    Public Function GetMaxItemCount() As Integer Implements IPaginatibleUserControl.GetMaxItemCount
        Dim width = PageContent.ActualWidth
        Dim height = PageContent.ActualHeight

        Dim ItemHeight = 151
        Dim ItemWidth = 359.055118110236

        Dim Rows = CInt(width) \ CInt(ItemWidth)
        Dim Columns = CInt(height) \ CInt(ItemHeight)

        Dim total = Rows * Columns
        If total = 0 Then
            Throw New Exception("Seitenformat zu klein! Kann kein Element darauf vollständig drucken!")
        End If
        Return total
    End Function

    Public Sub SetSource(startIndex As Integer, ByVal elements As IEnumerable(Of Object)) Implements IPaginatibleUserControl.SetSource
        ItemsContainer.ItemsSource = elements
    End Sub
End Class
