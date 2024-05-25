Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class SpielerInfo

#Region "Public Properties"

    Public Sub New(id As String)
        Me.Id = id
    End Sub

    Public Sub New(spieler As SpielerInfo)
        Fremd = spieler.Fremd
        Geburtsjahr = spieler.Geburtsjahr
        Geschlecht = spieler.Geschlecht
        Id = spieler.Id
        Lizenznummer = spieler.Lizenznummer
        Nachname = spieler.Nachname
        TTRating = spieler.TTRating
        TTRMatchCount = spieler.TTRMatchCount
        Verbandsspitzname = spieler.Verbandsspitzname
        Vereinsname = spieler.Vereinsname
        Vereinsnummer = spieler.Vereinsnummer
        Vorname = spieler.Vorname
    End Sub

    Public ReadOnly Property Id As String

    Public Property Vorname As String = ""

    Public Property Nachname As String = ""

    Public Property Vereinsname As String = ""

    Public Property Vereinsnummer As Integer

    Public Property Verbandsspitzname As String = ""

    Public Property Lizenznummer As String = ""

    Public ReadOnly Property StartNummer As Integer
        Get
            Return Integer.Parse(Regex.Match(Id, "-?\d+\Z").Value)
        End Get
    End Property

    Public Property TTRating As Integer

    Public Property TTRMatchCount As Integer

    Public Property Geschlecht As Integer

    Public Property Fremd As Boolean

    Public Property Geburtsjahr As Integer

#End Region

    Public Overrides Function Equals(ByVal obj As Object) As Boolean

        If TypeOf obj IsNot SpielerInfo Then Return False
        Dim other = CType(obj, SpielerInfo)
        Return Me.Id = other.Id
    End Function


    Public Shared Operator <>(ByVal left As SpielerInfo, ByVal right As SpielerInfo) As Boolean
        Return Not left = right
    End Operator

    Public Shared Operator =(ByVal left As SpielerInfo, ByVal right As SpielerInfo) As Boolean
        Return left.Equals(right)
    End Operator

    Public Overrides Function GetHashCode() As Integer
        Return Id.GetHashCode
    End Function

    Public Overrides Function ToString() As String
        Return Id
    End Function
End Class
