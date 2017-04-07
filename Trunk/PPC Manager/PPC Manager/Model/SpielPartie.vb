Imports System.ComponentModel
Imports System.Collections.ObjectModel

''' <summary>
''' 
''' </summary>
''' <remarks>
''' Ein Spiel besteht aus den darin teilnehmenden Spielern, und den Ergebnissen die sie verbuchen konnten.
''' Jedes Ergebnis besteht aus 3 oder 5 Sätzen, und den Punkten die darin angehäuft wurden.
''' </remarks>
''' 
<DebuggerDisplay("Runde = {RundenName}")>
Public Class SpielPartie
    Inherits ObservableCollection(Of Satz)

    Protected ReadOnly _GewinnSätze As Integer
    Private ReadOnly Spieler As KeyValuePair(Of SpielerInfo, SpielerInfo)
    Private ReadOnly _RundenName As String
    Public ReadOnly Property RundenName As String
        Get
            Return _RundenName
        End Get
    End Property


    Public Sub New(rundenName As String, ByVal spielerLinks As SpielerInfo, ByVal spielerRechts As SpielerInfo, gewinnsätze As Integer)
        If spielerLinks Is Nothing Then Throw New ArgumentNullException
        If spielerRechts Is Nothing Then Throw New ArgumentNullException

        Spieler = New KeyValuePair(Of SpielerInfo, SpielerInfo)(spielerLinks, spielerRechts)
        _RundenName = rundenName
        _GewinnSätze = gewinnsätze
        AddHandler Me.CollectionChanged, Sub()
                                             Me.OnPropertyChanged(New PropertyChangedEventArgs("Abgeschlossen"))
                                         End Sub
    End Sub

    Public ReadOnly Property SpielerLinks As SpielerInfo
        Get
            Return Spieler.Key
        End Get
    End Property

    Public ReadOnly Property SpielerRechts As SpielerInfo
        Get
            Return Spieler.Value
        End Get
    End Property

    Public Property ZeitStempel As Date

    Public ReadOnly Property MeinGegner(ByVal ich As SpielerInfo) As SpielerInfo
        Get
            If Spieler.Key = ich Then
                Return Spieler.Value
            Else
                Return Spieler.Key
            End If
        End Get
    End Property

    Public Overridable ReadOnly Property Abgeschlossen() As Boolean
        Get
            Dim AbgeschlosseneSätzeLinks = Aggregate x In Me Where x.GewonnenLinks Into Count()

            Dim AbgeschlosseneSätzeRechts = Aggregate x In Me Where x.GewonnenRechts Into Count()

            Return Math.Max(AbgeschlosseneSätzeLinks, AbgeschlosseneSätzeRechts) >= _GewinnSätze
        End Get
    End Property

    Public Overridable ReadOnly Property MeineVerlorenenSätze(ByVal ich As SpielerInfo) As IList(Of Satz)
        Get
            Dim verlorenLinks = From x In Me Where x.Abgeschlossen AndAlso x.PunkteRechts > x.PunkteLinks
                                Select x

            Dim verlorenRechts = From x In Me Where x.Abgeschlossen AndAlso x.PunkteLinks > x.PunkteRechts
                                 Select x

            If SpielerLinks = ich Then
                Return verlorenLinks.ToList
            Else
                Return verlorenRechts.ToList
            End If
        End Get
    End Property

    Public Overridable ReadOnly Property MeineGewonnenenSätze(ByVal ich As SpielerInfo) As IList(Of Satz)
        Get
            Dim gewonnenLinks = From x In Me Where x.GewonnenLinks

            Dim gewonnenRechts = From x In Me Where x.GewonnenRechts

            If SpielerLinks = ich Then
                Return gewonnenLinks.ToList
            Else
                Return gewonnenRechts.ToList
            End If
        End Get
    End Property

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim other = TryCast(obj, SpielPartie)
        If other Is Nothing Then Return False
        If Me.SpielerLinks <> other.SpielerLinks Then Return False
        If Me.SpielerRechts <> other.SpielerRechts Then Return False
        If Me.RundenName <> other.RundenName Then Return False
        Return True
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return SpielerLinks.GetHashCode Xor SpielerRechts.GetHashCode Xor RundenName.GetHashCode
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0} : {1}", SpielerLinks, SpielerRechts)
    End Function

End Class
