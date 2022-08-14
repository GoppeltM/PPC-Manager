Imports System.ComponentModel

Public Class SpielerInfo
    Implements INotifyPropertyChanged, IComparable(Of SpielerInfo)

    Public Sub New()

    End Sub

    Public Sub New(spieler As SpielerInfo)
        ID = spieler.ID
        Vorname = spieler.Vorname
        Nachname = spieler.Nachname
        Verein = spieler.Verein
        TTR = spieler.TTR
        TTRMatchCount = spieler.TTRMatchCount
        LizenzNr = spieler.LizenzNr
        Klassement = spieler.Klassement
        Geschlecht = spieler.Geschlecht
        Geburtsjahr = spieler.Geburtsjahr
        Fremd = spieler.Fremd
        Bezahlt = spieler.Bezahlt
        Abwesend = spieler.Abwesend
        Anwesend = spieler.Anwesend
    End Sub

    Public Property ID As String
    Public Property Vorname As String
    Public Property Nachname As String
    Public Property Verein As String
    Public Property TTR As Integer
    Public Property TTRMatchCount As Integer
    Public Property LizenzNr As String
    Public Property Klassement As String
    Public Property Geschlecht As Integer
    Public Property Geburtsjahr As Integer
    Public Property Fremd As Boolean

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Function CompareTo(other As SpielerInfo) As Integer Implements IComparable(Of SpielerInfo).CompareTo
        Dim diff = TTR - other.TTR
        If diff <> 0 Then Return diff
        diff = TTRMatchCount - other.TTRMatchCount
        If diff <> 0 Then Return diff
        diff = Nachname.CompareTo(other.Nachname)
        If diff <> 0 Then Return diff
        diff = Vorname.CompareTo(other.Vorname)
        If diff <> 0 Then Return diff
        diff = LizenzNr.CompareTo(other.LizenzNr)
        If diff <> 0 Then Return diff
        Return 0
    End Function

    Private _Bezahlt As Boolean
    Public Property Bezahlt As Boolean
        Get
            Return _Bezahlt
        End Get
        Set(value As Boolean)
            _Bezahlt = value
            NotifyPropertyChanged("Bezahlt")
        End Set
    End Property

    Private _Anwesend As Boolean
    Public Property Anwesend As Boolean
        Get
            Return _Anwesend
        End Get
        Set(value As Boolean)
            _Anwesend = value
            NotifyPropertyChanged("Anwesend")
        End Set
    End Property

    Private _Abwesend As Boolean
    Public Property Abwesend As Boolean
        Get
            Return _Abwesend
        End Get
        Set(value As Boolean)
            _Abwesend = value
            NotifyPropertyChanged("Abwesend")
        End Set
    End Property
End Class
