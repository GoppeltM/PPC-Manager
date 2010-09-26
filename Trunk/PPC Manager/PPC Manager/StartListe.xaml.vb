Class StartListe

    Private Sub DataGrid1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles DataGrid1.KeyDown
        Dim Cell = DataGrid1.CurrentCell
        If Cell.Column.Header = "Verein" Then
            DataGrid1.BeginEdit()
        End If

    End Sub

    Private Sub Paste_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        Dim HasData = Clipboard.ContainsData("CSV")
        e.CanExecute = HasData AndAlso DataGrid1.SelectedCells.Any
    End Sub

    Private Sub Paste_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim data As String = Clipboard.GetDataObject.GetData("CSV")
        Dim ClipboardRows = New List(Of List(Of String))
        Dim Rows = Regex.Split(data, String.Format(Environment.NewLine)).ToList
        Rows.RemoveAt(Rows.Count - 1)

        For Each row In Rows
            ClipboardRows.Add(row.Split(","c, ";"c).ToList)
        Next


        Dim liste As SpielerListe = Me.FindResource("myPersonen")

        Dim selectedRows = From x In DataGrid1.SelectedCells Group By x.Item Into Group

        Dim currentRow = 0

        'Wenn letzte Zeile gleich Platzhalter, erstmal Platz machen

        Dim LastSelectedRow = selectedRows.Last
        If Not LastSelectedRow.Item.GetType Is GetType(Spieler) Then
            Dim selectedCells = LastSelectedRow.Group

            For i = selectedRows.Count To ClipboardRows.Count
                Dim spieler = New Spieler
                liste.Add(spieler)
                For Each cellInfo In selectedCells
                    Dim newCell As New DataGridCellInfo(spieler, cellInfo.Column)
                    DataGrid1.SelectedCells.Add(newCell)
                Next
                For Each cellInfo In selectedCells
                    DataGrid1.SelectedCells.Remove(cellInfo)
                Next
            Next
            DataGrid1.UpdateLayout()
        End If

        For Each row In ClipboardRows

            If currentRow >= selectedRows.Count Then Exit For
            Dim selectedGroup = selectedRows(currentRow)
            Dim selectedCells = selectedGroup.Group
            Dim SelectedRow = selectedRows(currentRow).Item

            Dim currentSpieler = selectedRows(currentRow).Item
            currentRow += 1


            Dim currentClipCell = 0
            For Each cell In selectedCells
                If currentClipCell >= row.Count Then Exit For
                Dim value = row(currentClipCell)
                currentClipCell += 1

                Dim myProperty As Object = cell.Column.GetCellContent(currentSpieler)

                Select Case myProperty.GetType
                    Case GetType(TextBlock)
                        Dim text = CType(myProperty, TextBlock)
                        text.Text = value
                    Case GetType(ComboBox)
                        Dim combo = CType(myProperty, ComboBox)
                        combo.SelectedItem = value
                End Select
            Next
        Next

    End Sub

    Private Sub Cut_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = DataGrid1.SelectedItems.Count > 0
    End Sub

    Private Sub Cut_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)        
        ApplicationCommands.Copy.Execute(DataGrid1.SelectedCells, DataGrid1)
        ApplicationCommands.Delete.Execute(DataGrid1.SelectedCells, DataGrid1)
    End Sub

    Private Sub Copy_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = DataGrid1.SelectedCells.Any
    End Sub

    Private Sub CommandBinding_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim selectedRows = From x In DataGrid1.SelectedCells Group By x.Item Into Group


        Dim CSVContent = String.Empty
        For Each row In selectedRows
            Dim CellContents As New List(Of String)
            For Each cell In row.Group
                Dim myProperty As Object = cell.Column.GetCellContent(row.Item)
                Select Case myProperty.GetType
                    Case GetType(TextBlock)
                        Dim text = CType(myProperty, TextBlock)
                        CellContents.Add(text.Text)
                    Case GetType(ComboBox)
                        Dim combo = CType(myProperty, ComboBox)
                        CellContents.Add(combo.SelectedItem)
                End Select
            Next
            CSVContent &= String.Join(";"c, CellContents.ToArray)
            CSVContent &= Environment.NewLine
        Next
        Clipboard.SetData(DataFormats.CommaSeparatedValue, CSVContent)
    End Sub
End Class
