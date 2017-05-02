Public Class SpielKlassenKonverterTests

    Private _Konverter As New SpielKlassenkonverter(2015)

    <TestCase(2005, "u13")>
    <TestCase(2002, "u15")>
    <TestCase(2001, "u15")>
    <TestCase(2000, "u18")>
    <TestCase(1998, "u18")>
    Public Sub GeburtsJahr_ist_Darstellung(geburtsJahr As Integer, display As String)
        Dim result = _Konverter.Convert(geburtsJahr, Nothing, Nothing, Nothing)
        Assert.That(result, Iz.EqualTo(display))
    End Sub
End Class
