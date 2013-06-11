Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class Paket_Tests

    <TestMethod>
    Sub Spieler_Sortieren()
        MainWindow.AktiveCompetition = New Competition
        Dim XNode = <players>
                        <player type="single" id="PLAYER114">
                            <person licence-nr="49790" club-federation-nickname="BaTTV" club-name="VfR Rheinsheim" sex="1"
                                ttr-match-count="181" lastname="Westermann" ttr="1284" internal-nr="NU439379" club-nr="128"
                                firstname="Gerd" birthyear="1973"/>
                        </player>
                        <player type="single" id="PLAYER115">
                            <person licence-nr="143323" club-federation-nickname="BaTTV" club-name="TTF 1955 Ispringen e.V."
                                sex="1" ttr-match-count="46" lastname="Wild" ttr="1009" internal-nr="NU1072486" club-nr="713"
                                firstname="Christian" birthyear="1993"/>
                        </player>
                        <player type="single" id="PLAYER116">
                            <person licence-nr="108866" club-federation-nickname="BaTTV" club-name="TV 1897 Forst eV"
                                sex="1" ttr-match-count="178" lastname="Wolfert" ttr="1310" internal-nr="NU489193"
                                club-nr="108" firstname="Till" birthyear="1996"/>
                        </player>
                        <ppc:player xmlns:ppc="http://www.ttc-langensteinbach.de/" id="PLAYER-1">
                            <person club-name="TTC Langensteinbach" sex="1" ttr-match-count="0" firstname="Marius"
                                lastname="Goppelt" ttr="1102" licence-nr="-1"/>
                        </ppc:player>
                    </players>

        Dim Spielerliste = (From x In XNode.Elements Select Spieler.FromXML(x)).ToList
        Spielerliste.Sort()
        CollectionAssert.AreEqual(New String() {"Wolfert", "Westermann", "Goppelt", "Wild"}, (From x In Spielerliste Select x.Nachname).ToList)

    End Sub

    <TestMethod>
    Sub Gerade_StandardPaarung()
        MainWindow.AktiveCompetition = New Competition
        Dim p As New Paket(1, "Runde 1")
        p.SpielerListe = New List(Of Spieler) From
                {New Spieler With {.Nachname = "Alpha", .TTRating = 50},
                New Spieler With {.Nachname = "Beta", .TTRating = 40},
                New Spieler With {.Nachname = "Gamma", .TTRating = 30},
                New Spieler With {.Nachname = "Delta", .TTRating = 20}}

        Dim suche = New PaarungsSuche("Runde 1").StandardPaarung(p.SpielerListe, 2, p)
        Assert.IsNotNull(suche)
        Assert.IsNull(suche.aktuellerSchwimmer)
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.AreEqual("Runde 1", suche.Partien.First.RundenName)
    End Sub

    <TestMethod>
    Sub Ungerade_StandardPaarung()
        MainWindow.AktiveCompetition = New Competition
        Dim p As New Paket(1, "Runde xyz")
        p.SpielerListe = New List(Of Spieler) From
                {New Spieler With {.Nachname = "Alpha", .TTRating = 50},
                New Spieler With {.Nachname = "Beta", .TTRating = 40},
                New Spieler With {.Nachname = "Gamma", .TTRating = 30},
                New Spieler With {.Nachname = "Delta", .TTRating = 20},
                New Spieler With {.Nachname = "Epsilon", .TTRating = 20}}

        Dim suche = New PaarungsSuche("Runde xyz").StandardPaarung(p.SpielerListe, 2, p)
        Assert.IsNotNull(suche)
        Assert.IsNotNull(suche.aktuellerSchwimmer)
        Assert.AreEqual(2, suche.Partien.Count)
        Assert.AreEqual("Runde xyz", suche.Partien.First.RundenName)
    End Sub

End Class
