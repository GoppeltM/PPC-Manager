Imports System.ComponentModel

Public Class FremdSpielerDialog

    Private ReadOnly _Doc As XDocument

    Public Sub New(doc As XDocument)
        InitializeComponent()
        _Doc = doc
    End Sub

    Public ReadOnly Property Spieler As Spieler
        Get
            Return DirectCast(Resources("AktuellerSpieler"), SpielerContainer).Spieler
        End Get
    End Property

    Public Shared Function NeuerFremdSpieler(doc As XDocument, ttr As Integer, lizenzNr As Integer) As FremdSpielerDialog
        Dim dialog As New FremdSpielerDialog(doc)
        dialog.Spieler.TTR = ttr
        dialog.Spieler.LizenzNr = lizenzNr
        dialog.Spieler.ID = "PLAYER" & lizenzNr
        Return dialog
    End Function

    Public Shared Function EditiereFremdSpieler(doc As XDocument, spieler As Spieler) As FremdSpielerDialog
        Dim dialog As New FremdSpielerDialog(doc)
        DirectCast(dialog.Resources("AktuellerSpieler"), SpielerContainer).Spieler = Spieler
        dialog.Klassements.SelectedItem = Spieler.Klassement
        Return dialog
    End Function

    Private Sub FremdSpielerDialog_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        Dim Categories = From x In _Doc.Root.<competition> Select x.Attribute("age-group").Value

        For Each cat In Categories
            Klassements.Items.Add(cat)
        Next        
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)

        If Spieler.XmlKnoten.Ancestors.Any Then
            Spieler.XmlKnoten.Remove()
        End If

        Dim SelektiertesKlassement = Klassements.SelectedItem.ToString
        Dim klassementKnoten = (From x In _Doc.Root.<competition>
                                Where x.Attribute("age-group").Value = SelektiertesKlassement Select x).Single
        Dim neuesElement = New XElement(Spieler.XmlKnoten)
        klassementKnoten.<players>.First.Add(neuesElement)        
        Spieler.XmlKnoten = neuesElement
        Me.DialogResult = True
        Me.Close()
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
