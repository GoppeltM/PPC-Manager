Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports PPC_Manager

<TestClass()> Public Class XMLValidation

    <TestMethod()> Public Sub Tournament_To_XML()
        With New PPC_Manager.MainWindow
            .SpielerListe.Add(New Spieler() With {.Vorname = "Marius", .Nachname = "Goppelt", .Geschlecht = 0, .Geburtsjahr = 1981})
            .SpielerListe.Add(New Spieler() With {.Vorname = "Flo", .Nachname = "Ewald", .Geschlecht = 0, .Geburtsjahr = 1981})

            Dim RefDoc = .getXmlDocument()
        End With


    End Sub

    <TestMethod>
    Public Sub Competition_To_XML()
        Dim reference = New XElement(<competition start-date="2012-09-08 11:00" ttr-remarks="-" age-group="Mädchen U 13" type="Einzel">
                                         <players/>
                                     </competition>)

        Dim result = New Competition() With {
        .StartDatum = "2012-09-08 11:00",
        .ttrRemarks = "-",
        .Altersgruppe = "Mädchen U 13",
        .Typ = "Einzel"
        }.ToXML

        Assert.IsTrue(XElement.DeepEquals(reference, result))

    End Sub

    <TestMethod>
    Public Sub Competition_From_XML()
        Dim reference = Competition.FromXML(<competition start-date="2012-09-08 11:00" ttr-remarks="-" age-group="Mädchen U 13" type="Einzel">
                                                <players>

                                                </players>
                                            </competition>)

        With reference
            Assert.AreEqual("2012-09-08 11:00", .StartDatum)
            Assert.AreEqual("-", .ttrRemarks)
            Assert.AreEqual("Mädchen U 13", .Altersgruppe)
            Assert.AreEqual("Einzel", .Typ)
        End With
    End Sub

End Class