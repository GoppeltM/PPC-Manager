Public Class Spielstand
    Implements ISpielstand
    Private ReadOnly _GewinnSätze As Integer

    Public Sub New(gewinnsätze As Integer)
        _GewinnSätze = gewinnsätze
    End Sub

    Public Function IstAbgeschlossen(spielPartie As SpielPartie) As Boolean Implements ISpielstand.IstAbgeschlossen
        If TypeOf spielPartie Is FreiLosSpiel Then
            Return True
        End If
        Dim AbgeschlosseneSätzeLinks = Aggregate x In spielPartie Where SatzLinksGewonnen(x) Into Count()

        Dim AbgeschlosseneSätzeRechts = Aggregate x In spielPartie Where SatzRechtsGewonnen(x) Into Count()

        Return Math.Max(AbgeschlosseneSätzeLinks, AbgeschlosseneSätzeRechts) >= _GewinnSätze
    End Function

    Public Function MeineVerlorenenSätze(partie As SpielPartie, ich As SpielerInfo) As IList(Of Satz) Implements ISpielstand.MeineVerlorenenSätze
        Dim verlorenLinks = From x In partie Where SatzAbgeschlossen(x) AndAlso x.PunkteRechts > x.PunkteLinks
                            Select x

        Dim verlorenRechts = From x In partie Where SatzAbgeschlossen(x) AndAlso x.PunkteLinks > x.PunkteRechts
                             Select x

        If partie.SpielerLinks = ich Then
            Return verlorenLinks.ToList
        Else
            Return verlorenRechts.ToList
        End If
    End Function

    Public Function MeineGewonnenenSätze(partie As SpielPartie, ByVal ich As SpielerInfo) As Integer Implements ISpielstand.MeineGewonnenenSätze
        If TypeOf partie Is FreiLosSpiel Then
            Dim sätze = From x In Enumerable.Range(0, _GewinnSätze)
                        Select New Satz() With {.PunkteLinks = 11}
            Return sätze.Count
        End If
        Dim gewonnenLinks = From x In partie Where SatzLinksGewonnen(x)

        Dim gewonnenRechts = From x In partie Where (SatzRechtsGewonnen(x))

        If partie.SpielerLinks = ich Then
            Return gewonnenLinks.Count
        Else
            Return gewonnenRechts.Count
        End If
    End Function

    Private Function SatzLinksGewonnen(s As Satz) As Boolean
        Return SatzAbgeschlossen(s) AndAlso s.PunkteLinks > s.PunkteRechts
    End Function

    Private Function SatzRechtsGewonnen(s As Satz) As Boolean
        Return SatzAbgeschlossen(s) AndAlso s.PunkteLinks < s.PunkteRechts
    End Function

    Private GewinnPunkte As Integer = My.Settings.GewinnPunkte

    Private Function SatzAbgeschlossen(s As Satz) As Boolean
        Return Math.Max(s.PunkteLinks, s.PunkteRechts) >= GewinnPunkte _
            AndAlso Math.Abs(s.PunkteLinks - s.PunkteRechts) >= 2
    End Function

End Class
