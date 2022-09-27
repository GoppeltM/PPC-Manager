<TestFixture, Apartment(Threading.ApartmentState.STA)>
Public Class RanglisteSeiteTests

    <SetUp>
    Public Sub Init()
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim s As New Spieler(New SpielerInfo("A"), spielverlauf, Mock.Of(Of IComparer(Of SpielerInfo)))
        _RangListe = New RanglisteSeite("Altersgruppe",
                                        {s},
                                        New List(Of SpielRunde)({Mock.Of(Of SpielRunde), Mock.Of(Of SpielRunde)}),
                                        Mock.Of(Of ISpielstand),
                                        Mock.Of(Of SeitenEinstellung))
    End Sub

    Dim _RangListe As RanglisteSeite

    <Test>
    Public Sub Klassementname_ist_wie_initialisiert()
        Assert.That(_RangListe.KlassementName.Text, [Is].EqualTo("Altersgruppe"))
    End Sub

    <Test>
    Public Sub Rundennummer_steht_im_Textfeld()
        Assert.That(_RangListe.RundenNummer.Text, [Is].EqualTo("Runde Nr. 1"))
    End Sub

    <Test>
    Public Sub Zeilen_entsprechen_Spielerzahl()
        Assert.That(_RangListe.SpielerRangListe.Items.Count, Iz.EqualTo(1))
    End Sub
End Class
