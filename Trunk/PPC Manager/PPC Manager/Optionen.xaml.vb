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

Public Class SatzZahlConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value = 1 Then
            Return "Ein Gewinnsatz"
        End If
        Return String.Format("{0} Gewinnsätze", value)
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class