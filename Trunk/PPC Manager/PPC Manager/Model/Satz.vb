Imports System.ComponentModel

Public Class Satz
    Implements INotifyPropertyChanged

    Private _PunkteLinks As Integer = 0
    Public Property PunkteLinks As Integer
        Get
            Return _PunkteLinks
        End Get
        Set(ByVal value As Integer)
            _PunkteLinks = value
            OnPropertyChanged("PunkteLinks")
            OnPropertyChanged("MySelf")
        End Set
    End Property

    Private _PunkteRechts As Integer = 0
    Public Property PunkteRechts As Integer
        Get
            Return _PunkteRechts
        End Get
        Set(ByVal value As Integer)
            _PunkteRechts = value
            OnPropertyChanged("PunkteRechts")
            OnPropertyChanged("MySelf")
        End Set
    End Property

    Public Property MySelf As Satz = Me
    Private GewinnPunkte As Integer = My.Settings.GewinnPunkte

    Public ReadOnly Property Abgeschlossen As Boolean
        Get
            Return Math.Max(PunkteLinks, PunkteRechts) >= GewinnPunkte _
                AndAlso Math.Abs(PunkteLinks - PunkteRechts) >= 2
        End Get
    End Property

    Public ReadOnly Property GewonnenLinks As Boolean
        Get
            Return Abgeschlossen AndAlso PunkteLinks > PunkteRechts
        End Get
    End Property

    Public ReadOnly Property GewonnenRechts As Boolean
        Get
            Return Abgeschlossen AndAlso PunkteLinks < PunkteRechts
        End Get
    End Property

    Private Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Public Overrides Function ToString() As String
        Return String.Format("{0}:{1}", PunkteLinks, PunkteRechts)
    End Function

End Class
