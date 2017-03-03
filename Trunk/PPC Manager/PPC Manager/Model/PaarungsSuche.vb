
Public Delegate Function HabenGegeneinanderGespielt(Of In T)(a As T, b As T) As Boolean

Public Class PaarungsSuche(Of T As IComparable(Of T))

    Private ReadOnly _HabenGegeneinanderGespielt As HabenGegeneinanderGespielt(Of T)
    Private ReadOnly _IstAltSchwimmer As Predicate(Of T)

    Public Sub New(habenGegeneinanderGespielt As HabenGegeneinanderGespielt(Of T),
                   ByVal istAltSchwimmer As Predicate(Of T))

        _HabenGegeneinanderGespielt = habenGegeneinanderGespielt
        _IstAltSchwimmer = istAltSchwimmer
    End Sub

    Public Function SuchePaarungen(ByVal spielerListe As IList(Of T), absteigend As Boolean) As PaarungsContainer(Of T)
        If spielerListe.Count < 2 Then Return Nothing

        ' Erzeugung der linken und rechten Liste. Diese dürfen durch die
        'nachfolgenden Tests NICHT verändert werden! Deshalb wird in jedem Test eine
        'Kopie dieser beiden Vektoren angelegt.
        Dim tempListe As New List(Of T)(spielerListe)
        Dim mitte = tempListe.Count \ 2
        Return rekursiveUmtauschung(New List(Of T), tempListe, mitte, absteigend)
    End Function

    Private Function rekursiveUmtauschung(ByVal anfang As List(Of T),
                                          ByVal rest As List(Of T),
                                          ByVal mitte As Integer,
                                          absteigend As Boolean) As PaarungsContainer(Of T)
        ' Ist nur noch ein Spieler am Ende da, muss das eine neue Kombination sein

        If rest.Count = 1 Then
            Dim kombination = New List(Of T)(anfang)
            kombination.AddRange(rest)
            Dim isOk = StandardPaarung(kombination, mitte)
            If isOk IsNot Nothing Then
                Return isOk
            End If
        Else
            ' bei mehreren wähle immer einen und setze ihn mit an den Anfang
            For i = 0 To rest.Count - 1
                Dim restNeu = New List(Of T)(rest)
                Dim anfangNeu = New List(Of T)(anfang)
                Dim tauschSpieler = restNeu(i)
                Dim NeuerIndex = anfangNeu.Count
                If NeuerIndex >= mitte Then
                    Dim PartnerSpieler = anfangNeu(NeuerIndex - mitte)
                    ' Optimierung 1:
                    '  Wenn ABC <-> DEF, dann prüfe ob D > A. Wenn ja, wurde A gegen D bereits geprüft,
                    ' und alle darauf aufbauenden Kombinationen auch.
                    Dim vergleich = tauschSpieler.CompareTo(PartnerSpieler)
                    If Not absteigend Then vergleich = vergleich * -1
                    If vergleich > 0 Then Continue For
                    ' Optimierung 2:
                    ' Wenn ABC <-> DEF, dann prüfe ob D gegen A schonmal gespielt hat.
                    ' Wenn ja, sind eh alle darauf basierenden Kombinationen unmöglich.
                    If _HabenGegeneinanderGespielt(tauschSpieler, PartnerSpieler) Then Continue For
                End If
                restNeu.Remove(tauschSpieler)
                anfangNeu.Add(tauschSpieler)

                Dim isOk = rekursiveUmtauschung(anfangNeu, restNeu, mitte, absteigend)

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
    Private Function StandardPaarung(ByVal kombination As List(Of T),
                                    ByVal mitte As Integer) As PaarungsContainer(Of T)
        ' prüft, ob der potentielle Schwimmer in der rechten Liste eigentlich gar nicht schwimmen kann

        If kombination.Count Mod 2 = 1 Then
            Dim potentiellerSchwimmer = kombination.Last
            If _IstAltSchwimmer(potentiellerSchwimmer) Then Return Nothing

        End If

        Dim listeLinks = kombination.GetRange(0, mitte)
        Dim listeRechts = kombination.GetRange(mitte, kombination.Count - mitte)

        Dim paarungen As New List(Of Tuple(Of T, T))

        While listeLinks.Count > 0
            Dim spieler1 = listeLinks.First
            Dim spieler2 = listeRechts.First
            If _HabenGegeneinanderGespielt(spieler1, spieler2) Then Return Nothing

            Dim partie = Tuple.Create(spieler1, spieler2)
            paarungen.Add(partie)
            listeLinks.Remove(spieler1)
            listeRechts.Remove(spieler2)
        End While

        Dim aktuellerSchwimmer As T = Nothing

        If listeRechts.Count > 0 Then
            aktuellerSchwimmer = listeRechts.First
        End If

        Return New PaarungsContainer(Of T) With
               {.Partien = paarungen, .Übrig = aktuellerSchwimmer}
    End Function

End Class