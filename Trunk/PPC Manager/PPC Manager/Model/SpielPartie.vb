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

    Private ReadOnly Kontrahenten As Tuple(Of SpielerInfo, SpielerInfo)
    Private ReadOnly _RundenName As String
    Public ReadOnly Property RundenName As String
        Get
            Return _RundenName
        End Get
    End Property


    Public Sub New(rundenName As String, ByVal spielerLinks As SpielerInfo, ByVal spielerRechts As SpielerInfo)
        If spielerLinks Is Nothing Then Throw New ArgumentNullException
        If spielerRechts Is Nothing Then Throw New ArgumentNullException
        Kontrahenten = Tuple.Create(spielerLinks, spielerRechts)
        _RundenName = rundenName
        AddHandler CollectionChanged, Sub()
                                          OnPropertyChanged(New PropertyChangedEventArgs("MySelf"))
                                      End Sub
    End Sub

    Public ReadOnly Property SpielerLinks As SpielerInfo
        Get
            Return Kontrahenten.Item1
        End Get
    End Property

    Public ReadOnly Property SpielerRechts As SpielerInfo
        Get
            Return Kontrahenten.Item2
        End Get
    End Property

    Public Property ZeitStempel As Date

    Public ReadOnly Property GegnerVon(ByVal Spieler As SpielerInfo) As SpielerInfo
        Get
            If SpielerLinks = Spieler Then
                Return SpielerRechts
            Else
                Return SpielerLinks
            End If
        End Get
    End Property

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim other = TryCast(obj, SpielPartie)
        If other Is Nothing Then Return False
        If SpielerLinks <> other.SpielerLinks Then Return False
        If SpielerRechts <> other.SpielerRechts Then Return False
        If RundenName <> other.RundenName Then Return False
        Return True
    End Function

    Public Shared Operator <>(ByVal left As SpielPartie, ByVal right As SpielPartie) As Boolean
        Return Not left = right
    End Operator

    Public Shared Operator =(ByVal left As SpielPartie, ByVal right As SpielPartie) As Boolean
        Return left.Equals(right)
    End Operator

    Public ReadOnly Property MySelf As SpielPartie
        Get
            Return Me
        End Get
    End Property

    Public Overrides Function GetHashCode() As Integer
        Return SpielerLinks.GetHashCode Xor SpielerRechts.GetHashCode Xor RundenName.GetHashCode
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0} : {1}", SpielerLinks, SpielerRechts)
    End Function

End Class
