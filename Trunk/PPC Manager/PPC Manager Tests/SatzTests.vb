Public Class SatzTests

    <Test>
    Public Sub Satz_ist_initial_nicht_abgeschlossen()
        Dim s As New Satz
        Assert.That(s.Abgeschlossen, [Is].False)
    End Sub

    <Test>
    Public Sub Nicht_abgeschlossene_Sätze_sind_nicht_gewonnen()
        Dim s As New Satz
        s.PunkteRechts = 10
        s.PunkteLinks = 11
        Assert.That(s.GewonnenLinks, [Is].False)
    End Sub

    <Test>
    Public Sub abgeschlossene_Sätze_sind_gewonnen_wenn_höher()
        Dim s As New Satz
        s.PunkteRechts = 10
        s.PunkteLinks = 12
        Assert.That(s.GewonnenLinks, [Is].True)
    End Sub

    <Test>
    Public Sub Abgeschlossen_ist_wahr_wenn_PunkteLinks_auf_11()
        Dim s As New Satz
        s.PunkteLinks = 11
        Assert.That(s.Abgeschlossen, [Is].True)
    End Sub

    <Test>
    Public Sub Nicht_abgeschlossen_wenn_Punkte_Abstand_nur_1()
        Dim s As New Satz
        s.PunkteLinks = 11
        s.PunkteRechts = 10
        Assert.That(s.Abgeschlossen, [Is].False)
    End Sub

    <Test>
    Public Sub Abgeschlossen_wenn_Abstand_größer_2_und_über_11_Punkte()
        Dim s As New Satz
        s.PunkteLinks = 11
        s.PunkteRechts = 13
        Assert.That(s.Abgeschlossen, [Is].True)
    End Sub

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
