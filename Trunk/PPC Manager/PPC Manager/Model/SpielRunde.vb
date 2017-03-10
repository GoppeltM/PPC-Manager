Imports System.Collections.ObjectModel

Public Class SpielRunde
    Inherits ObservableCollection(Of SpielPartie)

    Sub Update()
        Me.OnCollectionChanged(New Specialized.NotifyCollectionChangedEventArgs(Specialized.NotifyCollectionChangedAction.Reset, Me.Items))
    End Sub



    Friend Function ToXML(ByVal spielRunde As Integer, nextMatchNr As Func(Of Integer)) As IEnumerable(Of XElement)
        Dim SpielRunden = From x In Me Let y = x.ToXML(nextMatchNr()) Select y
        Return SpielRunden
    End Function

    Public Overrides Function ToString() As String
        Return "SpielRunde"
    End Function

End Class
