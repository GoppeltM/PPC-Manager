
Class MittelPaket
    Inherits Paket

    Private schwimmerVonOben As Spieler
    Private schwimmerVonUnten As Spieler

    Sub New(ByVal aktuelleRunde As Integer, rundenName As String)
        MyBase.New(aktuelleRunde, rundenName)
    End Sub

    Sub New(ByVal mittelPaket As MittelPaket)
        MyBase.New(mittelPaket)
        schwimmerVonOben = mittelPaket.schwimmerVonOben
        schwimmerVonUnten = mittelPaket.schwimmerVonUnten
    End Sub

    Public Overrides Property aktuellerSchwimmer As Spieler
        Get
            If Absteigend Then
                AltSchwimmer.Remove(schwimmerVonOben)
                Return schwimmerVonOben
            Else
                AltSchwimmer.Remove(schwimmerVonUnten)
                Return schwimmerVonUnten
            End If
        End Get
        Set(ByVal value As Spieler)
            MyBase.aktuellerSchwimmer = value
        End Set
    End Property

    Sub ÜbernimmPaarungen(ByVal vorgänger As Paket)
        Dim tempPaarungen = vorgänger.Partien.ToList
        tempPaarungen.Reverse()

        For Each partie In tempPaarungen
            Dim spielerL = partie.SpielerLinks
            Dim spielerR = partie.SpielerRechts
            If Not vorgänger.AltSchwimmer.Contains(spielerL) Then
                If Not vorgänger.AltSchwimmer.Contains(spielerR) Then
                    vorgänger.Partien.Remove(partie)
                    vorgänger.SpielerListe.Remove(spielerL)
                    vorgänger.SpielerListe.Remove(spielerR)
                    Me.SpielerListe.Add(spielerL)
                    Me.SpielerListe.Add(spielerR)
                    Return
                End If
            End If
        Next

        Dim MyPartie = vorgänger.Partien.Last
        vorgänger.Partien.Remove(MyPartie)
        vorgänger.SpielerListe.Remove(MyPartie.SpielerLinks)
        vorgänger.SpielerListe.Remove(MyPartie.SpielerRechts)
        Me.SpielerListe.Add(MyPartie.SpielerLinks)
        Me.SpielerListe.Add(MyPartie.SpielerRechts)
    End Sub



End Class
