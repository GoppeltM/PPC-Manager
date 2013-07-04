
Public Class PaarungsSuche

    Private Property _rundenName As String
    Public Sub New(rundenName As String)
        _rundenName = rundenName
    End Sub

    Public Function SuchePaarungen(ByVal spielerListe As List(Of Spieler), ByVal paket As Paket) As PaarungsContainer
        If spielerListe.Count < 2 Then Return Nothing

        ' Erzeugung der linken und rechten Liste. Diese dürfen durch die
        'nachfolgenden Tests NICHT verändert werden! Deshalb wird in jedem Test eine
        'Kopie dieser beiden Vektoren angelegt.
        Dim tempListe As New List(Of Spieler)(spielerListe)
        Dim mitte = tempListe.Count \ 2
        Return rekursiveUmtauschung(New List(Of Spieler), tempListe, mitte, paket)
    End Function

    Private Function rekursiveUmtauschung(ByVal anfang As List(Of Spieler), ByVal rest As List(Of Spieler), ByVal mitte As Integer, ByVal parent As Paket) As PaarungsContainer
        ' Ist nur noch ein Spieler am Ende da, muss das eine neue Kombination sein

        If rest.Count = 1 Then
            Dim kombination = New List(Of Spieler)(anfang)
            kombination.AddRange(rest)
            Dim isOk As PaarungsContainer = StandardPaarung(kombination, mitte, parent)
            If isOk IsNot Nothing Then
                Return isOk
            End If
        Else
            ' bei mehreren wähle immer einen und setze ihn mit an den Anfang
            For i = 0 To rest.Count - 1
                Dim restNeu = New List(Of Spieler)(rest)
                Dim anfangNeu = New List(Of Spieler)(anfang)
                Dim tauschSpieler = restNeu(i)
                Dim NeuerIndex = anfangNeu.Count
                If NeuerIndex >= mitte Then                    
                    Dim PartnerSpieler = anfangNeu(NeuerIndex - mitte)
                    ' Optimierung 1:
                    '  Wenn ABC <-> DEF, dann prüfe ob D > A. Wenn ja, wurde A gegen D bereits geprüft,
                    ' und alle darauf aufbauenden Kombinationen auch.
                    If parent.Compare(tauschSpieler, PartnerSpieler) > 0 Then Continue For
                    ' Optimierung 2:
                    ' Wenn ABC <-> DEF, dann prüfe ob D gegen A schonmal gespielt hat.
                    ' Wenn ja, sind eh alle darauf basierenden Kombinationen unmöglich.
                    If tauschSpieler.HatBereitsGespieltGegen(PartnerSpieler) Then Continue For
                End If
                restNeu.Remove(tauschSpieler)
                anfangNeu.Add(tauschSpieler)

                Dim isOk As PaarungsContainer = rekursiveUmtauschung(anfangNeu, restNeu, mitte, parent)

                If Not isOk Is Nothing Then Return isOk
            Next
        End If

        Return Nothing
    End Function

    '''
    '''versucht, die linke Hälfte mit der rechten zu paaren
    '''@param listeLinks - linke Hälfte der Liste
    '''@param listeRechts - rechte Hälfte der Liste
    '''@return - Paarung erfolgreich
    '''
    Public Function StandardPaarung(ByVal kombination As List(Of Spieler), ByVal mitte As Integer, ByVal parent As Paket) As PaarungsContainer
        ' prüft, ob der potentielle Schwimmer in der rechten Liste eigentlich gar nicht schwimmen kann

        If kombination.Count Mod 2 = 1 Then
            Dim potentiellerSchwimmer = kombination.Last
            If parent.IstAltSchwimmer(potentiellerSchwimmer) Then Return Nothing

        End If

        Dim listeLinks = kombination.GetRange(0, mitte)
        Dim listeRechts = kombination.GetRange(mitte, kombination.Count - mitte)

        Dim paarungen As New List(Of SpielPartie)

        While listeLinks.Count > 0
            Dim spieler1 = listeLinks.First
            Dim spieler2 = listeRechts.First
            If spieler1.HatBereitsGespieltGegen(spieler2) Then Return Nothing

            Dim partie = New SpielPartie(_rundenName, spieler1, spieler2)            
            paarungen.Add(partie)
            listeLinks.Remove(spieler1)
            listeRechts.Remove(spieler2)
        End While

        Dim aktuellerSchwimmer As Spieler = Nothing

        If listeRechts.Count > 0 Then
            aktuellerSchwimmer = listeRechts.First
        End If

        Return New PaarungsContainer With
               {.Partien = paarungen, .SpielerListe = kombination,
                .aktuellerSchwimmer = aktuellerSchwimmer}
    End Function

End Class