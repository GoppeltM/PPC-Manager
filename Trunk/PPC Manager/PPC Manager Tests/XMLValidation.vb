Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports PPC_Manager

<TestClass()> Public Class XMLValidation

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

End Class