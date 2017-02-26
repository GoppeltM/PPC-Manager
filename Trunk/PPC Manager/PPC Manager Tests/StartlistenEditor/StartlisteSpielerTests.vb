Imports StartlistenEditor

Public Class StartlisteSpielerTests

    <Test>
    Public Sub Bezahlt_feuert_PropertyEvent()
        Dim s As New SpielerInfo
        Dim erfolg = False
        AddHandler s.PropertyChanged, Sub(o, e)
                                          erfolg = True
                                          Assert.That(e.PropertyName, Iz.EqualTo("Bezahlt"))
                                      End Sub
        s.Bezahlt = True
        Assert.That(erfolg, [Is].True)
    End Sub

    <Test>
    Public Sub Anwesend_feuert_PropertyEvent()
        Dim s As New SpielerInfo
        Dim erfolg = False
        AddHandler s.PropertyChanged, Sub(o, e)
                                          erfolg = True
                                          Assert.That(e.PropertyName, Iz.EqualTo("Anwesend"))
                                      End Sub
        s.Anwesend = True
        Assert.That(erfolg, [Is].True)
    End Sub

    <Test>
    Public Sub Abwesend_feuert_PropertyEvent()
        Dim s As New SpielerInfo
        Dim erfolg = False
        AddHandler s.PropertyChanged, Sub(o, e)
                                          erfolg = True
                                          Assert.That(e.PropertyName, Iz.EqualTo("Abwesend"))
                                      End Sub
        s.Abwesend = True
        Assert.That(erfolg, [Is].True)
    End Sub
End Class
