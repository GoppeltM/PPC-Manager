Public Class SpielerInfoTests
    <Test>
    Public Sub KopierterSpieler_ist_gleich_zu_Original()
        Dim s As New SpielerInfo("949")

        Dim neu As New SpielerInfo(s)
        Assert.That(neu, [Is].EqualTo(s))
    End Sub

    <Test>
    Public Sub Spieler_mit_selber_Id_ist_gleich()
        Dim s As New SpielerInfo("949")

        Dim neu As New SpielerInfo("949")
        Assert.That(neu, [Is].EqualTo(s))
    End Sub

    <Test>
    Public Sub Spieler_mit_unterschiedlicher_Id_ist_nicht_gleich()
        Dim s As New SpielerInfo("949")
        Dim neu As New SpielerInfo("888")
        Assert.That(neu, [Is].Not.EqualTo(s))
    End Sub

    <Test>
    Public Sub KopierterSpieler_enthält_selbe_Attribute_wie_Original()
        Dim spielverlauf = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim s As New SpielerInfo("949") With {
            .Vorname = "Bob",
            .Nachname = "Builder",
            .Lizenznummer = 1234,
            .Geburtsjahr = 1984,
            .TTRating = 520,
            .TTRMatchCount = 23,
            .Fremd = True,
            .Geschlecht = 1
        }

        Dim neu As New SpielerInfo(s)
        Assert.That(neu.Id, [Is].EqualTo("949"))
        Assert.That(neu.Vorname, [Is].EqualTo("Bob"))
        Assert.That(neu.Nachname, [Is].EqualTo("Builder"))
        Assert.That(neu.Lizenznummer, [Is].EqualTo(1234))
        Assert.That(neu.Geburtsjahr, [Is].EqualTo(1984))
        Assert.That(neu.TTRating, [Is].EqualTo(520))
        Assert.That(neu.TTRMatchCount, [Is].EqualTo(23))
        Assert.That(neu.Fremd, [Is].True)
        Assert.That(neu.Geschlecht, [Is].EqualTo(1))
    End Sub
End Class
