
Class MittelPaket(Of T)
    Inherits Paket(Of T)

    Private schwimmerVonOben As T
    Private schwimmerVonUnten As T

    Sub New(suchePaarungenMitAltschwimmer As Func(Of Predicate(Of T), SuchePaarungen(Of T)),
            ByVal aktuelleRunde As Integer)
        MyBase.New(suchePaarungenMitAltschwimmer, aktuelleRunde)
    End Sub

    Sub New(ByVal mittelPaket As MittelPaket(Of T))
        MyBase.New(mittelPaket)
        schwimmerVonOben = mittelPaket.schwimmerVonOben
        schwimmerVonUnten = mittelPaket.schwimmerVonUnten
    End Sub

    Public Overrides Property aktuellerSchwimmer As T
        Get
            If Absteigend Then
                AltSchwimmer.Remove(schwimmerVonOben)
                Return schwimmerVonOben
            Else
                AltSchwimmer.Remove(schwimmerVonUnten)
                Return schwimmerVonUnten
            End If
        End Get
        Set(ByVal value As T)
            MyBase.aktuellerSchwimmer = value
        End Set
    End Property

    Sub ÜbernimmPaarungen(ByVal vorgänger As Paket(Of T))
        Dim tempPaarungen = vorgänger.Partien.ToList
        tempPaarungen.Reverse()

        For Each partie In tempPaarungen
            Dim spielerL = partie.Item1
            Dim spielerR = partie.Item2
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
        vorgänger.SpielerListe.Remove(MyPartie.Item1)
        vorgänger.SpielerListe.Remove(MyPartie.Item2)
        Me.SpielerListe.Add(MyPartie.Item1)
        Me.SpielerListe.Add(MyPartie.Item2)
    End Sub

End Class
