Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class Competition

    Property StartDatum As String
    Property Altersgruppe As String

    Property SpielRunden As New SpielRunden
    Property SpielerListe As New SpielerListe
    Property DateiPfad As String

    Private _SpielRegeln As SpielRegeln
    ReadOnly Property SpielRegeln As SpielRegeln
        Get
            Return _SpielRegeln
        End Get
    End Property

    Public Sub New(spielRegeln As SpielRegeln)
        _SpielRegeln = spielRegeln
    End Sub


    Public Shared Function FromXML(dateipfad As String, node As XElement, spielRegeln As SpielRegeln) As Competition        
        Dim c = New Competition(spielRegeln) With
               {
                   .DateiPfad = dateipfad,
                   .StartDatum = node.Attribute("start-date").Value,
                   .Altersgruppe = node.Attribute("age-group").Value
                }
        c.SpielerListe = SpielerListe.FromXML(node.<players>, c.SpielRunden, spielRegeln)
        Dim ParsedSpielrunden = SpielRunden.FromXML(c.SpielerListe, node.<matches>.SingleOrDefault, spielRegeln.Gewinnsätze)
        For Each runde In ParsedSpielrunden.Reverse
            c.SpielRunden.Push(runde)
        Next
        c.SpielRunden.AusgeschiedeneSpieler = ParsedSpielrunden.AusgeschiedeneSpieler
        Return c
    End Function

    Public Shared Function FromXML(dateiPfad As String, gruppe As String, spielRegeln As SpielRegeln) As Competition
        Dim doc = XDocument.Load(dateiPfad)
        Dim competitionXML = (From x In doc.Root.<competition> Where x.Attribute("age-group").Value = gruppe).Single
        ' Syntax Checks

        Dim TryBool = Function(x As String) As Boolean
                          Dim val As Boolean = False
                          Boolean.TryParse(x, val)
                          Return val
                      End Function

        Dim UnbekannterZustand = From x In competitionXML...<person> Where Not TryBool(x.@ppc:anwesend) AndAlso Not TryBool(x.@ppc:abwesend)

        If UnbekannterZustand.Any Then
            MessageBox.Show(String.Format("Es gibt noch {0} Spieler dessen Anwesenheitsstatus unbekannt ist. Bitte korrigieren bevor das Turnier beginnt.", UnbekannterZustand.Count), _
                            "Spieldaten unvollständig", MessageBoxButton.OK, MessageBoxImage.Error)
            Application.Current.Shutdown()
        End If
        Return FromXML(dateiPfad, competitionXML, spielRegeln)
    End Function


    Public Sub SaveXML()
        Dim doc = XDocument.Load(DateiPfad)
        Dim CompetitionNode = (From x In doc.Root.<competition> Where x.Attribute("age-group").Value = Altersgruppe).Single
        CompetitionNode.@ppc:satzdifferenz = SpielRegeln.SatzDifferenz.ToString.ToLower
        CompetitionNode.@ppc:gewinnsätze = SpielRegeln.Gewinnsätze.ToString
        CompetitionNode.@ppc:sonnebornberger = SpielRegeln.SonneBornBerger.ToString.ToLower
        Dim runden = SpielRunden.ToXML
        CompetitionNode.<matches>.Remove()
        CompetitionNode.Add(<matches>
                                <%= runden %>
                            </matches>)

        doc.Save(DateiPfad)
        Dim BereinigtesDoc = New XDocument(doc)
        BereinigeNamespaces(BereinigtesDoc)
        Dim ClickTTPfad = IO.Path.Combine(IO.Path.GetDirectoryName(DateiPfad), IO.Path.GetFileNameWithoutExtension(DateiPfad) & "_ClickTT.xml")

        BereinigtesDoc.Save(ClickTTPfad)
    End Sub

    ''' <summary>
    ''' Notwendig für Import ins ClickTT
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub BereinigeNamespaces(doc As XDocument)
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

    Public Sub SaveExcel()
        Dim spieler = SpielerListe.ToList
        ExcelInterface.CreateFile(ExcelPfad, spieler, Me)
    End Sub

    Public ReadOnly Property ExcelPfad As String
        Get
            Dim DateiName = IO.Path.GetFileNameWithoutExtension(DateiPfad)
            DateiName &= "_" & Altersgruppe
            For Each c In IO.Path.GetInvalidFileNameChars
                DateiName = DateiName.Replace(c, "_"c)
            Next
            DateiName = DateiName.Replace(" "c, "_"c)
            DateiName &= ".xlsx"
            Dim Unterpfad = IO.Path.Combine(IO.Path.GetDirectoryName(DateiPfad), "Protokolle")
            DateiName = IO.Path.Combine(Unterpfad, DateiName)
            Return DateiName
        End Get
    End Property


End Class
