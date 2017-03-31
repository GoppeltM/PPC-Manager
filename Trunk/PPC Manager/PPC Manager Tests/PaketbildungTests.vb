Public Class PaketbildungTests

    <Test>
    Public Sub Nullte_Runde_paart_einmal()
        Dim spielverlauf = New Mock(Of ISpielverlauf(Of Integer))
        Dim suchePaarungen = Function(istAltschwimmer As Predicate(Of Integer),
                                      spielerliste As IList(Of Integer), absteigend As Boolean) As PaarungsContainer(Of Integer)
                                 Dim c As New PaarungsContainer(Of Integer)
                                 c.Partien = New List(Of Tuple(Of Integer, Integer)) From {
                                    Tuple.Create(5, 3),
                                    Tuple.Create(4, 2)
                                 }
                                 Return c
                             End Function


        Dim p = New PaketBildung2(Of Integer)(spielverlauf.Object, suchePaarungen)
        Dim r = p.organisierePakete(New Integer() {5, 4, 3, 2, 1}, 0)
        Assert.That(r.Übrig, [Is].EqualTo(1))
        Assert.That(r.Partien, Has.Member(Tuple.Create(5, 3)))
        Assert.That(r.Partien, Has.Member(Tuple.Create(4, 2)))
    End Sub

    <Test>
    Public Sub Erste_Runde_splittet_zwei_Pakete()
        Dim spielverlauf = New Mock(Of ISpielverlauf(Of Integer))
        spielverlauf.Setup(Function(m) m.BerechnePunkte(5)).Returns(1)
        spielverlauf.Setup(Function(m) m.BerechnePunkte(4)).Returns(1)
        Dim suchePaarungen = Function(istAltschwimmer As Predicate(Of Integer),
                                      spielerliste As IList(Of Integer), absteigend As Boolean) As PaarungsContainer(Of Integer)
                                 Dim partien = New List(Of Tuple(Of Integer, Integer))
                                 Dim c As New PaarungsContainer(Of Integer)
                                 c.Partien = partien
                                 If (spielerliste.SequenceEqual({5, 4})) Then
                                     partien.Add(Tuple.Create(5, 4))
                                     Return c
                                 End If

                                 If (spielerliste.SequenceEqual({3, 2})) Then
                                     partien.Add(Tuple.Create(2, 3))
                                     Return c
                                 End If
                                 Return Nothing
                             End Function


        Dim p = New PaketBildung2(Of Integer)(spielverlauf.Object, suchePaarungen)
        Dim r = p.organisierePakete(New Integer() {5, 4, 3, 2, 1}, 1)
        Assert.That(r.Übrig, [Is].EqualTo(1))
        Assert.That(r.Partien, Has.Member(Tuple.Create(5, 4)))
        Assert.That(r.Partien, Has.Member(Tuple.Create(2, 3)))
    End Sub

    <Test>
    Public Sub Dritte_Runde_lässt_zwei_Pakete_schwimmen()
        Dim spielverlauf = New Mock(Of ISpielverlauf(Of Integer))
        spielverlauf.Setup(Function(m) m.BerechnePunkte(4)).Returns(3)
        spielverlauf.Setup(Function(m) m.BerechnePunkte(3)).Returns(2)
        spielverlauf.Setup(Function(m) m.BerechnePunkte(2)).Returns(1)
        spielverlauf.Setup(Function(m) m.BerechnePunkte(1)).Returns(0)
        Dim suchePaarungen = Function(istAltschwimmer As Predicate(Of Integer),
                                      spielerliste As IList(Of Integer), absteigend As Boolean) As PaarungsContainer(Of Integer)
                                 Dim partien = New List(Of Tuple(Of Integer, Integer))
                                 Dim c As New PaarungsContainer(Of Integer)
                                 c.Partien = partien
                                 If (spielerliste.SequenceEqual({4})) Then
                                     Return Nothing
                                 End If
                                 If (spielerliste.SequenceEqual({1})) Then
                                     Return Nothing
                                 End If

                                 If (spielerliste.SequenceEqual({4, 3})) Then
                                     partien.Add(Tuple.Create(4, 3))
                                     Return c
                                 End If

                                 If (spielerliste.SequenceEqual({2, 1})) Then
                                     partien.Add(Tuple.Create(1, 2))
                                     Return c
                                 End If
                                 Return Nothing
                             End Function


        Dim p = New PaketBildung2(Of Integer)(spielverlauf.Object, suchePaarungen)
        Dim r = p.organisierePakete(New Integer() {4, 3, 2, 1}, 3)
        Assert.That(r.Übrig, [Is].EqualTo(0))
        Assert.That(r.Partien, Has.Member(Tuple.Create(4, 3)))
        Assert.That(r.Partien, Has.Member(Tuple.Create(1, 2)))
    End Sub

End Class
