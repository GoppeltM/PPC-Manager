Imports System.Text
Imports NUnit.Framework
Imports System.Windows.Media
Imports System.Windows
Imports Moq

<TestFixture()> Public Class KonverterTests

    <Test>
    Public Sub GeschlechtKonverter_0_w()
        Dim converter As New GeschlechtKonverter
        Dim result = converter.Convert(0, Nothing, Nothing, Nothing)
        Assert.AreEqual("w", result)
    End Sub

    <Test>
    Public Sub GeschlechtKonverter_1_m()
        Dim converter As New GeschlechtKonverter
        Dim result = converter.Convert(1, Nothing, Nothing, Nothing)
        Assert.AreEqual("m", result)
    End Sub

    <Test>
    Public Sub AusgeschiedenPainter_true_Bisque()
        Dim converter = New AusgeschiedenPainter
        Dim result = converter.Convert(True, GetType(Brush), Nothing, Nothing)
        Assert.AreEqual(Brushes.Bisque, result)
    End Sub

    <Test>
    Public Sub AusgeschiedenPainter_false_Transparent()
        Dim converter = New AusgeschiedenPainter
        Dim result = converter.Convert(False, GetType(Brush), Nothing, Nothing)
        Assert.AreEqual(Brushes.Transparent, result)
    End Sub

    <Test>
    Public Sub HintergrundLinksKonverter_LinksGrößer_Yellow()
        Dim converter = New HintergrundLinksKonverter
        Dim satz As New Satz With {.PunkteLinks = 14, .PunkteRechts = 12}
        Dim result = converter.Convert(satz, Nothing, Nothing, Nothing)
        Assert.AreEqual(Brushes.Yellow, result)
    End Sub

    <Test>
    Public Sub HintergrundLinksKonverter_Unter11_Transparent()
        Dim converter = New HintergrundLinksKonverter
        Dim satz As New Satz With {.PunkteLinks = 7, .PunkteRechts = 5}
        Dim result = converter.Convert(satz, Nothing, Nothing, Nothing)
        Assert.AreEqual(Brushes.Transparent, result)
    End Sub

    <Test>
    Public Sub HintergrundRechtsKonverter_RechtsKleiner_Transparent()
        Dim converter = New HintergrundRechtsKonverter
        Dim satz As New Satz With {.PunkteLinks = 15, .PunkteRechts = 13}
        Dim result = converter.Convert(satz, Nothing, Nothing, Nothing)
        Assert.AreEqual(Brushes.Transparent, result)
    End Sub

    <Test>
    Public Sub HintergrundRechtsKonverter_Rechts11_Yellow()
        Dim converter = New HintergrundRechtsKonverter
        Dim satz As New Satz With {.PunkteLinks = 1, .PunkteRechts = 11}
        Dim result = converter.Convert(satz, Nothing, Nothing, Nothing)
        Assert.AreEqual(Brushes.Yellow, result)
    End Sub

    <Test>
    Public Sub MeineGewonnenenSätze_LVerloren_Hidden()
        Dim converter = New MeineGewonnenenSätze
        Dim satz As New Satz With {.PunkteLinks = 1, .PunkteRechts = 11}
        Dim result = converter.Convert(satz, Nothing, "L", Nothing)
        Assert.AreEqual(Visibility.Hidden, result)
    End Sub

    <Test>
    Public Sub MeineGewonnenenSätze_LGewonnen_Visible()
        Dim converter = New MeineGewonnenenSätze
        Dim satz As New Satz With {.PunkteLinks = 13, .PunkteRechts = 11}
        Dim result = converter.Convert(satz, Nothing, "L", Nothing)
        Assert.AreEqual(Visibility.Visible, result)
    End Sub

    <Test>
    Public Sub OpacityConverter_ist_transparent_wenn_SpielPartie_istAbgeschlossen()
        Dim converter = New OpacityConverter
        converter.IstAbgeschlossen = Function(x) True
        Dim result = converter.Convert(New SpielPartie("Runde 1", New SpielerInfo("A"), New SpielerInfo("B")), Nothing, Nothing, Nothing)
        Assert.AreEqual(0.2, result)
    End Sub

    <Test>
    Public Sub OpacityConverter_ist_intransparent_wenn_SpielPartie_nicht_istAbgeschlossen()
        Dim converter = New OpacityConverter
        converter.IstAbgeschlossen = Function(x) False
        Dim result = converter.Convert(New SpielPartie("Runde 1", New SpielerInfo("A"), New SpielerInfo("B")), Nothing, Nothing, Nothing)
        Assert.AreEqual(1.0, result)
    End Sub

    <Test>
    Public Sub GewonneneSätzeConverter()
        Dim converter = New GewonneneSätzeConverter
        Dim spieler1 = New SpielerInfo("1")
        Dim spieler2 = New SpielerInfo("2")
        converter.MeineGewonnenenSätze = Function(x, y) As Integer
                                             If y = spieler1 Then
                                                 Return 2
                                             End If
                                             If y = spieler2 Then
                                                 Return 1
                                             End If
                                             Throw New InvalidOperationException
                                         End Function
        Dim partie As New SpielPartie("Bla", spieler1, spieler2)
        Dim result = converter.Convert(partie, Nothing, Nothing, Nothing)
        Assert.AreEqual("2:1", result)
    End Sub

End Class