Imports StartlistenEditor
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class UngültigeSpielerXmlException
    Inherits FormatException
    Public Sub New(spieler As XElement, inner As Exception)
        MyBase.New("Konnte folgenden Spieler nicht deserialisieren:" + Environment.NewLine + spieler.ToString, inner)
    End Sub

End Class

Public Class Speicher
    Implements ISpeicher

    Private ReadOnly _Dateisystem As IDateisystem

    Public Sub New(dateisystem As IDateisystem)
        _Dateisystem = dateisystem
    End Sub

    Public ReadOnly Property KlassementNamen As IEnumerable(Of String) Implements ISpeicher.KlassementNamen
        Get
            Dim doc = _Dateisystem.LadeXml()
            Return (From x In doc.Root.<competition> Select x.Attribute("age-group").Value).ToList
        End Get
    End Property

    Public Sub Speichere(veränderung As Veränderung) Implements ISpeicher.Speichere
        Dim doc = _Dateisystem.LadeXml
        Dim klassements = doc.Root.<competition>
        veränderung(klassements)
        _Dateisystem.SpeichereXml(doc)
    End Sub


    Public Function LeseSpieler() As IList(Of SpielerInfo)
        Dim l As New List(Of SpielerInfo)
        Dim doc = _Dateisystem.LadeXml
        Dim klassements = doc.Root.<competition>
        For Each klassement In klassements

            Dim neuerSpieler = Function(id As String, fremd As Boolean, person As XElement) As SpielerInfo
                                   Dim s = New SpielerInfo With {
                                        .ID = id,
                                        .Fremd = fremd,
                                        .Geschlecht = Integer.Parse(person.@sex),
                                        .Klassement = klassement.Attribute("age-group").Value,
                                        .LizenzNr = Long.Parse(person.Attribute("licence-nr").Value),
                                        .Nachname = person.@lastname,
                                        .Vorname = person.@firstname,
                                        .TTR = Integer.Parse(person.@ttr),
                                        .TTRMatchCount = Integer.Parse(person.Attribute("ttr-match-count").Value),
                                        .Verein = person.Attribute("club-name").Value
                                    }
                                   Boolean.TryParse(person.@ppc:abwesend, s.Abwesend)
                                   Boolean.TryParse(person.@ppc:anwesend, s.Anwesend)
                                   Boolean.TryParse(person.@ppc:bezahlt, s.Bezahlt)
                                   Integer.TryParse(person.@birthyear, s.Geburtsjahr)
                                   Return s
                               End Function

            For Each xmlSpieler In klassement.<players>.<player>
                Dim id = xmlSpieler.@id
                Dim person = xmlSpieler.<person>.Single
                Try
                    l.Add(neuerSpieler(id, False, person))
                Catch ex As FormatException
                    Throw New UngültigeSpielerXmlException(xmlSpieler, ex)
                End Try

            Next
            For Each xmlSpieler In klassement.<players>.<ppc:player>
                Dim id = xmlSpieler.@id
                Dim person = xmlSpieler.<person>.Single
                Try
                    l.Add(neuerSpieler(id, True, person))
                Catch ex As FormatException
                    Throw New UngültigeSpielerXmlException(xmlSpieler, ex)
                End Try

            Next
        Next
        Return l
    End Function

End Class
