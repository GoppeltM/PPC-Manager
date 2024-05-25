
Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class MainWindowContext
    Public Property Spielerliste As IList(Of SpielerInfo)
    Public Property KlassementListe As IEnumerable(Of String)
End Class

Public Class Playoff_Config

    Public Property Doc As XDocument = Nothing

    Public Sub New()
        UpdateContext(Nothing)

        InitializeComponent()
    End Sub

    Public Sub UpdateContext(doc As XDocument)
        Me.Doc = doc

        If Me.Doc Is Nothing Then
            DataContext = New MainWindowContext With {
                .Spielerliste = New List(Of SpielerInfo),
                .KlassementListe = New List(Of String)
            }
            Return
        End If

        DataContext = New MainWindowContext With {
            .KlassementListe = (From klassement In doc...<Klassement>
                                Select klassement.Value).ToList,
            .Spielerliste = LeseSpieler()
        }

    End Sub

    Public Function LeseSpieler() As IList(Of SpielerInfo)

        'Todo: need to recalculate player ordering based on match results

        Dim l As New List(Of SpielerInfo)
        Dim klassements = Doc.Root.<competition>
        For Each klassement In klassements

            Dim neuerSpieler = Function(id As String, fremd As Boolean, person As XElement) As SpielerInfo
                                   Dim s = New SpielerInfo(id) With {
                                        .Geschlecht = Integer.Parse(person.@sex),
                                        .Lizenznummer = person.Attribute("licence-nr").Value,
                                        .Nachname = person.@lastname,
                                        .Vorname = person.@firstname,
                                        .Vereinsname = person.Attribute("club-name").Value
                                    }
                                   Integer.TryParse(person.@birthyear, s.Geburtsjahr)
                                   Integer.TryParse(person.@ttr, s.TTRating)
                                   Integer.TryParse(person.Attribute("ttr-match-count")?.Value, s.TTRMatchCount)
                                   Return s
                               End Function

            For Each xmlSpieler In klassement.<players>.<player>
                Dim id = xmlSpieler.@id
                Dim person = xmlSpieler.<person>.Single
                l.Add(neuerSpieler(id, False, person))

            Next

            'ausgeschiedene Spieler:
            'For Each xmlSpieler In klassement.<players>.<ppc:player>
            'Dim id = xmlSpieler.@id
            'Dim person = xmlSpieler.<person>.Single
            'l.Add(neuerSpieler(id, True, person))
            'Next
        Next
        Return l
    End Function

End Class
