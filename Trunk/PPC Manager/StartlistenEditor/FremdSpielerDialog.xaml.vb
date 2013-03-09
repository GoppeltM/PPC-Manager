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

            For Each cat In Categories
                VerfügbareKlassements.Items.Add(cat)
            Next
        End If
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
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