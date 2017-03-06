Public Class PaketBildung

    Private ReadOnly _Rundenname As String
    Private ReadOnly _Gewinnsätze As Integer
    Public Sub New(rundenname As String, gewinnsätze As Integer)
        _Rundenname = rundenname
        _Gewinnsätze = gewinnsätze
    End Sub

    '''
    '''Stellt sicher dass alle Pakete in der richtigen Form vorliegen und alle
    '''nötigen Regeln eingehalten wurden, und sorgt für die regelkonforme
    '''Paarungsbildung.
    '''Diese sind: 
    '''1) Bildung von Paketen aus gleichen Punkten
    '''2) nacheinander für jedes Paket: Schwimmerbewegungen durchführen
    '''3) nacheinander für jedes Paket: Paarungen finden
    '''
    Public Function organisierePakete(ByVal aktiveListe As List(Of Spieler), ByVal aktuelleRunde As Integer) As List(Of SpielPartie)
        aktiveListe.Sort()
        aktiveListe.Reverse()

        Dim paarungen As New List(Of SpielPartie)
        Dim AddFreilos = Sub()

                         End Sub

        If aktiveListe.Count Mod 2 = 1 Then
            Dim freilosSpieler = freilosRegel(aktiveListe)
            aktiveListe.Remove(freilosSpieler)
            AddFreilos = Sub()
                             paarungen.Add(New FreiLosSpiel(_Rundenname, freilosSpieler, _Gewinnsätze))
                         End Sub
        End If

        Dim mittelPaket As New MittelPaket(aktuelleRunde, _Rundenname, _Gewinnsätze)
        Dim pakete = makeEvenPointPackets(_Rundenname, aktiveListe, aktuelleRunde, mittelPaket)

        If pakete.Count = 1 Then
            Dim paket = pakete.First
            paket.SuchePaarungen()
            paarungen.AddRange(From x In paket.Partien Select New SpielPartie(_Rundenname, x.Item1, x.Item2, _Gewinnsätze))
            AddFreilos()
            Return paarungen
        End If

        Dim oberePakete = New List(Of Paket)
        Dim unterePakete = New List(Of Paket)
        For i = 0 To pakete.IndexOf(mittelPaket) - 1
            oberePakete.Add(pakete(i))
        Next
        For i = pakete.IndexOf(mittelPaket) + 1 To pakete.Count - 1
            unterePakete.Add(pakete(i))
        Next

        ' Obere und untere Liste bearbeiten
        For Each Paket In oberePakete
            Paket.Absteigend = True
        Next

        mittelPaket.Absteigend = True


        doPaarungen(oberePakete, mittelPaket)

        For Each Paket In unterePakete
            Paket.Absteigend = False
        Next
        mittelPaket.Absteigend = False
        unterePakete.Reverse()
        doPaarungen(unterePakete, mittelPaket)

        ' Sonderregeln für Mittelpaket durchführen
        mittelPaket.Absteigend = True
        doMittelPaket(oberePakete, unterePakete, mittelPaket)

        For Each Paket In pakete
            Dim spielPartien = From x In Paket.Partien Select New SpielPartie(_Rundenname, x.Item1, x.Item2, _Gewinnsätze)
            paarungen.AddRange(spielPartien)
        Next
        AddFreilos()
        Return paarungen
    End Function

    Private Shared Function SucheNächstesPaket(ByVal oberePakete As List(Of Paket), ByVal unterePakete As List(Of Paket), ByVal mittelPaket As Paket, ByVal prioOben As Boolean) As Paket
        Dim gesamtListe = oberePakete.ToList
        gesamtListe.Add(mittelPaket)
        gesamtListe.AddRange(unterePakete)
        gesamtListe.Sort()

        Dim indexMittelPaket = gesamtListe.IndexOf(mittelPaket)
        Dim oberesPaket As Paket = Nothing
        Dim unteresPaket As Paket = Nothing
        If indexMittelPaket > 0 Then
            oberesPaket = gesamtListe(indexMittelPaket - 1)
        End If
        If indexMittelPaket < gesamtListe.Count - 1 Then
            unteresPaket = gesamtListe(indexMittelPaket + 1)
        End If
        If oberesPaket Is Nothing AndAlso unteresPaket Is Nothing Then Return Nothing

        If oberesPaket Is Nothing Then Return unteresPaket

        If unteresPaket Is Nothing Then Return oberesPaket

        Dim differenzOben = Math.Abs(oberesPaket.InitialNummer - mittelPaket.InitialNummer)
        Dim differenzUnten = Math.Abs(unteresPaket.InitialNummer - mittelPaket.InitialNummer)


        If differenzOben = differenzUnten Then
            If prioOben Then
                Return oberesPaket
            Else
                Return unteresPaket
            End If
        End If
        If differenzOben < differenzUnten Then
            Return oberesPaket
        Else
            Return unteresPaket
        End If

    End Function

    '''
    '''Erzeugt gültige Paarungen für das Mittelpaket. Zuerst wird versucht, das Mittelpaket
    '''auf normale Weise zu paaren. Geht das nicht, wird nach einem Schwimmer gesucht der aus der oberen
    '''bzw. unteren Paketliste stammt. Dieser wird nach oben bzw. nach unten geschoben,
    '''und von dem dortigen Paket ein neuer Schwimmer angefordert. Dies wird so lange wiederholt,
    '''bis in den angrenzenden Paketen keine Schwimmer mehr zur Verfügung stehen. Wurde auch dann keine
    '''Paarung gefunden, oder hat das Mittelpaket nie einen Schwimmer empfangen, werden die ursprünglichen Paarungen der
    '''angrenzenden Pakete schrittweise aufgelöst und dem Mittelpaket zugeordnet, bis eine Paarung funktioniert.
    '''@param oberePakete
    '''@param unterePakete
    '''@param mittelPaket
    '''
    Private Shared Sub doMittelPaket(ByVal oberePakete As List(Of Paket), ByVal unterePakete As List(Of Paket), ByVal mittelPaket As MittelPaket)
        If mittelPaket.SuchePaarungen Then Return

        If alternierendeSchwimmer(oberePakete, unterePakete, mittelPaket) Then Return

        ' Angrenzende Paarungen auflösen und dem Mittelpaket zuordnen, bis
        ' eine gültige Paarung zustande kommt

        Dim vorgänger As Paket = SucheNächstesPaket(oberePakete, unterePakete, mittelPaket, True)

        While vorgänger IsNot Nothing
            If mittelPaket.SuchePaarungen Then                
                Return
            End If

            mittelPaket.ÜbernimmPaarungen(vorgänger)
            If Not vorgänger.SpielerListe.Any Then
                oberePakete.Remove(vorgänger)
                unterePakete.Remove(vorgänger)
            End If
            vorgänger = SucheNächstesPaket(oberePakete, unterePakete, mittelPaket, True)
        End While

        Throw New Exception("Es können keine Paarungen mehr gebildet werden. Dies war die letzte Runde")
    End Sub

    Private Shared Function alternierendeSchwimmer(ByVal oberePakete As List(Of Paket), ByVal unterePakete As List(Of Paket), ByVal mittelPaket As MittelPaket) As Boolean
        ' Erstes Paket bearbeiten
        Dim paket = SucheNächstesPaket(oberePakete, unterePakete, mittelPaket, True)
        If paket Is Nothing Then Return False
        If SchwimmerTausch(paket, mittelPaket) Then Return True

        If paket.Absteigend Then
            If Not unterePakete.Any Then Return False
            paket = unterePakete.First
        Else
            If Not oberePakete.Any Then Return False
            paket = oberePakete.Last
        End If

        ' Zweites Paket bearbeiten
        Return SchwimmerTausch(paket, mittelPaket)
    End Function

    Private Shared Function SchwimmerTausch(ByVal paket As Paket, ByVal mittelPaket As MittelPaket) As Boolean
        mittelPaket.Absteigend = paket.Absteigend
        paket.sort()

        Dim backup = New Paket(paket)
        Dim backupMitte As MittelPaket = New MittelPaket(mittelPaket)

        While mittelPaket.aktuellerSchwimmer IsNot Nothing
            mittelPaket.VerschiebeSchwimmer(paket)
            If Not paket.SuchePaarungen Then Exit While
            paket.VerschiebeSchwimmer(mittelPaket)

            If mittelPaket.SuchePaarungen Then Return True
        End While

        paket.Set(backup)
        mittelPaket.Set(backupMitte)
        Return False
    End Function

    Private Shared Sub doPaarungen(ByVal pakete As List(Of Paket), ByVal mittelPaket As MittelPaket)
        For i = 0 To pakete.Count - 1
            Dim aktuellesPaket = pakete(i)
            Dim nächstesPaket As Paket
            If i = pakete.Count - 1 Then
                nächstesPaket = mittelPaket
            Else
                nächstesPaket = pakete(i + 1)
            End If

            Dim hasSucceeded = aktuellesPaket.SuchePaarungen
            If Not hasSucceeded Then
                nächstesPaket.übernimmPaket(aktuellesPaket)
            End If
            aktuellesPaket.VerschiebeSchwimmer(nächstesPaket)
        Next
        Dim toBeRemoved = New List(Of Paket)
        For Each Paket In pakete
            If Not Paket.SpielerListe.Any Then
                toBeRemoved.Add(Paket)
            End If
        Next

        For Each Paket In toBeRemoved
            pakete.Remove(Paket)
        Next

    End Sub

    '''
    '''markiert bei ungeraden Spielerzahlen denjenigen, 
    '''der ein Freilos erhalten soll.
    '''In der Paketbildung wird dieser Spieler dann ignoriert.
    '''
    Private Shared Function freilosRegel(ByVal aktiveListe As List(Of Spieler)) As Spieler
        For i = aktiveListe.Count - 1 To 0 Step -1
            Dim tempSpieler = aktiveListe(i)
            If Not tempSpieler.HatFreilos Then
                Return tempSpieler
            End If
        Next
        Throw New ArgumentException("Kein Freilosspieler gefunden")
    End Function

  

    '''
    '''erzeugt Pakete mit Spielern mit gleicher Punktzahl.
    '''Dies geschieht solange, bis keine Pakete mehr gebildet werden können.
    '''@return - es konnte ein Paket gebildet werden.
    '''
    Private Function makeEvenPointPackets(rundenname As String, ByVal spielerliste As List(Of Spieler), ByVal aktuelleRunde As Integer, ByVal mittelPaket As MittelPaket) As List(Of Paket)

        Dim pakete As New List(Of Paket)
        Dim gesamtPaketzahl = aktuelleRunde * 2 + 1

        For i = 0 To gesamtPaketzahl - 1
            If i = aktuelleRunde Then
                pakete.Add(mittelPaket)
            Else
                pakete.Add(New Paket(i, rundenname, _Gewinnsätze))
            End If
        Next

        For Each Spieler In spielerliste
            Dim punkte = Spieler.Punkte
            pakete(2 * aktuelleRunde - 2 * punkte).SpielerListe.Add(Spieler)
        Next

        Dim removePakete = New List(Of Paket)
        For Each Paket In pakete
            If Not Paket Is mittelPaket Then
                If Not Paket.SpielerListe.Any Then
                    removePakete.Add(Paket)
                End If
            End If
        Next

        For Each Paket In removePakete
            pakete.Remove(Paket)
        Next

        Return pakete
    End Function


End Class
