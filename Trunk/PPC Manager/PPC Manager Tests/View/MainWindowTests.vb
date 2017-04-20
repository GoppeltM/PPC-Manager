<Explicit, Apartment(Threading.ApartmentState.STA)>
Public Class MainWindowTests

    <Test>
    Sub MainWindowUIDummy()
        Dim controller = New Moq.Mock(Of IController)
        Dim s = Mock.Of(Of ISpielverlauf(Of SpielerInfo))
        Dim l = New SpielerListe From {
            New Spieler(s) With {.Nachname = "Goppelt", .Vorname = "Marius"}
            }
        Dim r = New SpielRunden
        r.Push(New SpielRunde() From {New SpielPartie("Runde 2",
                                                      New SpielerInfo With {.Vorname = "Marius"},
                                                      New SpielerInfo With {.Vorname = "Stefan"}, 3)})
        Dim window = New MainWindow(controller.Object, l, r, "Hallo Welt")
        window.ShowDialog()
    End Sub
End Class
