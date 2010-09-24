Imports System.ComponentModel
Imports System.Collections.ObjectModel

''' <summary>
''' 
''' </summary>
''' <remarks>
''' Ein Spiel besteht aus den darin teilnehmenden Spielern, und den Ergebnissen die sie verbuchen konnten.
''' Jedes Ergebnis besteht aus 3 oder 5 Sätzen, und den Punkten die darin angehäuft wurden.
''' </remarks>
Public Class SpielPartie
    Implements INotifyPropertyChanged


    Public Sub New()
        'For i = 1 To My.Settings.MaxSätze
        '    Sätze.Add(New Satz)
        'Next
        Spieler.Add(New Spieler)
        Spieler.Add(New Spieler)
    End Sub

    Public Property Spieler As New ObservableCollection(Of Spieler)

    Public Property Sätze As New ObservableCollection(Of Satz)


    Public ReadOnly Property MeinGegner(ByVal ich As Spieler) As Spieler
        Get

        End Get
    End Property

    Public ReadOnly Property SpielerLinks As Spieler
        Get
            Return Spieler.First
        End Get
    End Property

    Public ReadOnly Property SpielerRechts As Spieler
        Get
            Return Spieler.Last
        End Get
    End Property

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
End Class

Public Class Satz
    Implements INotifyPropertyChanged

    Private _PunkteLinks As Integer = 0
    Public Property PunkteLinks As Integer
        Get
            Return _PunkteLinks
        End Get
        Set(ByVal value As Integer)
            _PunkteLinks = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("PunkteLinks"))
        End Set
    End Property

    Private _PunkteRechts As Integer = 0
    Public Property PunkteRechts As Integer
        Get
            Return _PunkteRechts
        End Get
        Set(ByVal value As Integer)
            _PunkteRechts = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("PunkteRechts"))
        End Set
    End Property

    Public ReadOnly Property Abgeschlossen As Boolean
        Get
            Return Math.Max(PunkteLinks, PunkteRechts) >= My.Settings.GewinnPunkte
        End Get
    End Property


    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
End Class
