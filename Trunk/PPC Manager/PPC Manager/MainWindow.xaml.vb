Imports System.Collections.ObjectModel

Class MainWindow

    Private Sub Öffnen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Öffnen.Click
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

    Private Sub TurnierStarten_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles TurnierStarten.Click
        Frame1.Navigate(New Uri("pack://application:,,,/Begegnungen.xaml"))
    End Sub

    Private Sub Close_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub Close_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        My.Application.MainWindow.Close()
    End Sub
End Class
