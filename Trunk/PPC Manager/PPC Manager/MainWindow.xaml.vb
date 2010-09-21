Imports System.Collections.ObjectModel

Class MainWindow

    Private Sub MenuItem1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Öffnen.Click
        Dim dialog = New OpenFileDialog
        With dialog
            .Filter = "Excel Dateien|*.xlsx;*.xls"
            If .ShowDialog(Me) = True Then
                Dim name = .SafeFileName
            End If
        End With
    End Sub

    Private Sub Vereine_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Vereine.Click
        Dim dialog As New Optionen
        dialog.ShowDialog()
    End Sub

    Private Sub DataGrid1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles DataGrid1.KeyDown
        DataGrid1.BeginEdit()
    End Sub
End Class
