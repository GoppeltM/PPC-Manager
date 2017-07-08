Imports System.Windows
Imports System.Windows.Controls

<Explicit, Apartment(System.Threading.ApartmentState.STA)>
Public Class MainWindowTests
    Private _Window As MainWindow
    Private _Controller As Mock(Of IController)

    <SetUp>
    Public Sub Init()
        _Controller = New Moq.Mock(Of IController)
        Dim s = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim c = New SpielerInfoComparer(s, False, False)
        Dim l = New SpielerListe From {
            New Spieler(New SpielerInfo("1"), s, c) With {.Nachname = "Goppelt", .Vorname = "Marius", .TTRating = 45},
            New Spieler(New SpielerInfo("2"), s, c) With {.Nachname = "Ewald", .Vorname = "Florian", .TTRating = 24},
            New Spieler(New SpielerInfo("2"), s, c) With {.Nachname = "Mustermann", .Vorname = "Max", .TTRating = 33}
            }
        Dim r = New SpielRunden
        r.Push(New SpielRunde() From {New SpielPartie("Runde 2",
                                                      New SpielerInfo("1") With {.Vorname = "Marius"},
                                                      New SpielerInfo("2") With {.Vorname = "Stefan"}),
                                    New SpielPartie("Runde 2",
                                                    New SpielerInfo("3") With {.Vorname = "Rick"},
                                                    New SpielerInfo("4") With {.Vorname = "Gerard"})})
        Dim d = Mock.Of(Of IDruckerFabrik)(Function(m) m.Neu(It.IsAny(Of PrintDialog)) Is Mock.Of(Of IPrinter))
        _Window = New MainWindow(
            _Controller.Object, l, r,
            "Hallo Welt", Mock.Of(Of ISpielstand), c, d)
    End Sub

    <Test>
    Sub MainWindowUIDummy()
        _Window.ShowDialog()
    End Sub

    <Test>
    Sub Mit_dummy_Controller()
        _Controller.Setup(Sub(m) m.ExcelExportieren(It.IsAny(Of String))) _
            .Callback(Of String)(Sub(x) MessageBox.Show(x))
        _Controller.Setup(Function(m) m.DruckeNeuePaarungen(It.IsAny(Of SeitenEinstellung))) _
            .Callback(Of IPrinter)(Sub(m) MessageBox.Show(m.ToString))
        _Controller.Setup(Function(m) m.DruckeRangliste(It.IsAny(Of SeitenEinstellung))) _
            .Callback(Of IPrinter)(Sub(m) MessageBox.Show(m.ToString))
        _Controller.Setup(Function(m) m.DruckeSchiedsrichterzettel(It.IsAny(Of SeitenEinstellung))) _
            .Callback(Of IPrinter)(Sub(m) MessageBox.Show(m.ToString))
        _Controller.Setup(Function(m) m.DruckeSpielergebnisse(It.IsAny(Of SeitenEinstellung))) _
            .Callback(Of IPrinter)(Sub(m) MessageBox.Show(m.ToString))
        _Window.ShowDialog()
    End Sub
End Class
