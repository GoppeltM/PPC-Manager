<TestFixture>
Public Class SchiedsrichterZettelTests

    <Test, STAThread>
    Public Sub KlassementName_ist_wie_Konstruktor()
        Dim zettel = New SchiedsrichterZettel("MeinKlassementName", 3)
        Assert.That(zettel.KlassementName.Text, Iz.EqualTo("MeinKlassementName"))
    End Sub

    <Test, STAThread>
    Public Sub Seitennummer_ist_eins_höher_als_gesetzte_Seitenzahl()
        Dim zettel = New SchiedsrichterZettel("MeinKlassementName", 3)
        zettel.PageNumber = 2

        Assert.That(zettel.Seitennummer.Text, Iz.EqualTo("Seite 3"))
    End Sub

    <Test, STAThread>
    Public Sub Aktuelles_Datum_ist_wohlgeformt()
        Dim zettel = New SchiedsrichterZettel("MeinKlassementName", 3)

        Assert.That(zettel.AktuellesDatum.Text, Iz.StringMatching("\d\d\.\d\d\.\d\d"))
    End Sub
End Class
