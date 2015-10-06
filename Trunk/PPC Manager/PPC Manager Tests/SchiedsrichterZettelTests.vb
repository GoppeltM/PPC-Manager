<TestFixture>
Public Class SchiedsrichterZettelTests

    <SetUp>
    Public Sub Init()
        Dim s As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        _Zettel = New SchiedsrichterZettel(New List(Of SpielPartie) _
                                           From {New FreiLosSpiel("Runde X", s, 3)},
                                           "MeinKlassementName", 5, 2)
    End Sub

    Private _Zettel As SchiedsrichterZettel

    <Test, STAThread>
    Public Sub KlassementName_ist_wie_Konstruktor()
        Assert.That(_Zettel.KlassementName.Text, Iz.EqualTo("MeinKlassementName"))
    End Sub

    <Test, STAThread>
    Public Sub Seitennummer_ist_eins_höher_als_gesetzte_Seitenzahl()
        Assert.That(_Zettel.Seitennummer.Text, Iz.EqualTo("Seite 3"))
    End Sub

    <Test, STAThread>
    Public Sub RundenNummer_ist_identisch_zu_Konstruktor()
        Assert.That(_Zettel.RundenNummer.Text, [Is].EqualTo("Runde Nr. 5"))
    End Sub

    <Test, STAThread>
    Public Sub ListView_enthält_ein_Element()
        Assert.That(_Zettel.ItemsContainer.Items.Count, [Is].EqualTo(1))
    End Sub

    <Test, STAThread>
    Public Sub Aktuelles_Datum_ist_wohlgeformt()
        Assert.That(_Zettel.AktuellesDatum.Text, Iz.StringMatching("\d\d\.\d\d\.\d\d"))
    End Sub
End Class
