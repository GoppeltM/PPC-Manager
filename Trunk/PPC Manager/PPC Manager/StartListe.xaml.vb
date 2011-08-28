Class StartListe


    Private Sub Paste_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        Dim HasData = Clipboard.ContainsData("CSV")
        e.CanExecute = HasData AndAlso DataGrid1.SelectedCells.Any
    End Sub

    Private Sub Paste_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim data As String = Clipboard.GetDataObject.GetData("CSV").ToString
        Dim ClipboardRows = New List(Of List(Of String))
        Dim Rows = Regex.Split(data, String.Format(Environment.NewLine)).ToList
        Rows.RemoveAt(Rows.Count - 1)

        For Each row In Rows
            ClipboardRows.Add(row.Split(","c, ";"c).ToList)
        Next


        Dim liste As SpielerListe = CType(Me.FindResource("SpielerListe"), SpielerListe)

        Dim selectedRows = From x In DataGrid1.SelectedCells Group By x.Item Into Group

        Dim currentRow = 0

        'Wenn letzte Zeile gleich Platzhalter, erstmal Platz machen

        Dim LastSelectedRow = selectedRows.Last
        If Not LastSelectedRow.Item.GetType Is GetType(Spieler) Then
            Dim selectedCells = LastSelectedRow.Group

            For i = selectedRows.Count To ClipboardRows.Count
                Dim spieler = New Spieler()
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
                    Case Else
                        CType(currentSpieler, Spieler).Verein = value
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

    Private Sub Copy_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim selectedRows = From x In DataGrid1.SelectedCells Where x.Item.GetType Is GetType(Spieler) Group By x.Item Into Group


        Dim CSVContent = String.Empty
        For Each row In selectedRows            
            Dim CellContents As New List(Of String)
            For Each cell In row.Group
                Dim item = CType(cell.Item, Spieler)
                If cell.Column Is Vorname Then
                    CellContents.Add(item.Vorname)
                    Continue For
                End If

                If cell.Column Is Nachname Then
                    CellContents.Add(item.Nachname)
                    Continue For
                End If

                If cell.Column Is Verein Then
                    CellContents.Add(item.Verein)
                    Continue For
                End If

                If cell.Column Is SpielKlasse Then
                    CellContents.Add(item.SpielKlasse)
                    Continue For
                End If

                If cell.Column Is Position Then
                    CellContents.Add(item.Position.ToString)
                    Continue For
                End If

                If cell.Column Is Turnierklasse Then
                    CellContents.Add(item.TurnierKlasse)
                    Continue For
                End If
            Next
            CSVContent &= String.Join(";"c, CellContents.ToArray)
            CSVContent &= Environment.NewLine
        Next
        Clipboard.SetData(DataFormats.CommaSeparatedValue, CSVContent)
    End Sub

    Private Sub DataGrid1_CurrentCellChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DataGrid1.CurrentCellChanged
        If DataGrid1.CurrentColumn Is Nothing Then Return
        If DataGrid1.CurrentColumn.GetType Is GetType(DataGridComboBoxColumn) Then
            DataGrid1.BeginEdit()
        End If

    End Sub

    Private Sub DataGrid1_LoadingRow(ByVal sender As System.Object, ByVal e As System.Windows.Controls.DataGridRowEventArgs) Handles DataGrid1.LoadingRow
        e.Row.Header = e.Row.GetIndex + 1
    End Sub

End Class
