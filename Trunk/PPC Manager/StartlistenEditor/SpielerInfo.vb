Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class SpielerInfo
    Implements INotifyPropertyChanged

    Public Property ID As String
    Public Property Vorname As String
    Public Property Nachname As String
    Public Property Verein As String
    Public Property TTR As Integer
    Public Property TTRMatchCount As Integer
    Public Property LizenzNr As Integer
    Public Property Klassement As String
    Public Property Geschlecht As Integer
    Public Property Geburtsjahr As Integer
    Public Property Fremd As Boolean

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

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
