Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class ZuXML


    Public Shared Sub SaveXML(dateipfad As String, spielregeln As SpielRegeln, altersgruppe As String, spielrunden As SpielRunden)
        Dim doc = XDocument.Load(dateipfad)
        Dim CompetitionNode = (From x In doc.Root.<competition> Where x.Attribute("age-group").Value = altersgruppe).Single
        CompetitionNode.@ppc:satzdifferenz = spielregeln.SatzDifferenz.ToString.ToLower
        CompetitionNode.@ppc:gewinnsätze = spielregeln.Gewinnsätze.ToString
        CompetitionNode.@ppc:sonnebornberger = spielregeln.SonneBornBerger.ToString.ToLower
        Dim runden = ToXML(spielrunden)
        CompetitionNode.<matches>.Remove()
        CompetitionNode.Add(<matches>
                                <%= runden %>
                            </matches>)

        doc.Save(dateipfad)
        Dim BereinigtesDoc = New XDocument(doc)
        BereinigeNamespaces(BereinigtesDoc)
        Dim ClickTTPfad = IO.Path.Combine(IO.Path.GetDirectoryName(dateipfad), IO.Path.GetFileNameWithoutExtension(dateipfad) & "_ClickTT.xml")

        BereinigtesDoc.Save(ClickTTPfad)
    End Sub

    Public Shared Function ToXML(runden As SpielRunden) As IEnumerable(Of XElement)

        Dim xSpielRunden As New List(Of XElement)

        For Each SpielRunde In runden.Reverse
            Dim matchNr = 0
            For Each Match In From x In SpielRunde
                              Let y = PartieZuXML(x, Function() As Integer
                                                         matchNr += 1
                                                         Return matchNr
                                                     End Function()) Select y
                xSpielRunden.Add(Match)
            Next
        Next

        For Each s In runden.AusgeschiedeneSpieler
            xSpielRunden.Add(<ppc:inactiveplayer player=<%= s.Spieler.Id %> group=<%= s.Runde %>/>)
        Next

        Return xSpielRunden
    End Function

    ''' <summary>
    ''' Notwendig für Import ins ClickTT
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub BereinigeNamespaces(doc As XDocument)
        Dim NodesToRemove = From x In doc.Root.Descendants Where x.Name.NamespaceName = "http://www.ttc-langensteinbach.de"

        Dim AttributesToRemove = From x In doc.Root.Descendants
                                 From y In x.Attributes
                                 Where y.Name.NamespaceName = "http://www.ttc-langensteinbach.de" Or
                                 y.Value = "http://www.ttc-langensteinbach.de" Select y

        For Each attr In AttributesToRemove.ToList
            attr.Remove()
        Next

        For Each node In NodesToRemove.ToList
            node.Remove()
        Next

    End Sub


    Shared Function PartieZuXML(partie As SpielPartie, matchNr As Integer) As XElement
        If TypeOf partie Is FreiLosSpiel Then
            Dim freilos = CType(partie, FreiLosSpiel)
            Return <ppc:freematch player=<%= freilos.SpielerLinks.Id %> group=<%= freilos.RundenName %>
                       scheduled=<%= freilos.ZeitStempel.ToString("yyyy-MM-dd HH:mm") %>/>
        End If
        Dim SätzeLinks = partie.MeineGewonnenenSätze(partie.SpielerLinks).Count
        Dim SätzeRechts = partie.MeineGewonnenenSätze(partie.SpielerRechts).Count

        Dim GewonnenLinks = 0
        Dim GewonnenRechts = 0
        If (SätzeLinks > SätzeRechts) Then GewonnenLinks = 1
        If (SätzeLinks < SätzeRechts) Then GewonnenRechts = 1

        Dim SatzReihe = Function(ident As String, ergebnisse As IEnumerable(Of Integer)) As IEnumerable(Of XAttribute)
                            Dim attributes = (From x In Enumerable.Range(1, 7) Select New XAttribute(String.Format("set-{0}-{1}", ident, x), 0)).ToList
                            Dim current = 0
                            For Each ergebnis In ergebnisse
                                attributes(current).Value = ergebnis.ToString
                                current += 1
                            Next
                            Return attributes
                        End Function

        Dim node = <match player-a=<%= partie.SpielerLinks.Id %> player-b=<%= partie.SpielerRechts.Id %>
                       games-a=<%= Aggregate x In partie Into Sum(x.PunkteLinks) %>
                       games-b=<%= Aggregate x In partie Into Sum(x.PunkteRechts) %>
                       sets-a=<%= SätzeLinks %> sets-b=<%= SätzeRechts %>
                       matches-a=<%= GewonnenLinks %> matches-b=<%= GewonnenRechts %>
                       scheduled=<%= partie.ZeitStempel.ToString("yyyy-MM-dd HH:mm") %>
                       group=<%= partie.RundenName %> nr=<%= matchNr %>
                       <%= SatzReihe("a", From x In partie Select x.PunkteLinks) %>
                       <%= SatzReihe("b", From x In partie Select x.PunkteRechts) %>
                   />

        If partie.SpielerLinks.Fremd Or partie.SpielerRechts.Fremd Then
            Dim ns = XNamespace.Get("http://www.ttc-langensteinbach.de")
            node.Name = ns + "match"
            node.Add(New XAttribute(XNamespace.Xmlns + "ppc", ns.NamespaceName))
        End If
        Return node
    End Function

End Class
