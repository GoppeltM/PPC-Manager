﻿Public Class LadenNeu

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        With LadenDialog()
            If .ShowDialog Then
                XMLPathText.Text = .FileName
                My.Settings.LetztesVerzeichnis = IO.Path.GetDirectoryName(.FileName)
                With CompetitionCombo
                    .Items.Clear()
                    For Each Entry In XDocument.Load(XMLPathText.Text).Root.<competition>
                        .Items.Add(Entry.Attribute("age-group").Value)
                    Next
                    .IsEnabled = True
                End With
            End If
        End With        
    End Sub

    Public Shared Function SpeichernDialog() As SaveFileDialog
        Dim dialog As New SaveFileDialog
        With dialog
            .Filter = "XML Dateien |*.xml"
            If My.Settings.LetztesVerzeichnis = String.Empty Then
                .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            Else
                .InitialDirectory = My.Settings.LetztesVerzeichnis
            End If

        End With
        Return dialog
    End Function

    Public Shared Function LadenDialog() As OpenFileDialog
        Dim dialog = New OpenFileDialog
        With dialog
            .Filter = "XML Dateien |*.xml"
            If My.Settings.LetztesVerzeichnis = String.Empty Then
                .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            Else
                .InitialDirectory = My.Settings.LetztesVerzeichnis
            End If

        End With
        Return dialog
    End Function

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button3.Click
        Application.Current.Shutdown()
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub
End Class

Public Class IsNotNullConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return value IsNot Nothing
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
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