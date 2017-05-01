﻿Imports System.Windows

<Explicit, Apartment(System.Threading.ApartmentState.STA)>
Public Class MainWindowTests
    Private _Window As MainWindow
    Private _Controller As Mock(Of IController)

    <SetUp>
    Public Sub Init()
        _Controller = New Moq.Mock(Of IController)
        Dim s = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim c = Mock.Of(Of IComparer(Of SpielerInfo))
        Dim l = New SpielerListe From {
            New Spieler(New SpielerInfo("1"), s, c) With {.Nachname = "Goppelt", .Vorname = "Marius"}
            }
        Dim r = New SpielRunden
        r.Push(New SpielRunde() From {New SpielPartie("Runde 2",
                                                      New SpielerInfo("1") With {.Vorname = "Marius"},
                                                      New SpielerInfo("2") With {.Vorname = "Stefan"}, 3),
                                    New SpielPartie("Runde 2",
                                                    New SpielerInfo("3") With {.Vorname = "Rick"},
                                                    New SpielerInfo("4") With {.Vorname = "Gerard"}, 3)})
        _Window = New MainWindow(_Controller.Object, l, r, "Hallo Welt", 3, Mock.Of(Of ISpielstand))
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
