Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports System.Collections.ObjectModel


Public Class SpielRunden
    Inherits Stack(Of SpielRunde)

    <DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>
    Public Property AusgeschiedeneSpieler As New ObservableCollection(Of Ausgeschieden)

    Public Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal matchesKnoten As XElement) As SpielRunden
        Dim SpielRunden As New SpielRunden
        If matchesKnoten Is Nothing Then
            Return SpielRunden
        End If
        Dim xSpielPartien = From x In matchesKnoten.Elements.Except(matchesKnoten.<ppc:inactiveplayer>)
                      Group By x.@group Into Runde = Group


        For Each AusgeschiedenerSpieler In matchesKnoten.<ppc:inactiveplayer>
            Dim StartNummer = AusgeschiedenerSpieler.@player
            Dim ausgeschieden As New Ausgeschieden
            ausgeschieden.Spieler = (From x In spielerListe Where x.Id = StartNummer Select x).Single
            ausgeschieden.Runde = Integer.Parse(AusgeschiedenerSpieler.@group)
            SpielRunden.AusgeschiedeneSpieler.Add(ausgeschieden)
        Next

        For Each xRunde In xSpielPartien.Reverse
            SpielRunden.Push(SpielRunde.FromXML(spielerListe, xRunde.Runde))
        Next

        Return SpielRunden
    End Function


    Public Function ToXML() As IEnumerable(Of XElement)

        Dim xSpielRunden As New List(Of XElement)

        Dim current = 0
        For Each SpielRunde In Me.Reverse
            Dim matchNr = 0
            For Each Match In SpielRunde.ToXML(current, Function() As Integer
                                                            matchNr += 1
                                                            Return matchNr
                                                        End Function)
                xSpielRunden.Add(Match)
            Next
            current += 1
        Next

        For Each s In AusgeschiedeneSpieler
            xSpielRunden.Add(<ppc:inactiveplayer player=<%= s.Spieler.Id %> group=<%= s.Runde %>/>)
        Next

        Return xSpielRunden
    End Function

End Class


Public Class SpielRunde
    Inherits ObservableCollection(Of SpielPartie)

    Sub Update()
        Me.OnCollectionChanged(New Specialized.NotifyCollectionChangedEventArgs(Specialized.NotifyCollectionChangedAction.Reset, Me.Items))
    End Sub

    Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xSpiele As IEnumerable(Of XElement)) As SpielRunde
        Dim runde As New SpielRunde
        For Each xSpielPartie In From x In xSpiele Where x.Name.LocalName = "match"
            runde.Add(SpielPartie.FromXML(spielerListe, xSpielPartie))
        Next
        Dim xFreilos = (From x In xSpiele Where x.Name = XNamespace.Get("http://www.ttc-langensteinbach.de") + "freematch").SingleOrDefault
        If xFreilos IsNot Nothing Then
            runde.Add(FreiLosSpiel.FromXML(spielerListe, xFreilos))
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

Public Class Ausgeschieden
    Property Spieler As Spieler
    Property Runde As Integer

End Class