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
    Inherits ObservableCollection(Of Satz)
    Implements INotifyPropertyChanged



    Public Sub New(ByVal spielerLinks As Spieler, ByVal spielerRechts As Spieler, ByVal runde As Integer)
        Spieler = New KeyValuePair(Of Spieler, Spieler)(spielerLinks, spielerRechts)
        _Runde = runde
        For i = 1 To My.Settings.MaxSätze
            Me.Add(New Satz)
        Next
    End Sub

    Private Property Spieler As KeyValuePair(Of Spieler, Spieler)

    Private _Runde As Integer
    Public ReadOnly Property Runde() As Integer
        Get
            Return _Runde
        End Get
    End Property


    Public ReadOnly Property SpielerLinks As Spieler
        Get
            Return Spieler.Key
        End Get
    End Property

    Public ReadOnly Property SpielerRechts As Spieler
        Get
            Return Spieler.Value
        End Get
    End Property

    Public ReadOnly Property MeinGegner(ByVal ich As Spieler) As Spieler
        Get
            If Spieler.Key = ich Then
                Return Spieler.Value
            Else
                Return Spieler.Key
            End If
        End Get
    End Property

    Public ReadOnly Property MeineGewonnenenSätze(ByVal ich As Spieler) As IList(Of Satz)
        Get
            Dim gewonnenLinks = From x In Me Where x.Abgeschlossen AndAlso x.PunkteLinks > x.PunkteRechts
                           Select x

            Dim gewonnenRechts = From x In Me Where x.Abgeschlossen AndAlso x.PunkteLinks < x.PunkteRechts
                           Select x

            If SpielerLinks = ich Then
                Return gewonnenLinks
            Else
                Return gewonnenRechts
            End If
        End Get
    End Property

    Function ToXML() As XElement
        Dim node = <SpielPartie SpielerLinks=<%= SpielerLinks.StartNummer %> SpielerRechts=<%= SpielerRechts.StartNummer %>>
                       <%= From x In Me Let y = x.ToXML() Select y %>
                   </SpielPartie>
        Throw New NotImplementedException
    End Function

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

    Function ToXML() As XElement
        Dim node = <Satz PunkteLinks=<%= PunkteLinks %> PunkteRechts=<%= PunkteRechts %>>

                   </Satz>
        Return node
    End Function

End Class
