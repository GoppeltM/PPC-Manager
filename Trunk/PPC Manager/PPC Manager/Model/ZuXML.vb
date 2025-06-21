Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class ZuXML

    Public Shared Sub AddSpieler(doc As XDocument, spieler As IEnumerable(Of SpielerInfoTurnier), altersgruppe As String, mode As Integer)

        Dim CompetitionNode = (From x In doc.Root.<competition> Where x.Attribute("ttr-remarks").Value = altersgruppe).Single
        CompetitionNode.@ppc:finalsmode = mode.ToString
        Dim SpielerListe = CompetitionNode.<players>.SingleOrDefault
        If SpielerListe Is Nothing Then
            SpielerListe = <players/>
            CompetitionNode.Add(SpielerListe)
        End If

        For Each el In spieler
            Dim person = <person
                             licence-nr=<%= el.Lizenznummer %>
                             club-name=<%= el.Vereinsname %>
                             sex=<%= el.Geschlecht %>
                             ttr-match-count=<%= el.TTRMatchCount %>
                             lastname=<%= el.Nachname %>
                             ttr=<%= el.TTRating %>
                             firstname=<%= el.Vorname %>
                             birthyear=<%= el.Geburtsjahr %>
                             ppc:anwesend=<%= True %>
                             ppc:bezahlt=<%= True %>
                             ppc:abwesend=<%= False %>
                         />

            If mode = FinalMode.Viertelfinale Then
                person.@ppc:vpos = el.Platz.ToString
            End If
            If mode = FinalMode.Halbfinale Then
                person.@ppc:hpos = el.Platz.ToString
            End If


            SpielerListe.Add(<player type="single" id=<%= el.Id %>>
                                 <%= person %>
                             </player>)
        Next

        Dim dateipfad = CType(Application.Current, Application).xmlPfad
        doc.Save(dateipfad)
        Dim BereinigtesDoc = New XDocument(doc)
        BereinigeNamespaces(BereinigtesDoc)
        Dim ClickTTPfad = IO.Path.Combine(IO.Path.GetDirectoryName(dateipfad), IO.Path.GetFileNameWithoutExtension(dateipfad) & "_ClickTT.xml")

    End Sub

    Public Shared Sub SaveXML(dateipfad As String,
                              spielregeln As SpielRegeln,
                              altersgruppe As String,
                              spielrunden As SpielRunden)
        Dim doc = XDocument.Load(dateipfad)
        Dim CompetitionNode = (From x In doc.Root.<competition> Where x.Attribute("ttr-remarks").Value = altersgruppe).Single
        CompetitionNode.@ppc:satzdifferenz = spielregeln.SatzDifferenz.ToString.ToLower
        CompetitionNode.@ppc:gewinnsätze = spielregeln.Gewinnsätze.ToString
        CompetitionNode.@ppc:sonnebornberger = spielregeln.SonneBornBerger.ToString.ToLower
        Dim spielstand = New Spielstand(spielregeln.Gewinnsätze)
        Dim runden = ToXML(spielrunden, spielstand)
        CompetitionNode.<matches>.Remove()
        CompetitionNode.Add(<matches>
                                <%= runden %>
                            </matches>)

        doc.Save(dateipfad)
        Dim BereinigtesDoc = New XDocument(doc)
        BereinigeNamespaces(BereinigtesDoc)
        FixXmlIssues(BereinigtesDoc)
        Dim ClickTTPfad = IO.Path.Combine(IO.Path.GetDirectoryName(dateipfad), IO.Path.GetFileNameWithoutExtension(dateipfad) & "_ClickTT.xml")

        BereinigtesDoc.Save(ClickTTPfad)
    End Sub

    Public Shared Function ToXML(runden As SpielRunden, spielstand As ISpielstand) As IEnumerable(Of XElement)
        Dim xSpielRunden As New List(Of XElement)

        For Each SpielRunde In runden.Reverse
            Dim matchNr = 0
            Dim partienSortiert = SpielRunde.Where(Function(partie) TypeOf partie IsNot FreiLosSpiel) _
                                        .Concat(SpielRunde.Where(Function(partie) TypeOf partie Is FreiLosSpiel))
            Dim rundeSortiert As New SpielRunde(partienSortiert)

            For Each match In From x In rundeSortiert
                              Let y = PartieZuXML(x, spielstand, Function() As Integer
                                                                     matchNr += 1
                                                                     Return matchNr
                                                                 End Function()) Select y
                xSpielRunden.Add(match)
            Next
        Next

        For Each i In Enumerable.Range(0, runden.Count)
            Dim runde = runden(i)
            Dim rundenname = 0
            If runde.Count > 0 Then
                Dim match = runde(0)
                rundenname = Integer.Parse(match.RundenName.Substring(match.RundenName.IndexOf(" ")))
            End If

            For Each x In runde.AusgeschiedeneSpielerIDs
                xSpielRunden.Add(<ppc:inactiveplayer player=<%= x %> group=<%= rundenname %>/>)
            Next
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

    ' Function that fixes the XML issues
    Public Shared Sub FixXmlIssues(xDoc As XDocument)
        ' Dictionary to track existing player IDs and corresponding internal-nr
        Dim playerIdDict As New Dictionary(Of String, String)()

        ' Step 1: Collect all existing player IDs and their internal-nr
        Dim players As IEnumerable(Of XElement) = xDoc.Descendants("player")
        For Each player As XElement In players
            Dim playerId As String = player.Attribute("id").Value
            Dim person As XElement = player.Element("person")

            If person IsNot Nothing AndAlso person.Attribute("internal-nr") IsNot Nothing Then
                Dim internalNr As String = person.Attribute("internal-nr").Value

                ' Store the internal-nr if not already in the dictionary
                If Not playerIdDict.ContainsKey(playerId) Then
                    playerIdDict(playerId) = internalNr
                End If
            End If
        Next

        ' Dictionary to track existing player IDs and corresponding internal-nr
        Dim playerOccurenceDict As New Dictionary(Of String, Integer)()

        Dim competitions As IEnumerable(Of XElement) = xDoc.Descendants("competition")
        For Each competition As XElement In competitions

            ' Step 2: Process players within the competition
            Dim playersComp As IEnumerable(Of XElement) = competition.Descendants("player")

            ' Step 2: Ensure unique IDs for all players and copy internal-nr where necessary
            For Each player As XElement In playersComp
                Dim playerId As String = player.Attribute("id").Value


                ' If there are multiple players with the same ID, resolve duplicates
                If playerOccurenceDict.ContainsKey(playerId) Then
                    Dim newPlayerId As String = playerId & "_" & playerOccurenceDict(playerId).ToString()
                    player.SetAttributeValue("id", newPlayerId)

                    playerOccurenceDict(playerId) = playerOccurenceDict(playerId) + 1

                    ' Step 3: Copy internal-nr from the dictionary if not present
                    Dim person As XElement = player.Element("person")
                    If person IsNot Nothing AndAlso person.Attribute("internal-nr") Is Nothing Then
                        Dim internalNr As String = playerIdDict(playerId) ' Get the internal-nr from the dictionary
                        person.SetAttributeValue("internal-nr", internalNr)
                    End If
                Else
                    playerOccurenceDict(playerId) = 1
                End If
            Next


            ' Step 3: Update player references in matches within the competition
            Dim matches As IEnumerable(Of XElement) = competition.Descendants("match")
            For Each match As XElement In matches
                ' Update player-a reference if necessary
                Dim playerA As String = match.Attribute("player-a").Value
                If playerOccurenceDict.ContainsKey(playerA) AndAlso playerOccurenceDict(playerA) > 1 Then
                    match.SetAttributeValue("player-a", playerA & "_" & playerOccurenceDict(playerA) - 1)
                End If

                ' Update player-b reference if necessary
                Dim playerB As String = match.Attribute("player-b").Value
                If playerOccurenceDict.ContainsKey(playerB) AndAlso playerOccurenceDict(playerB) > 1 Then
                    match.SetAttributeValue("player-b", playerB & "_" & playerOccurenceDict(playerB) - 1)
                End If
            Next

        Next

    End Sub



    Shared Function PartieZuXML(partie As SpielPartie, spielstand As ISpielstand, matchNr As Integer) As XElement
        If TypeOf partie Is FreiLosSpiel Then
            Dim freilos = CType(partie, FreiLosSpiel)
            Return <ppc:freematch player=<%= freilos.SpielerLinks.Id %> group=<%= freilos.RundenName %>
                       scheduled=<%= freilos.ZeitStempel.ToString("yyyy-MM-dd HH:mm") %>/>
        End If
        Dim SätzeLinks = spielstand.MeineGewonnenenSätze(partie, partie.SpielerLinks)
        Dim SätzeRechts = spielstand.MeineGewonnenenSätze(partie, partie.SpielerRechts)

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
