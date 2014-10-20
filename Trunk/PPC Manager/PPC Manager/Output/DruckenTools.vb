Public Class DruckenTools

    Public Shared Sub SpaltenAngleichen(seiten As IEnumerable(Of UserControl), gridName As String)
        Dim ColumnsOberesLimit As New Dictionary(Of Integer, Double)
        For Each seite In seiten
            Dim datagrid = FindChild(Of DataGrid)(seite, gridName)
            If datagrid Is Nothing Then
                Throw New ArgumentException("Grid nicht gefunden")
            End If
            For Each column In datagrid.Columns
                If Not ColumnsOberesLimit.ContainsKey(column.DisplayIndex) Then
                    ColumnsOberesLimit.Add(column.DisplayIndex, column.ActualWidth)
                End If
                If column.ActualWidth > ColumnsOberesLimit.Item(column.DisplayIndex) Then
                    ColumnsOberesLimit.Item(column.DisplayIndex) = column.ActualWidth
                End If
            Next
        Next

        For Each seite In seiten
            Dim datagrid = FindChild(Of DataGrid)(seite, gridName)
            If datagrid Is Nothing Then
                Throw New ArgumentException("Grid nicht gefunden")
            End If
            For Each column In ColumnsOberesLimit
                datagrid.Columns(column.Key).MinWidth = column.Value
            Next
            seite.UpdateLayout()
        Next

    End Sub

    Public Shared Function FindChild(Of T As FrameworkElement)(item As DependencyObject, name As String) As T
        Dim value = item.GetValue(Controls.Control.NameProperty).ToString
        If TypeOf item Is T And value = name Then
            Return CType(item, T)
        End If
        Dim childCount = VisualTreeHelper.GetChildrenCount(item)
        For Each index In Enumerable.Range(0, childCount)
            Dim child = VisualTreeHelper.GetChild(item, index)
            Dim c = FindChild(Of T)(child, name)
            If c IsNot Nothing Then
                Return c
            End If
        Next
        Return Nothing
    End Function
End Class
