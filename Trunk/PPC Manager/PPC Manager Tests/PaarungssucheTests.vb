Public Class PaarungssucheTests
    <Test>
    Sub Gerade_StandardPaarung_hat_keinen_Schwimmer()
        Dim l = {4, 3, 2, 1}

        Dim suche = New PaarungsSuche(Of Integer)(Function(a, b) False, Function(x) False).SuchePaarungen(l, True)
        Assert.IsNotNull(suche)
        Assert.That(suche.aktuellerSchwimmer, Iz.EqualTo(0))
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.That(suche.Partien(0), Iz.EqualTo(Tuple.Create(4, 2)))
        Assert.That(suche.Partien(1), Iz.EqualTo(Tuple.Create(3, 1)))
    End Sub

    <Test>
    Sub Ungerade_StandardPaarung_hat_letzten_Spieler_als_Schwimmer()
        Dim l = {5, 4, 3, 2, 1}

        Dim suche = New PaarungsSuche(Of Integer)(Function(a, b) False, Function(x) False).SuchePaarungen(l, True)
        Assert.IsNotNull(suche)
        Assert.That(suche.aktuellerSchwimmer, Iz.EqualTo(1))
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.That(suche.Partien(0), Iz.EqualTo(Tuple.Create(5, 3)))
        Assert.That(suche.Partien(1), Iz.EqualTo(Tuple.Create(4, 2)))
    End Sub

End Class
