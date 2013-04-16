Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de/">
Imports Microsoft.Office.Interop.Excel

<TestClass()> Public Class StartListe

    <TestMethod> Sub FremdSpieler_Erzeugen()
        Dim XMLKnoten =
            <tournament>
                <competition age-group="Herren D">
                    <players>
                        <ppc:player type="single">
                            <person club-name="VfR Rheinsheim" sex="1" ttr-match-count="0" lastname="Akin" ttr="1299" firstname="Mikail"/>
                        </ppc:player>
                    </players>
                </competition>
                <competition age-group="Herren D">
                    <players>
                        <ppc:player type="single">
                            <person club-name="VfR Rheinsheim" sex="1" ttr-match-count="0" lastname="Akin" ttr="1299" firstname="Mikail"/>
                        </ppc:player>
                    </players>
                </competition>
            </tournament>...<ppc:player>

        Dim FremdSpieler = StartlistenEditor.Spieler.FromXML(XMLKnoten)
        With FremdSpieler
            Assert.IsTrue(FremdSpieler.Fremd)
            Assert.AreEqual(0, .TTRMatchCount)
            Assert.AreEqual(1, .Geschlecht)
            Assert.AreEqual(1299, .TTR)
            Assert.AreEqual("Mikail", .Vorname)
            Assert.AreEqual("Akin", .Nachname)
        End With
    End Sub

    <TestMethod> Sub Alle_Spieler_Importieren()
        Dim doc = XDocument.Parse(My.Resources.Turnierteilnehmer)

        Dim AlleSpieler = StartlistenEditor.MainWindow.XmlZuSpielerListe(doc)

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
        Dim AlleSpieler = StartlistenEditor.MainWindow.XmlZuSpielerListe(XDocument.Parse(My.Resources.Competition))
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
    Sub Spielerliste_Import()
        Dim excel As New Application
        Dim wb = excel.Workbooks.Open("D:\Dropbox\Ping Pong Turnier\PPC15_Anmeldungen.xls")

        Dim Turnier = <tournament end-date="2012-09-09" start-date="2012-09-08"
                          name="15. Turnier nach Schweizer Art " tournament-id="Fp12HjV93pTyQlVex23%2FfGT919cb58I9">
                      </tournament>


        Dim AktuellerIndex = -1

        For Each sheet As Worksheet In wb.Sheets

            Dim ColumnNames As New Dictionary(Of String, Integer)

            For Each column As Range In sheet.UsedRange.Columns
                Dim Text = column.Cells(1).Text
                If Not ColumnNames.ContainsKey(Text) Then
                    ColumnNames.Add(Text, column.Column)
                End If
            Next

            Dim SpielerImKlassement As New List(Of XElement)
            Dim SkipCount = 2

            For Each row As Range In sheet.UsedRange.Rows
                If SkipCount <> 0 Then
                    SkipCount -= 1
                    Continue For
                End If
                Dim Vorname = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Vorname")), Range).Text
                If Vorname.ToString = "" Then Exit For
                Dim Nachname = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Nachname")), Range).Text
                Dim Verein = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Verein")), Range).Text
                Dim TTR = DirectCast(row.Cells(ColumnIndex:=ColumnNames("TTR")), Range).Text
                Dim Geschlecht = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Geschl.")), Range).Text
                If Geschlecht.ToString = "M" Then Geschlecht = 1 Else Geschlecht = 0

                SpielerImKlassement.Add(<ppc:player id=<%= "PLAYER" & AktuellerIndex %>>
                                            <person licence-nr=<%= AktuellerIndex %> club-name=<%= Verein %>
                                                sex=<%= Geschlecht %> ttr-match-count="0" lastname=<%= Nachname %>
                                                ttr=<%= TTR %> firstname=<%= Vorname %>/>
                                        </ppc:player>)

                AktuellerIndex -= 1
            Next

            Dim Klassement = <competition start-date="2012-09-09 8:00" ttr-remarks="-"
                                 age-group=<%= sheet.Name %> type="Einzel">
                                 <players>
                                     <%= SpielerImKlassement %>
                                 </players>
                             </competition>
            Turnier.Add(Klassement)

        Next
        Turnier.Save("E:\Eigene Dateien - Marius\Dokumente\Repositories\Programme\Trunk\PPC Manager\PPC Manager Tests\Resources\PPC 15 Anmeldungen.xml")
        excel.Quit()
    End Sub


End Class