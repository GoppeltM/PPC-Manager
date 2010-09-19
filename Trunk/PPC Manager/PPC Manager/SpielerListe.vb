Imports System.Collections.ObjectModel

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

    Public Sub New()
        MyBase.New()
    End Sub

    Private Sub Personen_CollectionChanged(ByVal sender As Object, ByVal e As System.Collections.Specialized.NotifyCollectionChangedEventArgs) Handles Me.CollectionChanged
        Console.WriteLine()
    End Sub

    Private Sub Personen_PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        Console.WriteLine()
    End Sub


End Class
