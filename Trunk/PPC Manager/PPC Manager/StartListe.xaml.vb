Class StartListe

    Private Sub DataGrid1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles DataGrid1.KeyDown
        Dim Cell = DataGrid1.CurrentCell
        If Cell.Column.Header = "Verein" Then
            DataGrid1.BeginEdit()
        End If

    End Sub

    Private Sub CommandBinding_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = Clipboard.ContainsData("CSV")
    End Sub

    Private Sub CommandBinding_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim data As String = Clipboard.GetDataObject.GetData("CSV")
        Dim Items = New List(Of String)
        Dim Rows = Regex.Split(data, String.Format(Environment.NewLine)).ToList
        Rows.RemoveAt(Rows.Count - 1)

        For Each row In Rows
            Items.AddRange(row.Split(","c, ";"c))
        Next
        Dim current = 0
        For Each cell In DataGrid1.SelectedCells
            If current >= Items.Count Then current = 0
            Dim spieler As Spieler
            If Not TypeOf cell.Item Is Spieler Then
                spieler = New Spieler
                Dim liste As SpielerListe = Resources.Item("myPersonen")
                liste.Add(spieler)
            Else
                spieler = cell.Item
            End If

            Select Case cell.Column.Header
                Case "Vorname" : spieler.Vorname = Items(current)
                Case "Nachname" : spieler.Nachname = Items(current)
                Case "Verein" : spieler.Verein = Items(current)
                Case "Spielklasse" : spieler.SpielKlasse = Items(current)
                Case "Position" : Integer.TryParse(Items(current), spieler.Position)
                Case "Turnierklasse" : spieler.TurnierKlasse = Items(current)
            End Select
            current += 1
        Next
    End Sub

    Private Sub DataGrid1_Drop(ByVal sender As System.Object, ByVal e As System.Windows.DragEventArgs) Handles DataGrid1.Drop
        Dim data As String = e.Data.GetData("CSV")

    End Sub
End Class
