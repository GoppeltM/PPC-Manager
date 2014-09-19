Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports Microsoft.Office.Interop.Excel

<TestClass()> Public Class StartListe

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
        Dim s As New Spieler(c) With {.Vorname = "Marius", .Nachname = "Goppelt"}        
        ExcelInterface.CreateFile("E:\Skydrive\Dokumente\Repositories\Programme\Trunk\PPC Manager\PPC Manager Tests\Resources\PPC 15 Anmeldungen D-Klasse.xlsx", _
                                  New Spieler() {s}, c)
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
                Dim Text = DirectCast(column.Cells(1), Range).Text.ToString
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
                Dim Geburtsjahr = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Geb.J")), Range).Text

                SpielerImKlassement.Add(<ppc:player id=<%= "PLAYER" & AktuellerIndex %>>
                                            <person licence-nr=<%= AktuellerIndex %> club-name=<%= Verein %>
                                                sex=<%= Geschlecht %> ttr-match-count="0" lastname=<%= Nachname %>
                                                ttr=<%= TTR %> firstname=<%= Vorname %> birthyear=<%= Geburtsjahr %>>
                                            </person>
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
        Turnier.Save("E:\Skydrive\Dokumente\Repositories\Programme\Trunk\PPC Manager\PPC Manager Tests\Resources\PPC 15 Anmeldungen.xml")
        excel.Quit()
    End Sub

    <Ignore>
    <TestMethod>
    Sub Ergebnisse_Import()
        Dim excel As New Application
        Dim wb = excel.Workbooks.Open("D:\Dropbox\Ping Pong Turnier\Einzelergebnisse_PPC15.xlsx")
        Dim Ergebnisse = <Ergebnisse/>


        Dim AktuellerIndex = -1

        For Each sheet As Worksheet In wb.Sheets
            If sheet.Name <> "Tabelle1" Then Continue For

            Dim ColumnNames As New Dictionary(Of String, Integer)

            For Each column As Range In sheet.UsedRange.Columns
                Dim Text = DirectCast(column.Cells(1), Range).Text.ToString
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
                Dim NameLinks = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Name_links")), Range).Text
                If NameLinks.ToString = "" Then Exit For
                Dim NameRechts = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Name_rechts")), Range).Text
                Dim VornameLinks = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Vorname_links")), Range).Text
                Dim VornameRechts = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Vorname_rechts")), Range).Text
                Dim Runde = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Rd")), Range).Text
                Dim ErgebnisLinks = DirectCast(row.Cells(ColumnIndex:=ColumnNames("g_sätze")), Range).Text
                Dim ErgebnisRechts = DirectCast(row.Cells(ColumnIndex:=ColumnNames("v_sätze")), Range).Text
                Dim Klassement = DirectCast(row.Cells(ColumnIndex:=ColumnNames("Kl")), Range).Text

                Ergebnisse.Add(<Ergebnis Runde=<%= Runde %> Klassement=<%= Klassement %>
                                   VornameA=<%= VornameLinks %> VornameB=<%= VornameRechts %>
                                   SpielerA=<%= NameLinks %> SpielerB=<%= NameRechts %>
                                   SätzeA=<%= ErgebnisLinks %> SätzeB=<%= ErgebnisRechts %>/>)
            Next
        Next

        Dim GruppiertNachKlassement = From x In Ergebnisse.<Ergebnis> Group x By x.@Klassement Into Group


        Dim ErgebnisseStrukturiert = <Ergebnisse/>
        For Each Klassement In GruppiertNachKlassement
            Dim GruppiertNachRunde = From x In Klassement.Group Group x By x.@Runde Into Group

            Dim XKlasse = <Klassement Name=<%= Klassement.Klassement %>
                              <%= From x In GruppiertNachRunde Select <Runde Nummer=<%= x.Runde %>>
                                                                          <%= x.Group %>
                                                                      </Runde> %>
                          />
            ErgebnisseStrukturiert.Add(XKlasse)
        Next
        For Each Runde In ErgebnisseStrukturiert...<Ergebnis>
            Runde.@Klassement = Nothing
            Runde.@Runde = Nothing
        Next


        'Dim Gruppe = 
        ErgebnisseStrukturiert.Save("E:\Eigene Dateien - Marius\Dokumente\Repositories\Programme\Trunk\PPC Manager\PPC Manager Tests\Resources\PPC 15 Ergebnisse.xml")
        excel.Quit()
    End Sub


End Class