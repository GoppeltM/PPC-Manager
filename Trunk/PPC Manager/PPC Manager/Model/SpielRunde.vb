Imports System.Collections.ObjectModel

Public Class SpielRunde
    Inherits ObservableCollection(Of SpielPartie)

    Sub Update()
        OnCollectionChanged(New Specialized.NotifyCollectionChangedEventArgs(Specialized.NotifyCollectionChangedAction.Reset, Items))
    End Sub



    Public Overrides Function ToString() As String
        Return "SpielRunde"
    End Function

End Class
