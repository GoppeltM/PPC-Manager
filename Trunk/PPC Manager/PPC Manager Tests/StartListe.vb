Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de/">

<TestClass()> Public Class StartListe

    <TestMethod> Sub FremdSpieler_Erzeugen()
        Dim XMLKnoten = <ppc:player type="single">
                            <person club-name="VfR Rheinsheim" sex="1" ttr-match-count="0" lastname="Akin" ttr="1299" firstname="Mikail"/>
                        </ppc:player>
        Dim FremdSpieler = StartlistenEditor.Spieler.FromXML(New XElement() {XMLKnoten, XMLKnoten}, New String() {"Herren D", "Herren A"})
        With FremdSpieler
            Assert.IsTrue(FremdSpieler.Fremd)
            Assert.AreEqual(0, .TTRMatchCount)
            Assert.AreEqual(1, .Geschlecht)
            Assert.AreEqual(1299, .TTR)
            Assert.AreEqual("Mikail", .Vorname)
            Assert.AreEqual("Akin", .Nachname)
        End With
    End Sub
    

End Class