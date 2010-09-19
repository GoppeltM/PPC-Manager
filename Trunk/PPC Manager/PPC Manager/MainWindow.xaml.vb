Imports System.Collections.ObjectModel

Class MainWindow

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim keys = Me.Resources.Item("myPersonen")
        Console.WriteLine()
        

    End Sub

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
        Dim dialog As New Vereine_bearbeiten
        Dim result = dialog.ShowDialog
        If result = True Then
            My.Settings.Vereine.Clear()
            For Each verein In dialog.ListBox1.Items
                My.Settings.Vereine.Add(verein)
            Next
            My.Settings.Save()
        End If

    End Sub
End Class
