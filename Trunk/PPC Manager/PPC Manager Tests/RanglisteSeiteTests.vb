<TestFixture>
Public Class RanglisteSeiteTests

    <Test, STAThread>
    Public Sub Konstruktor_Rundennummer_steht_im_Textfeld()
        Dim r = New RanglisteSeite("Altersgruppe", 2, 1, 1, New List(Of Spieler))
        Assert.That(r.RundenNummer.Text, Iz.EqualTo("Runde Nr. 2"))
    End Sub

    <Test, STAThread>
    Public Sub Konstruktor_Seitennummer_steht_im_Textfeld()
        Dim r = New RanglisteSeite("Altersgruppe", 1, 42, 1, New List(Of Spieler))
        Assert.That(r.Seitennummer.Text, Iz.EqualTo("Seite 42"))
    End Sub

    <Test, STAThread>
    Public Sub Konstruktor_Zeilen_entsprechen_Spielerzahl()

        Dim spieler As New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "Spieler41"}
        Dim l = New List(Of Spieler) From {spieler}

        Dim r = New RanglisteSeite("Altersgruppe", 1, 42, 5, l)

        Assert.That(r.SpielerRangListe.Items.Count, Iz.EqualTo(1))
    End Sub
End Class
