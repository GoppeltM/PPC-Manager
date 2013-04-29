<TestClass>
Public Class PPC15_Turnier_Klasse_D


    <TestMethod>
    Sub SpielerAnmeldungen()
        Dim KlassementD = (From x In XDocument.Parse(My.Resources.PPC_15_Anmeldungen).Root.<competition>
                           Where x.Attribute("age-group").Value = "D-Klasse").First
        Dim reference = Competition.FromXML("D:\dummy.xml", KlassementD, 3, False)
        Assert.AreEqual(56, reference.SpielerListe.Count)

        MainWindow.AktiveCompetition = reference
        For Each Spieler In reference.SpielerListe
            Assert.IsTrue(Spieler.Fremd)
        Next
    End Sub

End Class
