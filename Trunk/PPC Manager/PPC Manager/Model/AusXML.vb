Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class AusXML
    Public Shared Function CompetitionFromXML(dateipfad As String, node As XElement, spielRegeln As SpielRegeln) As Competition
        Dim TryBool = Function(x As String) As Boolean
                          Dim val As Boolean = False
                          Boolean.TryParse(x, val)
                          Return val
                      End Function

        Dim UnbekannterZustand = From x In node...<person> Where Not TryBool(x.@ppc:anwesend) AndAlso Not TryBool(x.@ppc:abwesend)

        If UnbekannterZustand.Any Then
            Throw New SpielDatenUnvollständigException(UnbekannterZustand.Count)
        End If

        Dim c = New Competition(spielRegeln) With
               {
                   .DateiPfad = dateipfad,
                   .StartDatum = node.Attribute("start-date").Value,
                   .Altersgruppe = node.Attribute("age-group").Value
                }
        c.SpielerListe = SpielerListeFromXML(node.<players>, c.SpielRunden, spielRegeln)
        Dim ParsedSpielrunden = SpielRundenFromXML(c.SpielerListe, node.<matches>.SingleOrDefault, spielRegeln.Gewinnsätze)
        For Each runde In ParsedSpielrunden.Reverse
            c.SpielRunden.Push(runde)
        Next
        c.SpielRunden.AusgeschiedeneSpieler = ParsedSpielrunden.AusgeschiedeneSpieler
        Return c
    End Function

    Shared Function SpielerListeFromXML(ByVal xSpielerListe As IEnumerable(Of XElement), spielRunden As SpielRunden, spielRegeln As SpielRegeln) As SpielerListe
        Dim l = New SpielerListe

        For Each xSpieler In xSpielerListe.<player>
            l.Add(SpielerFromXML(xSpieler, spielRunden, spielRegeln))
        Next

        For Each xSpieler In xSpielerListe.<ppc:player>
            Dim s = SpielerFromXML(xSpieler, spielRunden, spielRegeln)
            s.Fremd = True
            l.Add(s)
        Next
        Return l
    End Function

    Public Shared Function SpielerFromXML(ByVal spielerNode As XElement, spielRunden As SpielRunden, spielRegeln As SpielRegeln) As Spieler
        Dim spieler As New Spieler(spielRunden, spielRegeln)
        With spieler
            .Id = spielerNode.@id
            Dim ppc = spielerNode.GetNamespaceOfPrefix("ppc")
            .Fremd = ppc IsNot Nothing AndAlso ppc.NamespaceName = "http://www.ttc-langensteinbach.de/"
            spielerNode = spielerNode.<person>.First
            .Vorname = spielerNode.@firstname
            .Nachname = spielerNode.@lastname
            .TTRMatchCount = CInt(spielerNode.Attribute("ttr-match-count").Value)
            .Geschlecht = CInt(spielerNode.@sex)
            .Geburtsjahr = CInt(spielerNode.@birthyear)
            .Vereinsname = spielerNode.Attribute("club-name").Value
            .TTRating = CInt(spielerNode.@ttr)
            .Lizenznummer = CInt(spielerNode.Attribute("licence-nr").Value)
        End With
        Return spieler
    End Function

    Public Shared Function SpielRundenFromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal matchesKnoten As XElement, gewinnsätze As Integer) As SpielRunden
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
            SpielRunden.Push(SpielRundeFromXML(spielerListe, xRunde.Runde, gewinnsätze))
        Next

        Return SpielRunden
    End Function

    Public Shared Function SpielRundeFromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xSpiele As IEnumerable(Of XElement), gewinnsätze As Integer) As SpielRunde
        Dim runde As New SpielRunde
        For Each xSpielPartie In From x In xSpiele Where x.Name.LocalName = "match"
            runde.Add(SpielPartieFromXML(spielerListe, xSpielPartie, gewinnsätze))
        Next
        Dim xFreilos = (From x In xSpiele Where x.Name = XNamespace.Get("http://www.ttc-langensteinbach.de") + "freematch").SingleOrDefault
        If xFreilos IsNot Nothing Then
            runde.Add(FreilosFromXML(spielerListe, xFreilos, gewinnsätze))
        End If

        Return runde
    End Function

    Shared Function FreilosFromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xFreilosSpiel As XElement, gewinnsätze As Integer) As FreiLosSpiel
        Dim spieler = (From x In spielerListe Where x.Id = xFreilosSpiel.@player Select x).First
        Return New FreiLosSpiel(xFreilosSpiel.@group, spieler, gewinnsätze)
    End Function

    Shared Function SpielPartieFromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xSpielPartie As XElement, gewinnsätze As Integer) As SpielPartie
        Dim spielerA = (From x In spielerListe Where x.Id = xSpielPartie.Attribute("player-a").Value Select x).First
        Dim spielerB = (From x In spielerListe Where x.Id = xSpielPartie.Attribute("player-b").Value Select x).First

        Dim partie As New SpielPartie(xSpielPartie.@group, spielerA, spielerB, gewinnsätze)

        Dim SätzeA = From x In xSpielPartie.Attributes Where x.Name.LocalName.Contains("set-a") Order By x.Name.LocalName Ascending

        Dim SätzeB = From x In xSpielPartie.Attributes Where x.Name.LocalName.Contains("set-b") Order By x.Name.LocalName Ascending

        For Each Satz In SätzeA.Zip(SätzeB, Function(x, y) New Satz With {.PunkteLinks = CInt(x.Value), .PunkteRechts = CInt(y.Value)}) _
            .Where(Function(s) s.PunkteLinks <> 0 Or s.PunkteRechts <> 0)
            partie.Add(Satz)
        Next

        partie.ZeitStempel = Date.Parse(xSpielPartie.@scheduled)

        Return partie
    End Function

    Public Shared Function CompetitionFromXML(dateiPfad As String, doc As XDocument, gruppe As String, spielRegeln As SpielRegeln) As Competition
        Dim competitionXML = (From x In doc.Root.<competition> Where x.Attribute("age-group").Value = gruppe).Single
        ' Syntax Checks

        Return CompetitionFromXML(dateiPfad, competitionXML, spielRegeln)
    End Function
End Class
