Imports System.ComponentModel

Public Class Spieler
    Implements IComparable(Of Spieler), INotifyPropertyChanged

#Region "Public Properties"

    Private _Vorname As String
    Public Property Vorname As String
        Get
            Return _Vorname
        End Get
        Set(ByVal value As String)
            _Vorname = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Vorname"))
        End Set
    End Property

    Private _Nachname As String = "Mustermann"
    Public Property Nachname As String
        Get
            Return _Nachname
        End Get
        Set(ByVal value As String)
            _Nachname = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Nachname"))
        End Set
    End Property

    Private _Verein As String
    Public Property Verein As String
        Get
            Return _Verein
        End Get
        Set(ByVal value As String)
            _Verein = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Verein"))
        End Set
    End Property

    Private _SpielKlasse As String = ""
    Public Property SpielKlasse As String
        Get
            Return _SpielKlasse
        End Get
        Set(ByVal value As String)
            _SpielKlasse = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("SpielKlasse"))
        End Set
    End Property

    
    Private _Position As Integer
    Public Property Position() As Integer
        Get
            Return _Position
        End Get
        Set(ByVal value As Integer)
            _Position = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Position"))
        End Set
    End Property


    Private _turnierKlasse As String
    Public Property TurnierKlasse() As String
        Get
            Return _turnierKlasse
        End Get
        Set(ByVal value As String)
            _turnierKlasse = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("TurnierKlasse"))
        End Set
    End Property


    Private _StartNummer As Integer
    Public Property StartNummer() As Integer
        Get
            Return _StartNummer
        End Get
        Set(ByVal value As Integer)
            _StartNummer = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("StartNummer"))
        End Set
    End Property


    Public ReadOnly Property Punkte As Integer
        Get

        End Get
    End Property

    Public Property BuchholzPunkte As Integer

    Public Property GespieltePartien As IList(Of SpielPartie)

    Public Property FreiLosInRunde As Integer


#End Region


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

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
