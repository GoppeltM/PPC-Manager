Public Class TurnierReportTests

    <Test>
    Public Sub SchreibeNeuePartien_schreibt_Header_und_eine_Zeile_pro_Partie()
        Dim e = New Mock(Of IExcelDokument)
        Dim s = New Mock(Of ISpielstand)
        Dim v = New Mock(Of ISpielverlauf(Of SpielerInfo))

        Dim t = New TurnierReport(e.Object, s.Object, v.Object)
        Dim partien = {
            New SpielPartie("Runde 1",
                            New SpielerInfo("1") With {.Nachname = "Herberger"},
                            New SpielerInfo("2") With {.Nachname = "Mayer"}),
            New SpielPartie("Runde 1",
                            New SpielerInfo("3") With {.Nachname = "Francesco"},
                            New SpielerInfo("4") With {.Nachname = "Smith"})}
        s.Setup(Function(x) x.MeineGewonnenenSätze(partien(0), New SpielerInfo("1"))).Returns(0)
        s.Setup(Function(x) x.MeineGewonnenenSätze(partien(0), New SpielerInfo("2"))).Returns(3)
        s.Setup(Function(x) x.MeineGewonnenenSätze(partien(1), New SpielerInfo("3"))).Returns(3)
        s.Setup(Function(x) x.MeineGewonnenenSätze(partien(1), New SpielerInfo("4"))).Returns(2)
        t.SchreibeNeuePartien(partien, 1)
        e.Verify(Sub(m) m.LeereBlatt("01"), Times.Once)
        e.Verify(Sub(m) m.NeueZeile("01", {"Linker Spieler", "Rechter Spieler", "Sätze Links", "Sätze Rechts"}), Times.Once)
        e.Verify(Sub(m) m.NeueZeile("01", {"1", "2", "0", "3"}), Times.Once)
        e.Verify(Sub(m) m.NeueZeile("01", {"3", "4", "3", "2"}), Times.Once)
    End Sub

    <Test>
    Public Sub SchreibeRangliste_schreibt_Header_aktuelle_Spielerdaten()
        Dim e = New Mock(Of IExcelDokument)
        Dim s = New Mock(Of ISpielstand)
        Dim spielerA = New SpielerInfo("A") With {
                .Vorname = "Marius",
                .Nachname = "Goppelt",
                .Geschlecht = 1,
                .Geburtsjahr = 1472,
                .Fremd = True,
                .Lizenznummer = "12345",
                .TTRating = 999,
                .TTRMatchCount = 22,
                .Verbandsspitzname = "Hello",
                .Vereinsname = "Hello World",
                .Vereinsnummer = 54321
            }
        Dim v = Mock.Of(Of ISpielverlauf(Of SpielerInfo))(
                Function(m) m.BerechneBuchholzPunkte(spielerA) = 5 AndAlso
                m.BerechneGegnerProfil(spielerA) Is {"B", "C"} AndAlso
                m.BerechneGewonneneSätze(spielerA) = 3 AndAlso
                m.BerechnePunkte(spielerA) = 2 AndAlso
                m.BerechneSonnebornBergerPunkte(spielerA) = 7 AndAlso
                m.BerechneVerloreneSätze(spielerA) = 1 AndAlso
                m.IstAusgeschieden(spielerA) = True)

        Dim t = New TurnierReport(e.Object, s.Object, v)
        t.SchreibeRangliste({spielerA}, 1)
        e.Verify(Sub(m) m.LeereBlatt("sp_rd01"))
        e.Verify(Sub(m) m.NeueZeile("sp_rd01", {"Rang", "Vorname", "Nachname",
                                    "ID", "Geschlecht", "Geburtsjahr",
                                    "Verein", "TTRating", "Punkte", "Buchholzpunkte",
                                    "SonnebornBergerpunkte", "Gewonnene Sätze",
                                    "Verlorene Sätze", "Ausgeschieden", "Gegnerprofil"}), Times.Once)
        e.Verify(Sub(m) m.NeueZeile("sp_rd01", {"1", "Marius", "Goppelt",
                                    "A", "m", "1472",
                                    "Hello World", "999", "2", "5",
                                    "7", "3",
                                    "1", "True", "B", "C"}), Times.Once)
        e.Verify(Sub(m) m.Speichern(), Times.Once)
    End Sub

End Class
