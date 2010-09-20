Public Class Optionen


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        Me.DialogResult = True
        My.Settings.Vereine.Clear()
        For Each verein As Verein In Resources.Item("Vereine")
            My.Settings.Vereine.Add(verein.Vereinsname)
        Next
        My.Settings.Save()
        Me.Close()
    End Sub
End Class

Friend Class VereinsDaten
    Inherits ObjectModel.ObservableCollection(Of Verein)

    Public Sub New()
        For Each verein As String In My.Settings.Vereine
            Me.Add(New Verein With {.Vereinsname = verein})
        Next
    End Sub
End Class

Friend Class Verein
    Public Property Vereinsname As String
End Class