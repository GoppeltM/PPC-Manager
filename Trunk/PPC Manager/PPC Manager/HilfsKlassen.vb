Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de/">

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

    Shared Function FromXML(ByVal xSpielerListe As IEnumerable(Of XElement)) As SpielerListe
        Dim l = New SpielerListe

        For Each xSpieler In xSpielerListe.<player>
            l.Add(Spieler.FromXML(xSpieler))
        Next

        For Each xSpieler In xSpielerListe.<ppc:player>
            Dim s = Spieler.FromXML(xSpieler)
            s.Fremd = True
            l.Add(s)
        Next
        Return l
    End Function

End Class

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


    Friend Function ToXML() As IEnumerable(Of XElement)

        Dim xSpielRunden As New List(Of XElement)

        Dim current = 0
        For Each SpielRunde In Me.Reverse
            Dim CurrentSpiel = 0
            For Each Spiel In SpielRunde
                xSpielRunden.Add(Spiel.ToXML(current, CurrentSpiel))
                CurrentSpiel += 1
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
        For Each xSpielPartie In From x In xSpiele Where x.Name = "match"
            runde.Add(SpielPartie.FromXML(spielerListe, xSpielPartie))
        Next

        ' TODO: Freilosspiele und ausgeschiedene Spieler implementieren
        Dim xFreilos = (From x In xSpiele Where x.Name = XNamespace.Get("http://www.ttc-langensteinbach.de/") + "freematch").SingleOrDefault
        If xFreilos IsNot Nothing Then
            runde.Add(FreiLosSpiel.FromXML(spielerListe, xFreilos))
        End If

        For Each xSpieler In From x In xSpiele Where x.Name = XNamespace.Get("http://www.ttc-langensteinbach.de/") + "inactiveplayer"
            Dim StartNummer = xSpieler.@id
            runde.AusgeschiedeneSpieler.Add((From x In spielerListe Where x.Id = StartNummer Select x).First)
        Next
        Return runde
    End Function

    Friend Function ToXML(ByVal spielRunde As Integer) As XElement


        Return <SpielRunde>
                   <%= From x In Me Let y = x.ToXML(spielRunde + 1, 1) Select y %>
                   <%= From x In AusgeschiedeneSpieler Select <AusgeschiedenerSpieler>
                                                                  <%= x.Id %>
                                                              </AusgeschiedenerSpieler> %>
               </SpielRunde>
    End Function

    Public Overrides Function ToString() As String
        Return "SpielRunde"
    End Function

End Class


Public Class SpielPartien
    Inherits ObservableCollection(Of SpielPartie)

End Class

Public Class Vereine
    Inherits Collections.ObjectModel.Collection(Of String)

End Class


