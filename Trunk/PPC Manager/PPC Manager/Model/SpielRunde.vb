Imports System.Collections.ObjectModel

Public Class SpielRunde
    Inherits ObservableCollection(Of SpielPartie)

    Sub Update()
        Me.OnCollectionChanged(New Specialized.NotifyCollectionChangedEventArgs(Specialized.NotifyCollectionChangedAction.Reset, Me.Items))
    End Sub

    Public Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xSpiele As IEnumerable(Of XElement), gewinnsätze As Integer) As SpielRunde
        Dim runde As New SpielRunde
        For Each xSpielPartie In From x In xSpiele Where x.Name.LocalName = "match"
            runde.Add(SpielPartie.FromXML(spielerListe, xSpielPartie, gewinnsätze))
        Next
        Dim xFreilos = (From x In xSpiele Where x.Name = XNamespace.Get("http://www.ttc-langensteinbach.de") + "freematch").SingleOrDefault
        If xFreilos IsNot Nothing Then
            runde.Add(FreiLosSpiel.FromXML(spielerListe, xFreilos, gewinnsätze))
        End If


        Return runde
    End Function

    Friend Function ToXML(ByVal spielRunde As Integer, nextMatchNr As Func(Of Integer)) As IEnumerable(Of XElement)
        Dim SpielRunden = From x In Me Let y = x.ToXML(nextMatchNr()) Select y
        Return SpielRunden
    End Function

    Public Overrides Function ToString() As String
        Return "SpielRunde"
    End Function

End Class
