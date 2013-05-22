Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports PPC_Manager
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports System.Collections.ObjectModel

<TestClass()> Public Class XMLValidation

    <TestMethod>
    Public Sub Competition_From_XML()
        Dim reference = Competition.FromXML("D:\dummy.xml", XDocument.Parse(My.Resources.Competition).Root.<competition>.First, 4, False)
        MainWindow.AktiveCompetition = reference
        With reference
            Assert.AreEqual("D:\dummy.xml", .DateiPfad)
            Assert.AreEqual(4, .Gewinnsätze)
            Assert.AreEqual(False, .SatzDifferenz)
            Assert.AreEqual("2012-09-08 11:00", .StartDatum)
            Assert.AreEqual("Mädchen U 13", .Altersgruppe)
            Assert.AreEqual(4, .SpielerListe.Count)
            Assert.AreEqual(2, .SpielRunden.Count)
            Assert.AreEqual(1, .SpielRunden.Last.OfType(Of FreiLosSpiel).Count)            
            CollectionAssert.AreEqual({"PLAYER72", "PLAYER126", "PLAYER127", "PLAYER-1"}, (From x In .SpielerListe Select x.Id).ToList)
        End With
    End Sub

    <TestMethod>
    Sub Competition_ErsteRunde_From_XML()
        Dim reference = Competition.FromXML("D:\dummy.xml", XDocument.Parse(My.Resources.Competition).Root.<competition>.First, 4, False)
        MainWindow.AktiveCompetition = reference

        Dim ersteRunde = reference.SpielRunden.First
        Assert.AreEqual(2, ersteRunde.Count, "Zwei Spielpartien erwartet")
        With ersteRunde
            Assert.AreEqual("PLAYER127", .AusgeschiedeneSpieler.First.Id)
            CollectionAssert.AreEqual({"PLAYER127"}, (From x In .AusgeschiedeneSpieler Select x.Id).ToList)

            For Each partie In .ToList
                Assert.AreEqual(3, partie.Count)
            Next
        End With
        
    End Sub


    <TestMethod>
    Public Sub Spieler_From_XML()
        Dim XNode = <player type="single" id="PLAYER72">
                        <person licence-nr="53010" club-federation-nickname="BaTTV" club-name="TTC Langensteinbach e.V. " sex="1" ttr-match-count="102" lastname="Ewald" ttr="1294" internal-nr="NU440049" club-nr="428" firstname="Florian" birthyear="1981"/>
                    </player>

        With Spieler.FromXML(XNode)
            Assert.AreEqual("Florian", .Vorname)
            Assert.AreEqual("Ewald", .Nachname)
            Assert.AreEqual("TTC Langensteinbach e.V. ", .Vereinsname)
            Assert.AreEqual(53010, .Lizenznummer)
        End With
    End Sub

    <TestMethod>
    Sub NichtZählende_SpielPartie_From_XML()
        Dim MatchXml = <ppc:match games-b="23" matches-b="0" sets-b="0" games-a="33" matches-a="1"
                           sets-a="3" set-b-7="0" set-b-6="0" set-b-5="0" set-b-4="0" set-b-3="9" set-b-2="9" set-b-1="5"
                           set-a-7="0" set-a-6="0" set-a-5="0" set-a-4="0" set-a-3="11" set-a-2="11" set-a-1="11"
                           player-b="PLAYER72" player-a="PLAYER-1" scheduled="22.05.2013 21:17:45" group="Runde 1" nr="1"/>

        Dim SpielerA = New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Id = "PLAYER-1", .Fremd = True}
        Dim SpielerB = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER72"}
        
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

    <TestMethod>
    Public Sub SpielPartie_From_XML()
        Dim MatchXml = <match games-b="23" matches-b="0" sets-b="0" games-a="33" matches-a="1"
                           sets-a="3" set-b-7="0" set-b-6="0" set-b-5="0" set-b-4="0" set-b-3="9" set-b-2="0" set-b-1="5"
                           set-a-7="0" set-a-6="0" set-a-5="0" set-a-4="0" set-a-3="11" set-a-2="11" set-a-1="11"
                           player-b="PLAYER299" player-a="PLAYER293" scheduled="22.05.2013 21:17:45" group=" Gruppe 01" nr="5"/>

        Dim SpielerA = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"}
        Dim SpielerB = New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Id = "PLAYER299"}

        Dim Partie = SpielPartie.FromXML(New Spieler() {SpielerA, SpielerB}, MatchXml)
        With Partie
            Assert.AreEqual(3, .MeineGewonnenenSätze(SpielerA).Count)
            Assert.AreEqual(0, .MeineGewonnenenSätze(SpielerB).Count)
            CollectionAssert.AreEqual({5, 0, 9}, .Select(Function(x) x.PunkteRechts).ToList)
            CollectionAssert.AreEqual({11, 11, 11}, .Select(Function(x) x.PunkteLinks).ToList)
            Assert.AreEqual(SpielerB, .MeinGegner(SpielerA))
            Assert.AreEqual(SpielerA, .MeinGegner(SpielerB))
        End With
    End Sub

    <TestMethod>
    Sub Runden_To_XML()
        MainWindow.AktiveCompetition = New Competition
        With MainWindow.AktiveCompetition
            .DateiPfad = "D:\dummy.xml"
            .Altersgruppe = "Mädchen U 13"
            .Gewinnsätze = 4
            .SatzDifferenz = False
            .StartDatum = "2012-09-08 11:00"
            .SpielerListe = New SpielerListe From {
                New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"},
                New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Id = "PLAYER299"},
                New Spieler With {.Vorname = "Alec", .Nachname = "Baldwin", .Id = "PLAYER33"},
                New Spieler With {.Vorname = "Mahatma", .Nachname = "Gandhi", .Id = "PLAYER77"}
            }
            Dim spieler = .SpielerListe
            .SpielRunden = New SpielRunden
            With .SpielRunden
                Dim runde = New SpielRunde With {.AusgeschiedeneSpieler = New ObservableCollection(Of Spieler) From
                                            {spieler(3)}}
                runde.Add(New FreiLosSpiel(spieler(2)))
                runde.Add(New SpielPartie(spieler(0), spieler(1)) With {.ZeitStempel = Date.Parse("22.05.2013 21:17:45", Globalization.CultureInfo.GetCultureInfo("de"))})
                .Push(runde)
            End With

            Dim spielPartien As IEnumerable(Of XElement) = .SpielRunden.ToXML()
            Dim Dummy = <Dummy xmlns:ppc="http://www.ttc-langensteinbach.de">
                            <%= spielPartien %>
                        </Dummy>
            Assert.AreEqual(3, spielPartien.Count)
            Assert.AreEqual("PLAYER33", Dummy.<ppc:freematch>.Single.@player)
            Assert.AreEqual("PLAYER77", Dummy.<ppc:inactiveplayer>.Single.@player)
            Assert.AreEqual("PLAYER293", Dummy.<match>.Single.Attribute("player-a").Value)
            Assert.AreEqual("PLAYER299", Dummy.<match>.Single.Attribute("player-b").Value)

            Assert.AreEqual(spielPartien(0).ToString, <ppc:freematch player="PLAYER33" group="Runde 1"/>.ToString)
            Assert.AreEqual(spielPartien(1).ToString, <match player-a="PLAYER293" player-b="PLAYER299"
                                                          games-a="0" games-b="0" sets-a="0" sets-b="0"
                                                          matches-a="0" matches-b="0" scheduled="22.05.2013 21:17:45" group="Runde 1" nr="2"
                                                          set-a-1="0" set-a-2="0" set-a-3="0" set-a-4="0" set-a-5="0" set-a-6="0" set-a-7="0"
                                                          set-b-1="0" set-b-2="0" set-b-3="0" set-b-4="0" set-b-5="0" set-b-6="0" set-b-7="0"/>.ToString)
            Assert.AreEqual(spielPartien(2).ToString, <ppc:inactiveplayer player="PLAYER77" group="Runde 1"/>.ToString)

        End With
    End Sub

    <TestMethod>
    Sub SpielePartie_To_XML()
        Dim SpielerA = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"}
        Dim SpielerB = New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Id = "PLAYER299"}

        Dim Partie = New SpielPartie(SpielerA, SpielerB)
        Partie.ZeitStempel = Date.Parse("22.05.2013 21:17:45", Globalization.CultureInfo.GetCultureInfo("de"))
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 5})
        Partie.Add(New Satz With {.PunkteLinks = 6, .PunkteRechts = 11})
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 3})


        Dim MatchXml = <match player-a="PLAYER293" player-b="PLAYER299" games-a="28" games-b="19" sets-a="2" sets-b="1"
                           matches-a="1" matches-b="0" scheduled="22.05.2013 21:17:45" group="Runde 1" nr="5" set-a-1="11" set-a-2="6"
                           set-a-3="11" set-a-4="0" set-a-5="0" set-a-6="0" set-a-7="0" set-b-1="5"
                           set-b-2="11" set-b-3="3" set-b-4="0" set-b-5="0" set-b-6="0" set-b-7="0"/>

        Dim result = Partie.ToXML("Runde 1", 5)

        Assert.AreEqual("Runde 1", result.@group)
        Assert.AreEqual("19", result.Attribute("games-b").Value)
        Assert.AreEqual("2", result.Attribute("sets-a").Value)
        Assert.IsTrue(XElement.DeepEquals(MatchXml, result))

    End Sub

    <TestMethod>
    Sub FremdPartie_To_XML()
        Dim SpielerA = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"}
        Dim SpielerB = New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Id = "PLAYER299", .Fremd = True}

        Dim Partie = New SpielPartie(SpielerA, SpielerB)
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 5})
        Partie.Add(New Satz With {.PunkteLinks = 6, .PunkteRechts = 11})
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 3})
        Partie.ZeitStempel = Date.Parse("22.05.2013 21:17:45", Globalization.CultureInfo.GetCultureInfo("de"))

        Dim MatchXml = <ppc:match player-a="PLAYER293" player-b="PLAYER299" games-a="28" games-b="19" sets-a="2" sets-b="1"
                           matches-a="1" matches-b="0" scheduled="22.05.2013 21:17:45" group="Runde 1" nr="5" set-a-1="11" set-a-2="6"
                           set-a-3="11" set-a-4="0" set-a-5="0" set-a-6="0" set-a-7="0" set-b-1="5"
                           set-b-2="11" set-b-3="3" set-b-4="0" set-b-5="0" set-b-6="0" set-b-7="0"/>

        Dim result = Partie.ToXML("Runde 1", 5)

        Assert.AreEqual("Runde 1", result.@group)
        Assert.AreEqual("19", result.Attribute("games-b").Value)
        Assert.AreEqual("2", result.Attribute("sets-a").Value)
        Assert.IsTrue(XElement.DeepEquals(MatchXml, result))

    End Sub

    <TestMethod>
    Sub FreiSpiel_To_XML()
        Dim SpielerA = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"}
        Dim Partie = New FreiLosSpiel(SpielerA)
        Assert.AreEqual(Partie.SpielerLinks, SpielerA)
        Assert.AreEqual(Partie.SpielerRechts, SpielerA)
    End Sub

    <TestMethod()> Public Sub TTRGültig()
        Dim doc = XDocument.Parse(My.Resources.Turnierteilnehmer)
        For Each person In doc...<person>
            Assert.IsNotNull(person.@ttr)
        Next
    End Sub

End Class