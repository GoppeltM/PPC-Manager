Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports System.Collections.ObjectModel


Public Class SpielRunden
    Inherits Stack(Of SpielRunde)

    <DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>
    Public Property AusgeschiedeneSpieler As New ObservableCollection(Of Ausgeschieden)

    Public Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal matchesKnoten As XElement, gewinnsätze As Integer) As SpielRunden
        Dim SpielRunden As New SpielRunden
        If matchesKnoten Is Nothing Then
            Return SpielRunden
        End If


        For Each AusgeschiedenerSpieler In matchesKnoten.<ppc:inactiveplayer>
            Dim StartNummer = AusgeschiedenerSpieler.@player
            Dim ausgeschieden As New Ausgeschieden
            ausgeschieden.Spieler = (From x In spielerListe Where x.Id = StartNummer Select x).Single
            ausgeschieden.Runde = Integer.Parse(AusgeschiedenerSpieler.@group)
            SpielRunden.AusgeschiedeneSpieler.Add(ausgeschieden)
        Next

        Dim xSpielPartien = From x In matchesKnoten.Elements.Except(matchesKnoten.<ppc:inactiveplayer>)
                      Group By x.@group Into Runde = Group Order By Date.Parse(Runde.First.@scheduled, Globalization.CultureInfo.GetCultureInfo("de")) Ascending

        For Each xRunde In xSpielPartien
            SpielRunden.Push(SpielRunde.FromXML(spielerListe, xRunde.Runde, gewinnsätze))
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



