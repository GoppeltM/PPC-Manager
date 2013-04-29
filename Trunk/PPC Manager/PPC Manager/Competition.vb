Public Class Competition

    Property StartDatum As String    
    Property Altersgruppe As String    
    Property SpielerListe As New SpielerListe
    Property DateiPfad As String
    Property SatzDifferenz As Boolean
    Property AutoSaveAn As Boolean
    Property SpielRunden As New SpielRunden
    Property Gewinnsätze As Integer

    Public Shared Function FromXML(dateipfad As String, node As XElement, satzzahl As Double, satzDifferenz As Boolean?) As Competition
        Dim c = New Competition With
               {
                   .Gewinnsätze = Convert.ToInt32(satzzahl),
                   .SatzDifferenz = Convert.ToBoolean(satzDifferenz),
                    .DateiPfad = dateipfad,
                    .StartDatum = node.Attribute("start-date").Value,
                    .Altersgruppe = node.Attribute("age-group").Value,
                    .SpielerListe = SpielerListe.FromXML(node.<players>)
                }
        For Each runde In SpielRunden.FromXML(c.SpielerListe, node.<matches>)
            c.SpielRunden.Push(runde)
        Next
        Return c
    End Function

    Public Shared Function FromXML(dateiPfad As String, gruppe As String, satzzahl As Double, satzDifferenz As Boolean?) As Competition
        Dim doc = XDocument.Load(dateiPfad)
        Dim competitionXML = (From x In doc.Root.<competition> Where x.Attribute("age-group").Value = gruppe).Single
        Return FromXML(dateiPfad, competitionXML, satzzahl, satzDifferenz)
    End Function

    
    Public Sub Save()
        Dim doc = XDocument.Load(DateiPfad)
        Dim CompetitionNode = (From x In doc.Root.<competition> Where x.Attribute("age-group").Value = Altersgruppe).Single
        Dim runden = SpielRunden.ToXML
        CompetitionNode.<matches>.Remove()
        CompetitionNode.Add(<matches>
                                <%= runden %>
                            </matches>)

        doc.Save(DateiPfad)
    End Sub


End Class
