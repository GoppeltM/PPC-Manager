<TestFixture>
Public Class RanglisteSeiteTests

    <SetUp>
    Public Sub Init()
        Dim s As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        _RangListe = New RanglisteSeite("Altersgruppe", 2, 5, 42, New List(Of Spieler) From {s})
    End Sub

    Dim _RangListe As RanglisteSeite

    <Test, STAThread>
    Public Sub Klassementname_ist_wie_initialisiert()
        Assert.That(_RangListe.KlassementName.Text, [Is].EqualTo("Altersgruppe"))
    End Sub

    <Test, STAThread>
    Public Sub Rundennummer_steht_im_Textfeld()
        Assert.That(_RangListe.RundenNummer.Text, [Is].EqualTo("Runde Nr. 2"))
    End Sub
    <Test, STAThread>
    Public Sub Seitennummer_ist_eins_größer_als_initialisiert()
        Assert.That(_RangListe.Seitennummer.Text, [Is].EqualTo("Seite 6"))
    End Sub

    <Test, STAThread>
    Public Sub Zeilen_entsprechen_Spielerzahl()
        Assert.That(_RangListe.SpielerRangListe.Items.Count, Iz.EqualTo(1))
    End Sub
End Class
