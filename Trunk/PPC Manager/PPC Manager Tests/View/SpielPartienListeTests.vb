Imports System.Windows

<Explicit, Apartment(System.Threading.ApartmentState.STA)>
Public Class SpielPartienListeTests

    <Test>
    Public Sub Opacity_wird_gesetzt()
        Dim s = New SpielPartienListe

        Dim partieA = New SpielPartie("Runde 1",
                            New SpielerInfo("A") With {.Vorname = "Horst"},
                            New SpielerInfo("B") With {.Vorname = "Gérome"})

        Dim partieB = New SpielPartie("Runde 1",
                            New SpielerInfo("A") With {.Vorname = "Marius"},
                            New SpielerInfo("B") With {.Vorname = "Flo"})

        s.IstAbgeschlossen = Function(x) As Boolean
                                 If x Is partieA Then Return True
                                 Return False
                             End Function

        s.DataContext = New SpielRunde From {partieA, partieB}

        Dim w = New Window
        w.Content = s
        w.ShowDialog()

    End Sub
End Class
