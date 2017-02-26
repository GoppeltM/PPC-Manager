Imports System.Text
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports Moq

<RequiresSTA>
Public Class StartListeTests

    <Explicit, Test>
    Sub UIDummy_Starten()
        Dim SpielerListe = New StartlistenEditor.SpielerListe
        Dim KlassementListe = New StartlistenEditor.KlassementListe

        Dim controller = New Mock(Of StartlistenEditor.IStartlistenController)()
        controller.Setup(Sub(m) m.Öffnend(It.IsAny(Of XDocument), It.IsAny(Of String))).Callback(Sub() Öffnend(SpielerListe, KlassementListe))
        Dim mainWindow As New StartlistenEditor.MainWindow(controller.Object, SpielerListe, KlassementListe)
        mainWindow.ShowDialog()
    End Sub

    Private Sub Öffnend(sl As StartlistenEditor.SpielerListe, k As StartlistenEditor.KlassementListe)
        Dim Doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        Dim AlleSpieler = StartlistenEditor.StartlistenController.XmlZuSpielerListe(Doc)

        Dim AlleKlassements = (From x In Doc.Root.<competition> Select x.Attribute("age-group").Value).Distinct
        With k
            For Each Klassement In AlleKlassements
                .Add(New StartlistenEditor.KlassementName With {.Name = Klassement})
            Next
        End With

        For Each s In AlleSpieler
            sl.Add(s)
        Next
    End Sub


    <Test> Sub FremdSpieler_Erzeugen()
        Dim XMLKnoten =
            <tournament>
                <competition age-group="Herren C">
                    <players>
                        <ppc:player type="single">
                            <person club-name="VfR Rheinsheim" sex="1" ttr-match-count="0" lastname="Akin" ttr="1299" firstname="Mikail"/>
                        </ppc:player>
                    </players>
                </competition>
                <competition age-group="Herren D">
                    <players>
                        <ppc:player type="single">
                            <person club-name="VfR Rheinsheim" sex="1" ttr-match-count="0" lastname="Akin"
                                ttr="1299" firstname="Mikail" ppc:anwesend="true" ppc:abwesend="true" ppc:bezahlt="true"/>
                        </ppc:player>
                    </players>
                </competition>
            </tournament>...<ppc:player>

        Dim FremdSpieler = StartlistenEditor.Spieler.FromXML(XMLKnoten(0))
        With FremdSpieler
            Assert.IsTrue(FremdSpieler.Fremd)
            Assert.AreEqual(0, .TTRMatchCount)
            Assert.AreEqual(1, .Geschlecht)
            Assert.AreEqual(1299, .TTR)
            Assert.AreEqual("Mikail", .Vorname)
            Assert.AreEqual("Akin", .Nachname)
            Assert.AreEqual("Herren C", .Klassement)
            Assert.IsFalse(.Bezahlt)
            Assert.IsFalse(.Anwesend)
            Assert.IsFalse(.Abwesend)
        End With

        FremdSpieler = StartlistenEditor.Spieler.FromXML(XMLKnoten(1))
        With FremdSpieler
            Assert.IsTrue(FremdSpieler.Fremd)
            Assert.AreEqual(0, .TTRMatchCount)
            Assert.AreEqual(1, .Geschlecht)
            Assert.AreEqual(1299, .TTR)
            Assert.AreEqual("Mikail", .Vorname)
            Assert.AreEqual("Akin", .Nachname)
            Assert.AreEqual("Herren D", .Klassement)
            Assert.IsTrue(.Bezahlt)
            Assert.IsTrue(.Anwesend)
            Assert.IsTrue(.Abwesend)
        End With
    End Sub

    <Test> Sub Alle_Spieler_Importieren()
        Dim doc = XDocument.Parse(My.Resources.Turnierteilnehmer)

        Dim AlleSpieler = StartlistenEditor.StartlistenController.XmlZuSpielerListe(doc)

        Assert.IsTrue(AlleSpieler.Any)

        For Each Spieler In AlleSpieler
            Dim g = Spieler.Geschlecht
            Dim vorname = Spieler.Vorname
            Dim nachname = Spieler.Nachname
            Dim LizenzNr = Spieler.LizenzNr
            Dim TTR = Spieler.TTR
            Dim MatchCount = Spieler.TTRMatchCount
        Next

    End Sub

    <Test> Sub Competition_Importieren()
        Dim AlleSpieler = StartlistenEditor.StartlistenController.XmlZuSpielerListe(XDocument.Parse(My.Resources.Competition))
        Assert.IsTrue(AlleSpieler.Any)
        Dim Referenz = {New With {.LizenzNr = 53010, .Fremd = False},
                        New With {.LizenzNr = -1, .Fremd = True},
                        New With {.LizenzNr = 133311, .Fremd = False},
                        New With {.LizenzNr = 54469, .Fremd = False}}
        For Each DatenSatz In AlleSpieler.Zip(Referenz, Function(x, y) New With {.Spieler = x, .Referenz = y})
            Assert.AreEqual(DatenSatz.Referenz.LizenzNr, DatenSatz.Spieler.LizenzNr)
            Assert.AreEqual(DatenSatz.Referenz.Fremd, DatenSatz.Spieler.Fremd)
        Next
    End Sub

    <Ignore>
    <Test>
    Sub ExcelExport()
        Dim c = New Competition(New SpielRegeln(0, True, True))
        c.SpielRunden.Push(New SpielRunde)
        c.SpielRunden.Push(New SpielRunde)
        Dim s As New Spieler(c.SpielRunden, c.SpielRegeln) With {.Vorname = "Marius", .Nachname = "Goppelt"}
        TurnierReport.CreateFile("D:\PPC 15 Anmeldungen D-Klasse.xlsx",
                                  New Spieler() {s}, c)
    End Sub

End Class