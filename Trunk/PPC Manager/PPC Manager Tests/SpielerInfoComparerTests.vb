Public Class SpielerInfoComparerTests
    Private _SpielerInfoComparer As SpielerInfoComparer
    Private _Spielverlauf As Mock(Of ISpielverlauf(Of SpielerInfo))
    Private _A As SpielerInfo
    Private _B As SpielerInfo

    <SetUp>
    Public Sub Init()
        _Spielverlauf = New Mock(Of ISpielverlauf(Of SpielerInfo))
        _SpielerInfoComparer = New SpielerInfoComparer(_Spielverlauf.Object)
        _A = New SpielerInfo() With {.Nachname = "X", .Vorname = "Y", .Id = "1"}
        _B = New SpielerInfo() With {.Nachname = "X", .Vorname = "Y", .Id = "2"}
    End Sub

    <Test>
    Public Sub Größer_wenn_mehr_Punkte()

        _Spielverlauf.Setup(Function(x) x.BerechnePunkte(_A)).Returns(5)
        _Spielverlauf.Setup(Function(x) x.BerechnePunkte(_B)).Returns(3)

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub

    <Test>
    Public Sub Größer_wenn_mehr_Buchholzpunkte()

        _Spielverlauf.Setup(Function(x) x.BerechneBuchholzPunkte(_A)).Returns(5)
        _Spielverlauf.Setup(Function(x) x.BerechneBuchholzPunkte(_B)).Returns(3)

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub

    <Test>
    Public Sub Größer_wenn_mehr_Sonnebornbergerpunkte()

        _Spielverlauf.Setup(Function(x) x.BerechneSonnebornBergerPunkte(_A)).Returns(5)
        _Spielverlauf.Setup(Function(x) x.BerechneSonnebornBergerPunkte(_B)).Returns(3)

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub

    <Test>
    Public Sub Größer_wenn_nicht_ausgeschieden()

        _Spielverlauf.Setup(Function(x) x.IstAusgeschieden(_A)).Returns(False)
        _Spielverlauf.Setup(Function(x) x.IstAusgeschieden(_B)).Returns(True)

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub

    <Test>
    Public Sub Größer_wenn_höhere_Satzdifferenz()

        _Spielverlauf.Setup(Function(x) x.BerechneSatzDifferenz(_A)).Returns(0)
        _Spielverlauf.Setup(Function(x) x.BerechneSatzDifferenz(_B)).Returns(-2)

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub

    <TestCase("A", "B")>
    <TestCase("F", "FF")>
    Public Sub Größer_wenn_Nachname_kleiner(a As String, b As String)
        _A.Nachname = a
        _B.Nachname = b

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub

    <TestCase("A", "B")>
    <TestCase("F", "FF")>
    Public Sub Größer_wenn_Vorname_kleiner(a As String, b As String)
        _A.Vorname = a
        _B.Vorname = b

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub

    <Test>
    Public Sub Größer_wenn_Lizenznummer_größer()
        _A.Lizenznummer = 7
        _B.Lizenznummer = 6

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub

    <Test>
    Public Sub Größer_wenn_TTRating_größer()
        _A.TTRating = 7
        _B.TTRating = 6

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub

    <Test>
    Public Sub Größer_wenn_TTRMatchcount_größer()
        _A.TTRMatchCount = 7
        _B.TTRMatchCount = 6

        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].GreaterThan(0))
    End Sub


    <Test>
    Public Sub Gleich_wenn_alles_gleich()
        Assert.That(_SpielerInfoComparer.Compare(_A, _B), [Is].EqualTo(0))
    End Sub


End Class
