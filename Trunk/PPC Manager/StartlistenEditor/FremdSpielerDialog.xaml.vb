Imports System.ComponentModel

Public Class FremdSpielerDialog

    Public ReadOnly Property Spieler As Spieler
        Get
            Return DirectCast(Resources("AktuellerSpieler"), SpielerContainer).Spieler
        End Get
    End Property

    Public Shared Function NeuerFremdSpieler(ttr As Integer, lizenzNr As Integer) As FremdSpielerDialog
        Dim dialog As New FremdSpielerDialog
        dialog.Spieler.TTR = ttr
        dialog.Spieler.LizenzNr = lizenzNr
        dialog.Spieler.ID = "PLAYER" & lizenzNr
        Return dialog
    End Function

    Public Shared Function EditiereFremdSpieler(spieler As Spieler) As FremdSpielerDialog
        Dim dialog As New FremdSpielerDialog
        DirectCast(dialog.Resources("AktuellerSpieler"), SpielerContainer).Spieler = spieler
        Return dialog
    End Function

    Private Sub FremdSpielerDialog_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If MainWindow.Doc IsNot Nothing Then
            Dim Categories = From x In MainWindow.Doc.Root.<competition> Select x.Attribute("age-group").Value

            Dim AusgewählteKategorien = Spieler.Klassements.ToList

            For Each cat In Categories.Except(AusgewählteKategorien)
                VerfügbareKlassements.Items.Add(cat)
            Next
            For Each cat In AusgewählteKategorien
                AusgewählteKlassements.Items.Add(cat)
            Next
        End If
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)

        For Each knoten In Spieler.XmlKnoten
            If knoten.Ancestors.Any Then
                knoten.Remove()
            End If
        Next
        Dim xmlKnoten = Spieler.XmlKnoten.First
        Dim AlleXMLKnoten As New List(Of XElement)
        For Each klassement As String In AusgewählteKlassements.Items
            Dim klassementKnoten = (From x In MainWindow.Doc.Root.<competition> Where x.Attribute("age-group").Value = klassement Select x).First
            Dim neuesElement = New XElement(xmlKnoten)
            klassementKnoten.<players>.First.Add(neuesElement)
            AlleXMLKnoten.Add(neuesElement)
        Next
        Spieler.XmlKnoten = AlleXMLKnoten
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        AusgewählteKlassements.Items.Add(VerfügbareKlassements.SelectedItem)
        VerfügbareKlassements.Items.Remove(VerfügbareKlassements.SelectedItem)
    End Sub

    Private Sub Button_Click_3(sender As Object, e As RoutedEventArgs)
        VerfügbareKlassements.Items.Add(AusgewählteKlassements.SelectedItem)
        AusgewählteKlassements.Items.Remove(AusgewählteKlassements.SelectedItem)
    End Sub
End Class

Public Class SpielerContainer
    Implements INotifyPropertyChanged

    Private _Spieler As New Spieler
    Property Spieler As Spieler
        Set(value As Spieler)
            _Spieler = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Spieler"))
        End Set
        Get
            Return _Spieler
        End Get
    End Property

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class

Public Class IsSelectedConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return DirectCast(value, Integer) <> -1
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class