Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

<TestClass()>
Public Class StartListe

    <TestMethod>
    <Ignore>
    Sub UIDummy_Starten()
        Dim controller = New ControllerStub()
        Dim mainWindow As New StartlistenEditor.MainWindow(controller)
        mainWindow.ShowDialog()
    End Sub

    <TestMethod> Sub FremdSpieler_Erzeugen()
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

    <TestMethod> Sub Alle_Spieler_Importieren()
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

    <TestMethod> Sub Competition_Importieren()
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
    <TestMethod>
    Sub ExcelExport()
        Dim c = New Competition(New SpielRegeln(0, True, True))
        Dim s As New Spieler(c.SpielRunden, c.SpielRegeln) With {.Vorname = "Marius", .Nachname = "Goppelt"}
        ExcelInterface.CreateFile("E:\Skydrive\Dokumente\Repositories\Programme\Trunk\PPC Manager\PPC Manager Tests\Resources\PPC 15 Anmeldungen D-Klasse.xlsx", _
                                  New Spieler() {s}, c)
    End Sub

End Class