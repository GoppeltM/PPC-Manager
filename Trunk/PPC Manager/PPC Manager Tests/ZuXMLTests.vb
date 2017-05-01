Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class ZuXMLTests
    <Test>
    Public Sub ToXML_mit_leeren_SpielRunden_ist_leer()
        Dim s As New SpielRunden()
        Dim ergebnis = ZuXML.ToXML(s, Mock.Of(Of ISpielstand))
        Assert.That(ergebnis, [Is].Empty)
    End Sub


    <Test>
    Sub Runden_To_XML()
        Dim runden = New SpielRunden

        Dim spieler = New List(Of SpielerInfo) From {
                New SpielerInfo("PLAYER293") With {.Vorname = "Florian", .Nachname = "Ewald"},
                New SpielerInfo("PLAYER299") With {.Vorname = "Marius", .Nachname = "Goppelt"},
                New SpielerInfo("PLAYER33") With {.Vorname = "Alec", .Nachname = "Baldwin"},
                New SpielerInfo("PLAYER77") With {.Vorname = "Mahatma", .Nachname = "Gandhi"}
            }


        Dim runde = New SpielRunde
        runde.AusgeschiedeneSpielerIDs.Add(spieler(3).Id)
        Dim Zeitstempel = Date.Parse("22.05.2013 21:17:45", Globalization.CultureInfo.GetCultureInfo("de"))
        runde.Add(New FreiLosSpiel("Runde 1", spieler(2)) With {.ZeitStempel = Zeitstempel})
        runde.Add(New SpielPartie("Runde 1", spieler(0), spieler(1)) With {.ZeitStempel = Zeitstempel})
        runden.Push(runde)

        Dim spielPartien As IEnumerable(Of XElement) = ZuXML.ToXML(runden, Mock.Of(Of ISpielstand))
        Dim Dummy = <Dummy xmlns:ppc="http://www.ttc-langensteinbach.de">
                        <%= spielPartien %>
                    </Dummy>
        Assert.AreEqual(3, spielPartien.Count)
        Assert.AreEqual("PLAYER33", Dummy.<ppc:freematch>.Single.@player)
        Assert.AreEqual("PLAYER77", Dummy.<ppc:inactiveplayer>.Single.@player)
        Assert.AreEqual("PLAYER293", Dummy.<match>.Single.Attribute("player-a").Value)
        Assert.AreEqual("PLAYER299", Dummy.<match>.Single.Attribute("player-b").Value)

        Assert.That(spielPartien(0).ToString, [Is].EqualTo(<ppc:freematch player="PLAYER33" group="Runde 1" scheduled="2013-05-22 21:17"/>.ToString))
        Assert.That(spielPartien(1).ToString, [Is].EqualTo(<match player-a="PLAYER293" player-b="PLAYER299"
                                                               games-a="0" games-b="0" sets-a="0" sets-b="0"
                                                               matches-a="0" matches-b="0" scheduled="2013-05-22 21:17" group="Runde 1" nr="2"
                                                               set-a-1="0" set-a-2="0" set-a-3="0" set-a-4="0" set-a-5="0" set-a-6="0" set-a-7="0"
                                                               set-b-1="0" set-b-2="0" set-b-3="0" set-b-4="0" set-b-5="0" set-b-6="0" set-b-7="0"/>.ToString))
        Assert.That(spielPartien(2).ToString, [Is].EqualTo(<ppc:inactiveplayer player="PLAYER77" group="0"/>.ToString))

    End Sub

    <Test>
    Sub SpielePartie_To_XML()
        Dim SpielerA = New SpielerInfo("PLAYER293") With {.Vorname = "Florian", .Nachname = "Ewald"}
        Dim SpielerB = New SpielerInfo("PLAYER299") With {.Vorname = "Marius", .Nachname = "Goppelt"}
        Dim Partie = New SpielPartie("Runde 1", SpielerA, SpielerB)
        Partie.ZeitStempel = Date.Parse("22.05.2013 21:17:45", Globalization.CultureInfo.GetCultureInfo("de"))
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 5})
        Partie.Add(New Satz With {.PunkteLinks = 6, .PunkteRechts = 11})
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 3})


        Dim MatchXml = <match player-a="PLAYER293" player-b="PLAYER299" games-a="28" games-b="19" sets-a="2" sets-b="1"
                           matches-a="1" matches-b="0" scheduled="2013-05-22 21:17" group="Runde 1" nr="5" set-a-1="11" set-a-2="6"
                           set-a-3="11" set-a-4="0" set-a-5="0" set-a-6="0" set-a-7="0" set-b-1="5"
                           set-b-2="11" set-b-3="3" set-b-4="0" set-b-5="0" set-b-6="0" set-b-7="0"/>

        Dim spielstand = Mock.Of(Of ISpielstand)(Function(x) x.MeineGewonnenenSätze(Partie, Partie.SpielerLinks) = 2 _
                                                     AndAlso x.MeineGewonnenenSätze(Partie, Partie.SpielerRechts) = 1)
        Dim result = ZuXML.PartieZuXML(Partie, spielstand, 5)

        Assert.AreEqual("Runde 1", result.@group)
        Assert.AreEqual("19", result.Attribute("games-b").Value)
        Assert.AreEqual("2", result.Attribute("sets-a").Value)
        Assert.That(XNode.DeepEquals(MatchXml, result))

    End Sub

    <Test>
    Sub FremdPartie_To_XML()
        Dim SpielerA = New SpielerInfo("PLAYER293") With {.Vorname = "Florian", .Nachname = "Ewald"}
        Dim SpielerB = New SpielerInfo("PLAYER299") With {.Vorname = "Marius", .Nachname = "Goppelt", .Fremd = True}

        Dim Partie = New SpielPartie("Runde 1", SpielerA, SpielerB)
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 5})
        Partie.Add(New Satz With {.PunkteLinks = 6, .PunkteRechts = 11})
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 3})
        Partie.ZeitStempel = Date.Parse("22.05.2013 21:17:45", Globalization.CultureInfo.GetCultureInfo("de"))

        Dim MatchXml = <ppc:match player-a="PLAYER293" player-b="PLAYER299" games-a="28" games-b="19" sets-a="2" sets-b="1"
                           matches-a="1" matches-b="0" scheduled="2013-05-22 21:17" group="Runde 1" nr="5" set-a-1="11" set-a-2="6"
                           set-a-3="11" set-a-4="0" set-a-5="0" set-a-6="0" set-a-7="0" set-b-1="5"
                           set-b-2="11" set-b-3="3" set-b-4="0" set-b-5="0" set-b-6="0" set-b-7="0"/>
        Dim spielstand = Mock.Of(Of ISpielstand)(Function(x) x.MeineGewonnenenSätze(Partie, Partie.SpielerLinks) = 2 _
                                                     AndAlso x.MeineGewonnenenSätze(Partie, Partie.SpielerRechts) = 1)

        Dim result = ZuXML.PartieZuXML(Partie, spielstand, 5)

        Assert.AreEqual("Runde 1", result.@group)
        Assert.AreEqual("19", result.Attribute("games-b").Value)
        Assert.AreEqual("2", result.Attribute("sets-a").Value)
        Assert.IsTrue(XNode.DeepEquals(MatchXml, result))

    End Sub

End Class
