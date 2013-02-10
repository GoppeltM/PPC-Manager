Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class Paket_Tests

    <TestMethod()> Public Sub Runde_1()
        Dim doc = XDocument.Parse(My.Resources.Turnierteilnehmer)
        MainWindow.AktiveCompetition = New Competition

        Dim SpielerListe = From x In doc.Root.<competition> Where x.Attribute("age-group").Value = "Herren C"
                        From y In x...<player> Select Spieler.FromXML(y)

        Dim SpielPartien = PaketBildung.organisierePakete(SpielerListe.ToList, 1)
        Assert.IsTrue(SpielPartien.Any)
    End Sub

End Class
