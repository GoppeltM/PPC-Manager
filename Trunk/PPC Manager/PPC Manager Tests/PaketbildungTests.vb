Public Class PaketbildungTests

    <Test>
    Public Sub Nullte_Runde_paart_einmal()
        Dim spielverlauf = New Mock(Of ISpielverlauf(Of Integer?))
        Dim suchePaarungen = Function(istAltschwimmer As Predicate(Of Integer?),
                                      spielerliste As IList(Of Integer?), absteigend As Boolean) As PaarungsContainer(Of Integer?)
                                 Dim c As New PaarungsContainer(Of Integer?)
                                 c.Partien = New List(Of Tuple(Of Integer?, Integer?)) From {
                                    Tuple.Create(Of Integer?, Integer?)(5, 3),
                                    Tuple.Create(New Integer?(4), New Integer?(2))
                                 }
                                 Return c
                             End Function
        Dim x = New Integer?(42)

        Dim p = New PaketBildung(Of Integer?)(spielverlauf.Object, suchePaarungen)
        Dim r = p.organisierePakete(New Integer?() {5, 4, 3, 2, 1}, 0)
        Assert.That(r.Übrig, [Is].EqualTo(New Integer?(1)))
        Assert.That(r.Partien, Has.Member(Tuple.Create(Of Integer?, Integer?)(5, 3)))
        Assert.That(r.Partien, Has.Member(Tuple.Create(Of Integer?, Integer?)(4, 2)))
    End Sub

    <Test>
    Public Sub Erste_Runde_splittet_zwei_Pakete()
        Dim spielverlauf = New Mock(Of ISpielverlauf(Of String))
        spielverlauf.Setup(Function(m) m.BerechnePunkte("e")).Returns(1)
        spielverlauf.Setup(Function(m) m.BerechnePunkte("d")).Returns(1)
        Dim suchePaarungen = Function(istAltschwimmer As Predicate(Of String),
                                      spielerliste As IList(Of String), absteigend As Boolean) As PaarungsContainer(Of String)
                                 Dim partien = New List(Of Tuple(Of String, String))
                                 Dim c As New PaarungsContainer(Of String)
                                 c.Partien = partien
                                 If (spielerliste.SequenceEqual({"e", "d"})) Then
                                     partien.Add(Tuple.Create("e", "d"))
                                     Return c
                                 End If

                                 If (spielerliste.SequenceEqual({"c", "b"})) Then
                                     partien.Add(Tuple.Create("b", "c"))
                                     Return c
                                 End If
                                 Return Nothing
                             End Function


        Dim p = New PaketBildung(Of String)(spielverlauf.Object, suchePaarungen)
        Dim r = p.organisierePakete(New String() {"e", "d", "c", "b", "a"}, 1)
        Assert.That(r.Übrig, [Is].EqualTo("a"))
        Assert.That(r.Partien, Has.Member(Tuple.Create("e", "d")))
        Assert.That(r.Partien, Has.Member(Tuple.Create("b", "c")))
    End Sub

    <Test>
    Public Sub Dritte_Runde_lässt_zwei_Pakete_schwimmen()
        Dim spielverlauf = New Mock(Of ISpielverlauf(Of String))
        spielverlauf.Setup(Function(m) m.BerechnePunkte("d")).Returns(3)
        spielverlauf.Setup(Function(m) m.BerechnePunkte("c")).Returns(2)
        spielverlauf.Setup(Function(m) m.BerechnePunkte("b")).Returns(1)
        spielverlauf.Setup(Function(m) m.BerechnePunkte("a")).Returns(0)
        Dim suchePaarungen = Function(istAltschwimmer As Predicate(Of String),
                                      spielerliste As IList(Of String), absteigend As Boolean) As PaarungsContainer(Of String)
                                 Dim partien = New List(Of Tuple(Of String, String))
                                 Dim c As New PaarungsContainer(Of String)
                                 c.Partien = partien
                                 If (spielerliste.SequenceEqual({"d"})) Then
                                     Return Nothing
                                 End If
                                 If (spielerliste.SequenceEqual({"a"})) Then
                                     Return Nothing
                                 End If

                                 If (spielerliste.SequenceEqual({"c", "d"})) Then
                                     partien.Add(Tuple.Create("d", "c"))
                                     Return c
                                 End If

                                 If (spielerliste.SequenceEqual({"b", "a"})) Then
                                     partien.Add(Tuple.Create("a", "b"))
                                     Return c
                                 End If

                                 If (spielerliste.SequenceEqual(New String() {})) Then
                                     Return c
                                 End If

                                 Throw New Exception("Unbekanntes Paket")
                             End Function


        Dim p = New PaketBildung(Of String)(spielverlauf.Object, suchePaarungen)
        Dim r = p.organisierePakete(New String() {"d", "c", "b", "a"}, 3)
        Assert.That(r.Übrig, [Is].EqualTo(Nothing))
        Assert.That(r.Partien, Has.Member(Tuple.Create("d", "c")))
        Assert.That(r.Partien, Has.Member(Tuple.Create("a", "b")))
    End Sub

End Class
