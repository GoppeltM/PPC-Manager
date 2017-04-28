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

    Private Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Public Overrides Function ToString() As String
        Return String.Format("{0}:{1}", PunkteLinks, PunkteRechts)
    End Function

End Class
