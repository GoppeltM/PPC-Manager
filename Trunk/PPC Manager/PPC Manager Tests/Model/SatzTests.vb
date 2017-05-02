Public Class SatzTests

    <Test>
    Public Sub PropertyChangedEvent_wird_für_PunkteRechts_gefeuert_wenn_Punkte_auf_11()
        Dim s As New Satz
        Dim gefeuert = False
        AddHandler s.PropertyChanged, Sub(sender, e) If e.PropertyName() = "PunkteRechts" Then gefeuert = True
        s.PunkteRechts = 11
        Assert.That(gefeuert, [Is].True)
    End Sub

    <Test>
    Public Sub PropertyChangedEvent_wird_für_Self_gefeuert_wenn_Punkte_auf_11()
        Dim s As New Satz
        Dim gefeuert = False
        AddHandler s.PropertyChanged, Sub(sender, e) If e.PropertyName() = "MySelf" Then gefeuert = True
        s.PunkteRechts = 11
        Assert.That(gefeuert, [Is].True)
    End Sub

    <Test>
    Public Sub PropertyChangedEvent_wird_für_Abgeschlossen_nicht_gefeuert_wenn_Punkte_auf_7()
        Dim s As New Satz
        Dim gefeuert = False
        AddHandler s.PropertyChanged, Sub(sender, e) gefeuert = e.PropertyName() = "Abgeschlossen"
        s.PunkteRechts = 7
        Assert.That(gefeuert, [Is].False)
    End Sub


End Class
