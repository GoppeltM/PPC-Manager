Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports <xmlns="http://PPCManager/SpeicherStand">

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



    Public Sub New(ByVal spielerLinks As Spieler, ByVal spielerRechts As Spieler)
        Spieler = New KeyValuePair(Of Spieler, Spieler)(spielerLinks, spielerRechts)
        For i = 1 To My.Settings.GewinnSätze
            Me.Add(New Satz)
        Next
    End Sub

    Private Property Spieler As KeyValuePair(Of Spieler, Spieler)

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

    Public Overridable ReadOnly Property MeineGewonnenenSätze(ByVal ich As Spieler) As IList(Of Satz)
        Get
            Dim gewonnenLinks = From x In Me Where x.Abgeschlossen AndAlso x.PunkteLinks > x.PunkteRechts
                           Select x

            Dim gewonnenRechts = From x In Me Where x.Abgeschlossen AndAlso x.PunkteLinks < x.PunkteRechts
                           Select x

            If SpielerLinks = ich Then
                Return gewonnenLinks.ToList
            Else
                Return gewonnenRechts.ToList
            End If
        End Get
    End Property

    Property Abgeschlossen As Boolean

    Overridable Function ToXML() As XElement
        Dim node = <SpielPartie SpielerLinks=<%= SpielerLinks.StartNummer %> SpielerRechts=<%= SpielerRechts.StartNummer %>>
                       <%= From x In Me Let y = x.ToXML() Select y %>
                   </SpielPartie>
        Return node
    End Function


    Shared Function FromXML(ByVal spielerListe As IEnumerable(Of PPC_Manager.Spieler), ByVal xSpielPartie As XElement) As SpielPartie
        Dim spielerA = (From x In spielerListe Where x.StartNummer = Integer.Parse(xSpielPartie.@SpielerA) Select x).First
        Dim spielerB = (From x In spielerListe Where x.StartNummer = Integer.Parse(xSpielPartie.@SpielerB) Select x).First

        Dim partie As New SpielPartie(spielerA, spielerB)
        Return partie
    End Function

End Class

Public Class FreiLosSpiel
    Inherits SpielPartie

    Public Sub New(ByVal freilosSpieler As Spieler)
        MyBase.New(freilosSpieler, freilosSpieler)
    End Sub


    Public Overrides Function ToXML() As System.Xml.Linq.XElement
        Dim node = <FreiLosSpiel Spieler=<%= SpielerLinks %>/>
        Return node
    End Function

    Public Overrides ReadOnly Property MeineGewonnenenSätze(ByVal ich As Spieler) As System.Collections.Generic.IList(Of Satz)
        Get
            Return New List(Of Satz) From {New Satz() With {.PunkteLinks = 1}}
        End Get
    End Property

    Overloads Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xFreilosSpiel As XElement) As FreiLosSpiel
        Dim spieler = (From x In spielerListe Where x.StartNummer = Integer.Parse(xFreilosSpiel.@Spieler) Select x).First
        Return New FreiLosSpiel(spieler)
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
