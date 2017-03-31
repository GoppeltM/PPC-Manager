Imports PPC_Manager

Public Class PaketBildung2(Of T)

    Private ReadOnly _SuchePaarungen As SuchePaarungen(Of T)
    Private ReadOnly _Spielverlauf As ISpielverlauf(Of T)

    Public Sub New(spielverlauf As ISpielverlauf(Of T),
                   suchePaarungen As SuchePaarungen(Of T))
        _SuchePaarungen = suchePaarungen
        _Spielverlauf = spielverlauf
    End Sub

    '''
    '''Stellt sicher dass alle Pakete in der richtigen Form vorliegen und alle
    '''nötigen Regeln eingehalten wurden, und sorgt für die regelkonforme
    '''Paarungsbildung.
    '''Diese sind: 
    '''1) Bildung von Paketen aus gleichen Punkten
    '''2) nacheinander für jedes Paket: Schwimmerbewegungen durchführen
    '''3) nacheinander für jedes Paket: Paarungen finden
    '''<param name="l">Absteigend sortierte Liste aller Spieler</param>    
    '''<param name="aktuelleRunde">Die Nummer der aktuellen Runde</param>
    Public Function organisierePakete(ByVal l As IEnumerable(Of T), ByVal aktuelleRunde As Integer) As PaarungsContainer(Of T)

        Dim aktiveListe As IEnumerable(Of T) = l.ToList
        Dim paarungen = New Stack(Of Tuple(Of T, T))
        Dim mitte = aktuelleRunde \ 2 + (aktuelleRunde Mod 2)
        Dim hochzählenListe = Enumerable.Range(0, mitte)
        Dim hochzählen = hochzählenListe.GetEnumerator
        Dim runterzählen = hochzählenListe.Select(Function(m) m + mitte).Reverse.GetEnumerator
        Dim altSchwimmer = New List(Of T)
        Dim istAltschwimmer = Function(x As T) altSchwimmer.Contains(x)
        Dim freilosSpieler As T = Nothing
        If aktiveListe.Count Mod 2 = 1 Then
            freilosSpieler = aktiveListe.Reverse().First(Function(x) Not _Spielverlauf.HatFreilos(x))
            aktiveListe = aktiveListe.Except(New T() {freilosSpieler})
        End If

        Dim pushStack = Sub(spielerListe As IList(Of T), absteigend As Boolean)
                            Dim paarung = _SuchePaarungen(istAltschwimmer, spielerListe, True)
                            If paarung IsNot Nothing Then
                                For Each p In paarung.Partien
                                    paarungen.Push(p)
                                    aktiveListe = aktiveListe.Except(New T() {p.Item1, p.Item2})
                                Next
                                If paarung.Übrig IsNot Nothing Then
                                    altSchwimmer.Add(paarung.Übrig)
                                End If
                            Else
                                For Each s In spielerListe
                                    altSchwimmer.Add(s)
                                Next
                            End If

                        End Sub

        While runterzählen.MoveNext
            With runterzählen
                Dim spielerPaket = aktiveListe.Where(Function(x) _Spielverlauf.BerechnePunkte(x) >= runterzählen.Current).ToList
                pushStack(spielerPaket, True)
            End With
        End While

        While hochzählen.MoveNext
            With hochzählen
                Dim spielerPaket = aktiveListe.Where(Function(x) _Spielverlauf.BerechnePunkte(x) <= hochzählen.Current).ToList
                pushStack(spielerPaket, False)
            End With
        End While

        Dim abwärts = True
        While aktiveListe.Any
            Dim paarung = _SuchePaarungen(istAltschwimmer, aktiveListe.ToList, abwärts)
            pushStack(aktiveListe.ToList, abwärts)
            If paarung Is Nothing Then
                Dim p = paarungen.Pop
                aktiveListe = aktiveListe.Concat(New T() {p.Item1, p.Item2})
            End If
            abwärts = Not abwärts
        End While

        Return New PaarungsContainer(Of T) With {.Partien = paarungen.ToList, .Übrig = freilosSpieler}

    End Function

End Class
