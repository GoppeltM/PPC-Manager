
Class MittelPaket(Of T)
    Inherits Paket(Of T)

    Private schwimmerVonOben As T
    Private schwimmerVonUnten As T

    Sub New(suchePaarungen As SuchePaarungen(Of T),
            ByVal aktuelleRunde As Integer,
            istAltschwimmer As Predicate(Of T),
            setzeAltschwimmer As Action(Of T))
        MyBase.New(suchePaarungen, aktuelleRunde, istAltschwimmer, setzeAltschwimmer)
    End Sub

    Sub New(ByVal mittelPaket As MittelPaket(Of T))
        MyBase.New(mittelPaket)
        schwimmerVonOben = mittelPaket.schwimmerVonOben
        schwimmerVonUnten = mittelPaket.schwimmerVonUnten
    End Sub

    Public Overrides Property aktuellerSchwimmer As T
        Get
            If Absteigend Then
                Return schwimmerVonOben
            Else
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
            If Not _IstAltschwimmer(spielerL) Then
                If Not _IstAltschwimmer(spielerR) Then
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
