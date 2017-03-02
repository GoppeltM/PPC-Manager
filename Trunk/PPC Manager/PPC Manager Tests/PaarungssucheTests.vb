Public Class PaarungssucheTests
    <Test>
    Sub Gerade_StandardPaarung()
        Dim l = New List(Of Spieler) From
                {CreateSpieler(nachname:="Alpha", ttrating:=50),
                CreateSpieler(nachname:="Beta", ttrating:=40),
                CreateSpieler(nachname:="Gamma", ttrating:=30),
                CreateSpieler(nachname:="Delta", ttrating:=20)}

        Dim suche = New PaarungsSuche("Runde 1", 3).StandardPaarung(l, 2, Function(x) False)
        Assert.IsNotNull(suche)
        Assert.IsNull(suche.aktuellerSchwimmer)
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.AreEqual("Runde 1", suche.Partien.First.RundenName)
    End Sub

    <Test>
    Sub Ungerade_StandardPaarung()
        Dim l = New List(Of Spieler) From
                {CreateSpieler(nachname:="Alpha", ttrating:=50),
                CreateSpieler(nachname:="Beta", ttrating:=40),
                CreateSpieler(nachname:="Gamma", ttrating:=30),
                CreateSpieler(nachname:="Delta", ttrating:=20),
                CreateSpieler(nachname:="Epsilon", ttrating:=20)}

        Dim suche = New PaarungsSuche("Runde xyz", 3).StandardPaarung(l, 2, Function(x) False)
        Assert.IsNotNull(suche)
        Assert.IsNotNull(suche.aktuellerSchwimmer)
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.AreEqual("Runde xyz", suche.Partien.First.RundenName)
    End Sub

    <SetUp>
    Sub Init()
        c = New Competition(New SpielRegeln(3, True, True))
    End Sub

    Private c As Competition

    Private Function CreateSpieler(Optional nachname As String = "", Optional vorname As String = "",
                               Optional ttrating As Integer = 0, Optional id As String = "") As Spieler
        Dim s = New Spieler(c.SpielRunden, c.SpielRegeln)
        s.Nachname = nachname
        s.Vorname = vorname
        s.TTRating = ttrating
        s.Id = id
        Return s
    End Function
End Class
