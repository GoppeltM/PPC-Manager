Public Class SpielRundenTests

    <Test>
    Public Sub FromXML_mit_leeren_SpielRunden_ist_leer()
        Dim s As New SpielRunden()
        Dim ergebnis = s.ToXML()
        Assert.That(ergebnis, [Is].Empty)
    End Sub

End Class
