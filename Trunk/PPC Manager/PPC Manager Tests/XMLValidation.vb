Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports PPC_Manager
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports System.Collections.ObjectModel
Imports System.Xml.Schema
Imports System.Xml

<TestClass()> Public Class XMLValidation

    <TestMethod>
    Public Sub Competition_From_XML()
        Dim reference = Competition.FromXML("D:\dummy.xml", XDocument.Parse(My.Resources.Competition).Root.<competition>.First, 4, False, False)
        MainWindow.AktiveCompetition = reference
        With reference
            Assert.AreEqual("D:\dummy.xml", .DateiPfad)
            Assert.AreEqual(4, .Gewinnsätze)
            Assert.AreEqual(False, .SatzDifferenz)
            Assert.AreEqual("2012-09-08 11:00", .StartDatum)
            Assert.AreEqual("Mädchen U 13", .Altersgruppe)
            Assert.AreEqual(4, .SpielerListe.Count)
            Assert.AreEqual(2, .SpielRunden.Count)
            Assert.AreEqual(1, .SpielRunden.Peek.OfType(Of FreiLosSpiel).Count)
            CollectionAssert.AreEqual({"PLAYER72", "PLAYER126", "PLAYER127", "PLAYER-1"}, (From x In .SpielerListe Select x.Id).ToList)
        End With
    End Sub

    <TestMethod>
    Sub Competition_ErsteRunde_From_XML()
        Dim reference = Competition.FromXML("D:\dummy.xml", XDocument.Parse(My.Resources.Competition).Root.<competition>.First, 4, False, False)
        MainWindow.AktiveCompetition = reference


        CollectionAssert.AreEqual({"PLAYER127"}, (From x In reference.SpielRunden.AusgeschiedeneSpieler Select x.Spieler.Id).ToList)
        Dim ersteRunde = reference.SpielRunden.Last

        Assert.AreEqual(2, ersteRunde.Count, "Zwei Spielpartien erwartet")

        With ersteRunde
            For Each partie In .ToList
                Assert.AreEqual(3, partie.Count)
            Next
        End With

        Dim zweiteRunde = reference.SpielRunden.Peek
        With zweiteRunde
            Assert.IsInstanceOfType(zweiteRunde.Single, GetType(FreiLosSpiel))
        End With

    End Sub

    <TestMethod>
    Sub Spielrunden_From_XML()
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
        Dim RundenRes = SpielRunden.FromXML(New SpielerListe From {New Spieler With {.Id = "PLAYER127"}, New Spieler With {.Id = "PLAYER126"},
                                                                   New Spieler With {.Id = "PLAYER72"}, New Spieler With {.Id = "PLAYER-1"}}, rundenRef)
        With RundenRes
            CollectionAssert.AreEqual({"PLAYER127"}.ToList, (From x In .AusgeschiedeneSpieler Select x.Spieler.Id).ToList)
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
            Assert.AreEqual(1981, .Geburtsjahr)
            Assert.AreEqual(1, .Geschlecht)
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
                .AusgeschiedeneSpieler = New ObservableCollection(Of Ausgeschieden) From {New Ausgeschieden With {.Spieler = spieler(3), .Runde = 1}}
                Dim runde = New SpielRunde
                Dim Zeitstempel = Date.Parse("22.05.2013 21:17:45", Globalization.CultureInfo.GetCultureInfo("de"))
                runde.Add(New FreiLosSpiel("Runde 1", spieler(2)) With {.ZeitStempel = Zeitstempel})
                runde.Add(New SpielPartie("Runde 1", spieler(0), spieler(1)) With {.ZeitStempel = Zeitstempel})
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

            Assert.AreEqual(spielPartien(0).ToString, <ppc:freematch player="PLAYER33" group="Runde 1" scheduled="2013-05-22 21:17"/>.ToString)
            Assert.AreEqual(spielPartien(1).ToString, <match player-a="PLAYER293" player-b="PLAYER299"
                                                          games-a="0" games-b="0" sets-a="0" sets-b="0"
                                                          matches-a="0" matches-b="0" scheduled="2013-05-22 21:17" group="Runde 1" nr="2"
                                                          set-a-1="0" set-a-2="0" set-a-3="0" set-a-4="0" set-a-5="0" set-a-6="0" set-a-7="0"
                                                          set-b-1="0" set-b-2="0" set-b-3="0" set-b-4="0" set-b-5="0" set-b-6="0" set-b-7="0"/>.ToString)
            Assert.AreEqual(spielPartien(2).ToString, <ppc:inactiveplayer player="PLAYER77" group="1"/>.ToString)

        End With
    End Sub

    <TestMethod>
    Sub SpielePartie_To_XML()
        Dim SpielerA = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"}
        Dim SpielerB = New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Id = "PLAYER299"}

        Dim Partie = New SpielPartie("Runde 1", SpielerA, SpielerB)
        Partie.ZeitStempel = Date.Parse("22.05.2013 21:17:45", Globalization.CultureInfo.GetCultureInfo("de"))
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 5})
        Partie.Add(New Satz With {.PunkteLinks = 6, .PunkteRechts = 11})
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 3})


        Dim MatchXml = <match player-a="PLAYER293" player-b="PLAYER299" games-a="28" games-b="19" sets-a="2" sets-b="1"
                           matches-a="1" matches-b="0" scheduled="2013-05-22 21:17" group="Runde 1" nr="5" set-a-1="11" set-a-2="6"
                           set-a-3="11" set-a-4="0" set-a-5="0" set-a-6="0" set-a-7="0" set-b-1="5"
                           set-b-2="11" set-b-3="3" set-b-4="0" set-b-5="0" set-b-6="0" set-b-7="0"/>

        Dim result = Partie.ToXML(5)

        Assert.AreEqual("Runde 1", result.@group)
        Assert.AreEqual("19", result.Attribute("games-b").Value)
        Assert.AreEqual("2", result.Attribute("sets-a").Value)
        Assert.IsTrue(XElement.DeepEquals(MatchXml, result))

    End Sub

    <TestMethod>
    Sub FremdPartie_To_XML()
        Dim SpielerA = New Spieler With {.Vorname = "Florian", .Nachname = "Ewald", .Id = "PLAYER293"}
        Dim SpielerB = New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Id = "PLAYER299", .Fremd = True}

        Dim Partie = New SpielPartie("Runde 1", SpielerA, SpielerB)
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 5})
        Partie.Add(New Satz With {.PunkteLinks = 6, .PunkteRechts = 11})
        Partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 3})
        Partie.ZeitStempel = Date.Parse("22.05.2013 21:17:45", Globalization.CultureInfo.GetCultureInfo("de"))

        Dim MatchXml = <ppc:match player-a="PLAYER293" player-b="PLAYER299" games-a="28" games-b="19" sets-a="2" sets-b="1"
                           matches-a="1" matches-b="0" scheduled="2013-05-22 21:17" group="Runde 1" nr="5" set-a-1="11" set-a-2="6"
                           set-a-3="11" set-a-4="0" set-a-5="0" set-a-6="0" set-a-7="0" set-b-1="5"
                           set-b-2="11" set-b-3="3" set-b-4="0" set-b-5="0" set-b-6="0" set-b-7="0"/>

        Dim result = Partie.ToXML(5)

        Assert.AreEqual("Runde 1", result.@group)
        Assert.AreEqual("19", result.Attribute("games-b").Value)
        Assert.AreEqual("2", result.Attribute("sets-a").Value)
        Assert.IsTrue(XElement.DeepEquals(MatchXml, result))

    End Sub



    <TestMethod()> Public Sub TTRGültig()
        Dim doc = XDocument.Parse(My.Resources.Turnierteilnehmer)
        For Each person In doc...<person>
            Assert.IsNotNull(person.@ttr)
        Next
    End Sub

    <Ignore>
    <TestMethod()>
    Public Sub BereinigeNamespaces()
        Dim doc = XDocument.Load("D:\Eigene Dateien - Marius\Desktop\Turnierteilnehmer_mu13_2013_test.xml")
        Dim NodesToRemove = From x In doc.Root.Descendants Where x.Name.NamespaceName = "http://www.ttc-langensteinbach.de"

        Dim AttributesToRemove = From x In doc.Root.Descendants
                                 From y In x.Attributes
                                 Where y.Name.NamespaceName = "http://www.ttc-langensteinbach.de" Or
                                 y.Value = "http://www.ttc-langensteinbach.de" Select y

        For Each attr In AttributesToRemove.ToList
            attr.Remove()
        Next

        For Each node In NodesToRemove.ToList
            node.Remove()
        Next

    End Sub

    <Ignore>
    <TestMethod>
    Public Sub FügeSchemaHinzu()
        Dim doc = XDocument.Load("D:\Eigene Dateien - Marius\Desktop\Turnierteilnehmer_mu13_2013_test.xml")
        Dim schema As XmlSchema
        Using stream = New IO.FileStream("E:\Skydrive\Dokumente\Repositories\Programme\Trunk\PPC Manager\PPC Manager\SpeicherStandSchema.xsd", IO.FileMode.Open)
            schema = XmlSchema.Read(stream, Nothing)
        End Using

        Dim schemaSet As New XmlSchemaSet
        schemaSet.Add(schema)

        schemaSet.Compile()

        doc.Validate(schemaSet, Nothing)

        Using s As New IO.MemoryStream
            schema.Write(s)
            s.Position = 0
            Dim schemaDoc = XDocument.Load(s) '"D:\Blubb.xml")
            doc.Root.Add(schemaDoc.Root)
        End Using

        doc.Save("D:\Eigene Dateien - Marius\Desktop\Turnierteilnehmer_mu13_2013_test_Schema.xml")
    End Sub

    <TestMethod>
    Sub DatumSetzen()
        Dim x = Date.Now.ToString("yyyy-MM-dd HH:mm")
    End Sub


End Class