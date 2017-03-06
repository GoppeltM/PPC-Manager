Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class Speicher
    Implements ISpeicher

    Private ReadOnly _Pfad As String
    Public Sub New(pfad As String)
        _Pfad = pfad
    End Sub

    Public ReadOnly Property KlassementNamen As IEnumerable(Of String) Implements ISpeicher.KlassementNamen
        Get
            Dim doc = XDocument.Load(_Pfad)
            Return (From x In doc.Root.<competition> Select x.Attribute("age-group").Value).ToList
        End Get
    End Property

    Public Sub Speichere(veränderung As Veränderung) Implements ISpeicher.Speichere
        Dim doc = XDocument.Load(_Pfad)
        Dim klassements = doc.Root.<competition>
        veränderung(klassements)
        doc.Save(_Pfad)
    End Sub


    Public Function LeseSpieler() As IList(Of SpielerInfo)
        Dim l As New List(Of SpielerInfo)
        Dim doc = XDocument.Load(_Pfad)
        Dim klassements = doc.Root.<competition>
        For Each klassement In klassements

            Dim neuerSpieler = Function(id As String, fremd As Boolean, person As XElement) As SpielerInfo
                                   Dim s = New SpielerInfo With {
                                        .ID = id,
                                        .Fremd = fremd,
                                        .Geschlecht = CInt(person.@sex),
                                        .Klassement = klassement.Attribute("age-group").Value,
                                        .LizenzNr = CInt(person.Attribute("licence-nr").Value),
                                        .Nachname = person.@lastname,
                                        .Vorname = person.@firstname,
                                        .TTR = CInt(person.@ttr),
                                        .TTRMatchCount = CInt(person.Attribute("ttr-match-count")),
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
                l.Add(neuerSpieler(id, False, person))
            Next
            For Each xmlSpieler In klassement.<players>.<ppc:player>
                Dim id = xmlSpieler.@id
                Dim person = xmlSpieler.<person>.Single
                l.Add(neuerSpieler(id, True, person))
            Next
        Next
        Return l
    End Function

End Class
