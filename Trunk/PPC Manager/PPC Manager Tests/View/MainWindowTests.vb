Imports System.Windows

<Explicit, Apartment(System.Threading.ApartmentState.STA)>
Public Class MainWindowTests
    Private _Window As MainWindow
    Private _Controller As Mock(Of IController)

    <SetUp>
    Public Sub Init()
        _Controller = New Moq.Mock(Of IController)
        Dim s = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim l = New SpielerListe From {
            New Spieler(s) With {.Nachname = "Goppelt", .Vorname = "Marius"}
            }
        Dim r = New SpielRunden
        r.Push(New SpielRunde() From {New SpielPartie("Runde 2",
                                                      New SpielerInfo With {.Vorname = "Marius"},
                                                      New SpielerInfo With {.Vorname = "Stefan"}, 3),
                                    New SpielPartie("Runde 2",
                                                    New SpielerInfo With {.Vorname = "Rick"},
                                                    New SpielerInfo With {.Vorname = "Gerard"}, 3)})
        _Window = New MainWindow(_Controller.Object, l, r, "Hallo Welt")
    End Sub

    <Test>
    Sub MainWindowUIDummy()
        _Window.ShowDialog()
    End Sub

    <Test>
    Sub Mit_dummy_Controller()
        _Controller.Setup(Sub(m) m.ExcelExportieren(It.IsAny(Of String))) _
            .Callback(Of String)(Sub(x) MessageBox.Show(x))
        _Controller.Setup(Sub(m) m.RundenbeginnDrucken(It.IsAny(Of IPrinter))) _
            .Callback(Of IPrinter)(Sub(m) MessageBox.Show(m.ToString))
        _Window.ShowDialog()
    End Sub
End Class
