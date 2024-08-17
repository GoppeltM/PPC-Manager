Imports System.Collections.ObjectModel

Public Class SpielRunde
    Inherits ObservableCollection(Of SpielPartie)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(partien As IEnumerable(Of SpielPartie))
        MyBase.New(partien)
    End Sub

    Sub Update()
        OnCollectionChanged(New Specialized.NotifyCollectionChangedEventArgs(Specialized.NotifyCollectionChangedAction.Reset, Items))
    End Sub

    Public ReadOnly Property AusgeschiedeneSpielerIDs As ICollection(Of String) = New List(Of String)

    Public Overrides Function ToString() As String
        Return "SpielRunde"
    End Function

End Class
