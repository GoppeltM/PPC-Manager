
Public Class Optionen


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Neu.Click
        My.Settings.Save()        
        DialogResult = False
        Me.Close()
    End Sub

    Private Sub Laden_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        DialogResult = True
        Me.Close()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Abbrechen.Click
        My.Settings.Reload()        
    End Sub



    Private Sub Abbrechen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Abbrechen.Click
        Me.DialogResult = False
    End Sub
End Class

Friend Class SatzZahlConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If CType(value, Integer) = 1 Then
            Return "Ein Gewinnsatz"
        End If
        Return String.Format("{0} Gewinnsätze", value)
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class

Friend Class FilePathValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim path = value.ToString
        If IO.Path.IsPathRooted(path) AndAlso IO.Path.GetExtension(path) = ".xml" Then
            Return ValidationResult.ValidResult
        End If
        Return New ValidationResult(False, "Bitte einen gültigen Dateipfad angeben!")
    End Function
End Class