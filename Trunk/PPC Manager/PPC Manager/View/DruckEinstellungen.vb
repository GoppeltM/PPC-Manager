Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class DruckEinstellungen
    Implements INotifyPropertyChanged

    Public Property DruckeNeuePaarungen As Boolean
    Public Property DruckeSchiedsrichterzettel As Boolean
    Public Property DruckeSpielergebnisse As Boolean
    Public Property DruckeRangliste As Boolean

    Private _EinstellungenNeuePaarungen As PrintDialog
    Public Property EinstellungenNeuePaarungen As PrintDialog
        Get
            Return _EinstellungenNeuePaarungen
        End Get
        Set(value As PrintDialog)
            _EinstellungenNeuePaarungen = value
            OnPropertyChanged()
        End Set
    End Property

    Private _EinstellungenSchiedsrichterzettel As PrintDialog
    Public Property EinstellungenSchiedsrichterzettel As PrintDialog
        Get
            Return _EinstellungenSchiedsrichterzettel
        End Get
        Set(value As PrintDialog)
            _EinstellungenSchiedsrichterzettel = value
            OnPropertyChanged()
        End Set
    End Property

    Private _Einstellungenspielergebnisse As PrintDialog
    Public Property EinstellungenSpielergebnisse As PrintDialog
        Get
            Return _Einstellungenspielergebnisse
        End Get
        Set(value As PrintDialog)
            _Einstellungenspielergebnisse = value
            OnPropertyChanged()
        End Set
    End Property

    Private _EinstellungenRangliste As PrintDialog
    Public Property EinstellungenRangliste As PrintDialog
        Get
            Return _EinstellungenRangliste
        End Get
        Set(value As PrintDialog)
            _EinstellungenRangliste = value
            OnPropertyChanged()
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub OnPropertyChanged(<CallerMemberName()> Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class

