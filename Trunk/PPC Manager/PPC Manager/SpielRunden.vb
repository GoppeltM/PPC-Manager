Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports System.Collections.ObjectModel

Public Class SpielRunden
    Inherits Stack(Of SpielRunde)

    Friend Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal runden As IEnumerable(Of XElement)) As SpielRunden
        Dim SpielRunden As New SpielRunden
        Dim xRunden = From x In runden.<match>.Concat(runden.<ppc:match>).Concat(runden.<ppc:freematch>).Concat(runden.<ppc:inactiveplayer>)
                      Group By Number = Integer.Parse(Regex.Match(x.@group, "\d+\Z").Value) Into Runde = Group Order By Number, Runde.@nr Ascending
        For Each xRunde In xRunden
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

        Return xSpielRunden
    End Function

End Class


Public Class SpielRunde
    Inherits ObservableCollection(Of SpielPartie)

    Public Property AusgeschiedeneSpieler As New ObservableCollection(Of Spieler)

    Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xSpiele As IEnumerable(Of XElement)) As SpielRunde
        Dim runde As New SpielRunde
        For Each xSpielPartie In From x In xSpiele Where x.Name.LocalName = "match"
            runde.Add(SpielPartie.FromXML(spielerListe, xSpielPartie))
        Next
        Dim xFreilos = (From x In xSpiele Where x.Name = XNamespace.Get("http://www.ttc-langensteinbach.de") + "freematch").SingleOrDefault
        If xFreilos IsNot Nothing Then
            runde.Add(FreiLosSpiel.FromXML(spielerListe, xFreilos))
        End If

        For Each xSpieler In From x In xSpiele Where x.Name = XNamespace.Get("http://www.ttc-langensteinbach.de") + "inactiveplayer"
            Dim StartNummer = xSpieler.@player
            runde.AusgeschiedeneSpieler.Add((From x In spielerListe Where x.Id = StartNummer Select x).First)
        Next
        Return runde
    End Function

    Friend Function ToXML(ByVal spielRunde As Integer, nextMatchNr As Func(Of Integer)) As IEnumerable(Of XElement)
        Dim rundenName = "Runde " & spielRunde + 1
        Dim SpielRunden = From x In Me Let y = x.ToXML(nextMatchNr()) Select y

        Dim inaktiveSpieler = From x In AusgeschiedeneSpieler
                       Let El As XElement = <ppc:inactiveplayer player=<%= x.Id %> group=<%= rundenName %>/>
                       Select El
        Return SpielRunden.Concat(inaktiveSpieler)
    End Function

    Public Overrides Function ToString() As String
        Return "SpielRunde"
    End Function

End Class