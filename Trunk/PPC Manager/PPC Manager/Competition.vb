Public Class Competition

    Property StartDatum As String
    Property ttrRemarks As String
    Property Altersgruppe As String
    Property Typ As String
    Property SpielerListe As New SpielerListe

    Public Shared Function FromXML(node As XElement) As Competition
        Return New Competition With
               {
                    .StartDatum = node.Attribute("start-date").Value,
                    .ttrRemarks = node.Attribute("ttr-remarks").Value,
                    .Altersgruppe = node.Attribute("age-group").Value,
                    .Typ = node.Attribute("type").Value,
                    .SpielerListe = SpielerListe.FromXML(node.<players>.<player>)
                }
    End Function

    Function ToXML() As XElement
        Return <competition start-date=<%= StartDatum %> ttr-remarks=<%= ttrRemarks %>
                   age-group=<%= Altersgruppe %> type=<%= Typ %>>
                   <players>
                       <%= From spieler In SpielerListe Select Spieler = spieler.ToXML() %>
                   </players>
               </competition>
    End Function

End Class
