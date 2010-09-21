Imports System.ComponentModel

Public Class Spieler
    Implements IComparable(Of Spieler)

    Public Property Vorname As String = "Bla"

    Public Property Nachname As String = ""

    Public Property Verein As String = ""

    Public Property SpielKlasse As String = ""

    Public Property Position As Integer = 1

    Public Property TurnierKlasse As String = ""

    Public Property StartNummer As Integer = 0

    Public ReadOnly Property Punkte As Integer
        Get

        End Get
    End Property

    Public Property BuchholzPunkte As Integer

    Public Property GespieltePartien As IList(Of SpielPartie)

    Public Property FreiLosInRunde As Integer

    Private ReadOnly Property SatzDifferenz As Integer
        Get            


        End Get
    End Property


    Public Overrides Function ToString() As String
        Return Nachname
    End Function


    Public Function CompareTo(ByVal other As Spieler) As Integer Implements System.IComparable(Of Spieler).CompareTo
        Dim diff = other.Punkte - Me.Punkte
        If diff <> 0 Then Return diff
        diff = other.BuchholzPunkte - Me.BuchholzPunkte
        If diff <> 0 Then Return diff
        If My.Settings.BerücksichtigeSatzDiff Then
            diff = other.SatzDifferenz - Me.SatzDifferenz
            If diff <> 0 Then Return diff
        End If
        Return Me.Vorname & Me.Nachname = other.Vorname & other.Nachname
    End Function


    Public Shared Operator <>(ByVal left As Spieler, ByVal right As Spieler) As Boolean
        Return Not left = right
    End Operator

    Public Shared Operator =(ByVal left As Spieler, ByVal right As Spieler) As Boolean
        Return left.Equals(right)
    End Operator


End Class
