Public Class Vereine_bearbeiten


    Private Sub Vereine_bearbeiten_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        'DataGrid1.Items.Clear()
        'For Each verein As String In My.Settings.Vereine
        '    DataGrid1.Items.Add(verein)
        'Next
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        Dim textbox = Resources.Item("VereinsText")
        ListBox1.Items.Add(textbox)
    End Sub
End Class
