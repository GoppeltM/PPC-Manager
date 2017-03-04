Public Class PaarungssucheTests
    <Test>
    Sub Gerade_StandardPaarung_hat_keinen_Schwimmer()
        Dim l = {4, 3, 2, 1}

        Dim suche = New PaarungsSuche(Of Integer)(Function(a, b) False, Function(x) False).SuchePaarungen(l, True)
        Assert.IsNotNull(suche)
        Assert.That(suche.Übrig, Iz.EqualTo(0))
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.That(suche.Partien(0), Iz.EqualTo(Tuple.Create(4, 2)))
        Assert.That(suche.Partien(1), Iz.EqualTo(Tuple.Create(3, 1)))
    End Sub

    <Test>
    Sub Wer_bereits_gegeneinander_gespielt_hat_spielt_nicht_erneut()
        Dim l = {4, 3, 2, 1}
        Dim habenGespielt = Function(x As Integer, y As Integer) As Boolean
                                If x = 4 And y = 2 Then Return True
                                If x = 2 And y = 4 Then Return True
                                Return False
                            End Function
        Dim suche = New PaarungsSuche(Of Integer)(habenGespielt, Function(x) False).SuchePaarungen(l, True)
        Assert.That(suche.Partien(0), Iz.EqualTo(Tuple.Create(4, 1)))
        Assert.That(suche.Partien(1), Iz.EqualTo(Tuple.Create(3, 2)))
    End Sub

    <Test>
    Sub Keine_Paarung_wenn_alle_möglichen_Schwimmer_Altschwimmer_sind()
        Dim l = {3, 2, 1}
        Dim habenGespielt = Function(x As Integer, y As Integer) As Boolean
                                If x = 3 And y = 2 Then Return True
                                If x = 2 And y = 3 Then Return True
                                Return False
                            End Function
        Dim altSchwimmer = Function(x As Integer) As Boolean
                               If x = 3 Then Return True
                               If x = 2 Then Return True
                               Return False
                           End Function
        Dim suche = New PaarungsSuche(Of Integer)(habenGespielt, altSchwimmer).SuchePaarungen(l, True)
        Assert.That(suche, Iz.Null)
    End Sub

    <Test>
    Sub Ungerade_StandardPaarung_hat_letzten_Spieler_als_Schwimmer()
        Dim l = {5, 4, 3, 2, 1}

        Dim suche = New PaarungsSuche(Of Integer)(Function(a, b) False, Function(x) False).SuchePaarungen(l, True)
        Assert.IsNotNull(suche)
        Assert.That(suche.Übrig, Iz.EqualTo(1))
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.That(suche.Partien(0), Iz.EqualTo(Tuple.Create(5, 3)))
        Assert.That(suche.Partien(1), Iz.EqualTo(Tuple.Create(4, 2)))
    End Sub

    <Test>
    Sub Kein_Ergebnis_wenn_einer_bereits_gegen_alle_gespielt_hat()
        Dim l = {4, 3, 2, 1}
        Dim habenGespielt = Function(x As Integer, y As Integer) As Boolean
                                If x = 4 Or y = 4 Then Return True
                                Return False
                            End Function
        Dim suche = New PaarungsSuche(Of Integer)(habenGespielt, Function(x) False).SuchePaarungen(l, True)
        Assert.That(suche, Iz.Null)
    End Sub

    <Test>
    Sub Kein_Ergebnis_wenn_ungerades_Paket_nur_Altschwimmer_hat()
        Dim l = {5, 4, 3, 2, 1}
        Dim suche = New PaarungsSuche(Of Integer)(Function(x, y) False, Function(x) True).SuchePaarungen(l, True)
        Assert.That(suche, Iz.Null)
    End Sub

    <Test>
    Sub Letzter_Spieler_ist_nicht_Schwimmer_wenn_er_Altschwimmer_ist()
        Dim l = {5, 4, 3, 2, 1}
        Dim suche = New PaarungsSuche(Of Integer)(Function(x, y) False, Function(x) x = 1).SuchePaarungen(l, True)
        Assert.That(suche.Übrig, Iz.EqualTo(2))
        Assert.That(suche.Partien(0), Iz.EqualTo(Tuple.Create(5, 3)))
        Assert.That(suche.Partien(1), Iz.EqualTo(Tuple.Create(4, 1)))
    End Sub

End Class
