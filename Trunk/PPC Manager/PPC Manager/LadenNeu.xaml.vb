﻿Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class LadenNeu

    Private suppressSave As Boolean = False
    Private doc As XDocument

    'Upon rendering the dialog, immediately call Datei_Laden
    Private Sub LadenNeu_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Datei_Laden(sender, e)
    End Sub

    Private Sub Datei_Laden(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        With LadenDialog()
            If .ShowDialog Then
                XMLPathText.Text = .FileName
                My.Settings.LetztesVerzeichnis = IO.Path.GetDirectoryName(.FileName)
                doc = XDocument.Load(XMLPathText.Text)
                With CompetitionCombo
                    .Items.Clear()
                    For Each Entry In doc.Root.<competition>
                        .Items.Add(Entry.Attribute("ttr-remarks").Value)
                    Next
                    .IsEnabled = True
                End With

                If CompetitionCombo.Items.Count > 0 Then
                    CompetitionCombo.SelectedIndex = 0
                    Turnier_Laden(sender, e)
                End If

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
            .Title = "Turnierdatei laden"
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
        Me.DialogResult = False
        Me.Close()
    End Sub

    Private Sub Turnier_Laden(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub CompetitionCombo_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles CompetitionCombo.SelectionChanged
        If Not e.AddedItems.Count > 0 Then Return
        Dim Item = e.AddedItems(0).ToString
        Dim competition = (From x In doc.Root.<competition>
                           Where x.Attribute("ttr-remarks").Value = Item Select x).Single

        ' don't trigger save event while parsing XML and updating UI
        suppressSave = True

        If competition.@ppc:gewinnsätze IsNot Nothing Then
            GewinnsätzeAnzahl.Value = Double.Parse(competition.@ppc:gewinnsätze)
        End If

        If competition.@ppc:satzdifferenz IsNot Nothing Then
            SatzDiffCheck.IsChecked = Boolean.Parse(competition.@ppc:satzdifferenz)
        End If

        If competition.@ppc:sonnebornberger IsNot Nothing Then
            SonneBorn.IsChecked = Boolean.Parse(competition.@ppc:sonnebornberger)
        End If

        If competition.<matches>.Elements.Except(competition.<matches>.<ppc:inactiveplayer>).Any Then
            SatzDiffCheck.IsEnabled = False
            GewinnsätzeAnzahl.IsEnabled = False
            SonneBorn.IsEnabled = False
        Else
            SatzDiffCheck.IsEnabled = True
            GewinnsätzeAnzahl.IsEnabled = True
            SonneBorn.IsEnabled = True
        End If

        suppressSave = False
    End Sub

    Private Sub SaveRules()
        If CompetitionCombo.SelectedItem Is Nothing Then Return
        If suppressSave Then Return

        Dim comp = CompetitionCombo.SelectedItem.ToString
        If comp <> "" Then
            Dim competition = (From x In doc.Root.<competition>
                               Where x.Attribute("ttr-remarks").Value = comp Select x).Single
            competition.@ppc:gewinnsätze = GewinnsätzeAnzahl.Value.ToString.ToLower
            competition.@ppc:satzdifferenz = SatzDiffCheck.IsChecked.ToString.ToLower
            competition.@ppc:sonnebornberger = SonneBorn.IsChecked.ToString.ToLower

            doc.Save(XMLPathText.Text)
        End If

    End Sub

    Private Sub SonneBorn_Checked(sender As Object, e As RoutedEventArgs) Handles SonneBorn.Click
        SaveRules()
    End Sub

    Private Sub SatzDiffCheck_Checked(sender As Object, e As RoutedEventArgs) Handles SatzDiffCheck.Click
        SaveRules()
    End Sub

    Private Sub GewinnsätzeAnzahl_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        SaveRules()
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