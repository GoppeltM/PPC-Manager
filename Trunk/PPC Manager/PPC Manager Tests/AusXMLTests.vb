Imports Moq
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class AusXMLTests
    <Test>
    Sub Spieler_Sortieren()
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
                                lastname="Goppelt" ttr="1102" birthyear="1993" licence-nr="-1"/>
                        </ppc:player>
                    </players>

        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim Spielerliste = (From x In XNode.Elements
                            Let s = AusXML.SpielerFromXML(x)
                            Select New Spieler(s, spielverlauf)).ToList
        Spielerliste.Sort()
        Spielerliste.Reverse()
        CollectionAssert.AreEqual(New String() {"Wolfert", "Westermann", "Goppelt", "Wild"}, (From x In Spielerliste Select x.Nachname).ToList)

    End Sub

    <Test>
    Sub SpielrundenFromXML_hat_4Spieler_und_1_ausgeschieden_in_Runde_1()
        Dim rundenRef = <matches>
                            <ppc:match games-b="23" matches-b="0" sets-b="0" games-a="33" matches-a="1"
                                sets-a="3" set-b-7="0" set-b-6="0" set-b-5="0" set-b-4="0" set-b-3="9" set-b-2="9" set-b-1="5"
                                set-a-7="0" set-a-6="0" set-a-5="0" set-a-4="0" set-a-3="11" set-a-2="11" set-a-1="11"
                                player-b="PLAYER72" player-a="PLAYER-1" scheduled="22.05.2013 21:17:45" group="Runde 1" nr="1"/>
                            <match games-b="23" matches-b="0" sets-b="0" games-a="33" matches-a="1"
                                sets-a="3" set-b-7="0" set-b-6="0" set-b-5="0" set-b-4="0" set-b-3="5" set-b-2="9" set-b-1="5"
                                set-a-7="0" set-a-6="0" set-a-5="0" set-a-4="0" set-a-3="11" set-a-2="11" set-a-1="11"
                                player-b="PLAYER126" player-a="PLAYER127" scheduled="22.05.2013 23:17:45" group="Runde 1" nr="2"/>
                            <ppc:freematch player="PLAYER126" scheduled="22.05.2013 23:17:45" group="Runde 2"/>
                            <ppc:inactiveplayer player="PLAYER127" group="1"/>
                        </matches>
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim RundenRes = New SpielRunden
        AusXML.SpielRundenFromXML(RundenRes, New SpielerListe From {
                                                  New Spieler("PLAYER127", spielverlauf),
                                                  New Spieler("PLAYER126", spielverlauf),
                                                  New Spieler("PLAYER72", spielverlauf),
                                                  New Spieler("PLAYER-1", spielverlauf)}, rundenRef, 3)
        Assert.That(RundenRes.Reverse.ElementAt(1).AusgeschiedeneSpielerIDs, Contains.Item("PLAYER127"))
    End Sub


    <Test>
    Public Sub Spieler_From_XML()
        Dim XNode = <player type="single" id="PLAYER72">
                        <person licence-nr="53010" club-federation-nickname="BaTTV" club-name="TTC Langensteinbach e.V. " sex="1" ttr-match-count="102" lastname="Ewald" ttr="1294" internal-nr="NU440049" club-nr="428" firstname="Florian" birthyear="1981"/>
                    </player>

        With AusXML.SpielerFromXML(XNode)
            Assert.AreEqual("Florian", .Vorname)
            Assert.AreEqual("Ewald", .Nachname)
            Assert.AreEqual("TTC Langensteinbach e.V. ", .Vereinsname)
            Assert.AreEqual(53010, .Lizenznummer)
            Assert.AreEqual(1981, .Geburtsjahr)
            Assert.AreEqual(1, .Geschlecht)
        End With
    End Sub



    <Test>
    Sub NichtZählende_SpielPartie_From_XML()
        Dim MatchXml = <ppc:match games-b="23" matches-b="0" sets-b="0" games-a="33" matches-a="1"
                           sets-a="3" set-b-7="0" set-b-6="0" set-b-5="0" set-b-4="0" set-b-3="9" set-b-2="9" set-b-1="5"
                           set-a-7="0" set-a-6="0" set-a-5="0" set-a-4="0" set-a-3="11" set-a-2="11" set-a-1="11"
                           player-b="PLAYER72" player-a="PLAYER-1" scheduled="22.05.2013 21:17:45" group="Runde 1" nr="1"/>

        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim SpielerA = New Spieler("PLAYER-1", spielverlauf) With {.Vorname = "Marius", .Nachname = "Goppelt", .Fremd = True}
        Dim SpielerB = New Spieler("PLAYER72", spielverlauf) With {.Vorname = "Florian", .Nachname = "Ewald"}

        Dim Partie = AusXML.SpielPartieFromXML(New Spieler() {SpielerA, SpielerB}, MatchXml, 3)
        With Partie
            Assert.AreEqual(3, .MeineGewonnenenSätze(SpielerA).Count)
            Assert.AreEqual(0, .MeineGewonnenenSätze(SpielerB).Count)
            CollectionAssert.AreEqual({5, 9, 9}, .Select(Function(x) x.PunkteRechts).ToList)
            CollectionAssert.AreEqual({11, 11, 11}, .Select(Function(x) x.PunkteLinks).ToList)
            Assert.AreEqual(SpielerB, .MeinGegner(SpielerA))
            Assert.AreEqual(SpielerA, .MeinGegner(SpielerB))
        End With
    End Sub

    <Test>
    Public Sub SpielPartie_From_XML()
        Dim MatchXml = <match games-b="23" matches-b="0" sets-b="0" games-a="33" matches-a="1"
                           sets-a="3" set-b-7="0" set-b-6="0" set-b-5="0" set-b-4="0" set-b-3="9" set-b-2="0" set-b-1="5"
                           set-a-7="0" set-a-6="0" set-a-5="0" set-a-4="0" set-a-3="11" set-a-2="11" set-a-1="11"
                           player-b="PLAYER299" player-a="PLAYER293" scheduled="22.05.2013 21:17:45" group=" Gruppe 01" nr="5"/>
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim SpielerA = New Spieler("PLAYER293", spielverlauf) With {.Vorname = "Florian", .Nachname = "Ewald"}
        Dim SpielerB = New Spieler("PLAYER299", spielverlauf) With {.Vorname = "Marius", .Nachname = "Goppelt"}

        Dim Partie = AusXML.SpielPartieFromXML(New Spieler() {SpielerA, SpielerB}, MatchXml, 3)
        With Partie
            Assert.AreEqual(3, .MeineGewonnenenSätze(SpielerA).Count)
            Assert.AreEqual(0, .MeineGewonnenenSätze(SpielerB).Count)
            CollectionAssert.AreEqual({5, 0, 9}, .Select(Function(x) x.PunkteRechts).ToList)
            CollectionAssert.AreEqual({11, 11, 11}, .Select(Function(x) x.PunkteLinks).ToList)
            Assert.AreEqual(SpielerB, .MeinGegner(SpielerA))
            Assert.AreEqual(SpielerA, .MeinGegner(SpielerB))
        End With
    End Sub
End Class
