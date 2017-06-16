Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class NeuesTurnierKontext
    Implements INotifyPropertyChanged

    Public Sub New()
        AddHandler Klassements.CollectionChanged, Sub()
                                                      RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Klassements"))
                                                  End Sub
    End Sub

    Public Property Turniername As String = String.Empty
    Public Property Turnierbeginn As Date = Date.Now
    Public Property Turnierende As Date = Date.Now

    Public Property Klassements As New ObservableCollection(Of NeuesKlassement)
    Private _Dateiname As String = ""
    Public Property Dateiname As String
        Get
            Return _Dateiname
        End Get
        Set(value As String)
            _Dateiname = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Dateiname"))
        End Set
    End Property
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class

Public Class NeuesKlassement
    Public Property KlassementName As String = String.Empty
    Public Property KlassementStart As Date = Date.Now
    Public Property TTRHinweis As String = String.Empty
    Public Property Typ As String = "Einzel"
End Class
