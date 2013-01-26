﻿Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports PPC_Manager

<TestClass()> Public Class XMLValidation

    <TestMethod>
    Public Sub Competition_From_XML()
        Dim reference = Competition.FromXML("D:\dummy.xml", <competition start-date="2012-09-08 11:00" ttr-remarks="-" age-group="Mädchen U 13" type="Einzel">
                                                                <players>
                                                                    <player type="single" id="PLAYER72">
                                                                        <person licence-nr="53010" club-federation-nickname="BaTTV" club-name="TTC Langensteinbach e.V. " sex="1" ttr-match-count="102" lastname="Ewald" ttr="1294" internal-nr="NU440049" club-nr="428" firstname="Florian" birthyear="1981"/>
                                                                    </player>
                                                                    <player type="single" id="PLAYER73">
                                                                        <person licence-nr="109075" club-federation-nickname="BaTTV" club-name="SV Büchenbronn e.V." sex="1" ttr-match-count="171" lastname="Fabricius" ttr="1326" internal-nr="NU490963" club-nr="703" firstname="Dominik" birthyear="1989"/>
                                                                    </player>
                                                                </players>
                                                                <matches>
                                                                    <match games-b="23" matches-b="0" sets-b="0" games-a="33" matches-a="1"
                                                                        sets-a="3" set-b-7="0" set-b-6="0" set-b-5="0" set-b-4="0" set-b-3="9" set-b-2="9" set-b-1="5"
                                                                        set-a-7="0" set-a-6="0" set-a-5="0" set-a-4="0" set-a-3="11" set-a-2="11" set-a-1="11"
                                                                        player-b="PLAYER299" player-a="PLAYER293" scheduled="" group=" Gruppe 01" nr="5"/>
                                                                    <match games-b="23" matches-b="0" sets-b="0" games-a="33" matches-a="1"
                                                                        sets-a="3" set-b-7="0" set-b-6="0" set-b-5="0" set-b-4="0" set-b-3="9" set-b-2="9" set-b-1="5"
                                                                        set-a-7="0" set-a-6="0" set-a-5="0" set-a-4="0" set-a-3="11" set-a-2="11" set-a-1="11"
                                                                        player-b="PLAYER299" player-a="PLAYER293" scheduled="" group=" Gruppe 01" nr="5"/>
                                                                </matches>
                                                            </competition>, 4, False)

        With reference
            Assert.AreEqual("D:\dummy.xml", .DateiPfad)
            Assert.AreEqual(4, .Gewinnsätze)
            Assert.AreEqual(False, .SatzDifferenz)
            Assert.AreEqual("2012-09-08 11:00", .StartDatum)
            Assert.AreEqual("Mädchen U 13", .Altersgruppe)
            Assert.AreEqual(2, .SpielerListe.Count)
            CollectionAssert.AreEqual({"PLAYER72", "PLAYER73"}, (From x In .SpielerListe Select x.Id).ToList)
        End With
    End Sub


    <TestMethod>
    Public Sub Spieler_From_XML()
        Dim XNode = <player type="single" id="PLAYER72">
                        <person licence-nr="53010" club-federation-nickname="BaTTV" club-name="TTC Langensteinbach e.V. " sex="1" ttr-match-count="102" lastname="Ewald" ttr="1294" internal-nr="NU440049" club-nr="428" firstname="Florian" birthyear="1981"/>

                    </player>

        With Spieler.FromXML(XNode, New SpielRunden)
            Assert.AreEqual("Florian", .Vorname)
            Assert.AreEqual("Ewald", .Nachname)
            Assert.AreEqual("TTC Langensteinbach e.V. ", .Vereinsname)
            Assert.AreEqual(53010, .Lizenznummer)
        End With
    End Sub

    <TestMethod>
    Public Sub SpielPartie_From_XML()
        Dim MatchXml = <match games-b="23" matches-b="0" sets-b="0" games-a="33" matches-a="1"
                           sets-a="3" set-b-7="0" set-b-6="0" set-b-5="0" set-b-4="0" set-b-3="9" set-b-2="9" set-b-1="5"
                           set-a-7="0" set-a-6="0" set-a-5="0" set-a-4="0" set-a-3="11" set-a-2="11" set-a-1="11"
                           player-b="PLAYER299" player-a="PLAYER293" scheduled="" group=" Gruppe 01" nr="5"/>

        Dim SpielerA = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"}
        Dim SpielerB = New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Id = "PLAYER299"}

        Dim Partie = SpielPartie.FromXML(New Spieler() {SpielerA, SpielerB}, MatchXml)
        With Partie
            Assert.AreEqual(3, .MeineGewonnenenSätze(SpielerA).Count)
            Assert.AreEqual(0, .MeineGewonnenenSätze(SpielerB).Count)
            CollectionAssert.AreEqual({5, 9, 9}, .Select(Function(x) x.PunkteRechts).ToList)
            CollectionAssert.AreEqual({11, 11, 11}, .Select(Function(x) x.PunkteLinks).ToList)
            Assert.AreEqual(SpielerB, .MeinGegner(SpielerA))
            Assert.AreEqual(SpielerA, .MeinGegner(SpielerB))
        End With

    End Sub

End Class