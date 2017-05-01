Imports System.Windows
Imports System.Windows.Input

<Explicit, Apartment(System.Threading.ApartmentState.STA)>
Public Class BegegnungenTests

    <Test>
    Public Sub Begegnungen_Integrierte_Sicht()
        Dim b = New Begegnungen
        Dim prev = New List(Of SpielPartie) From {
            New SpielPartie("Runde 1",
                            New SpielerInfo("1") With {.Vorname = "Marius"},
                            New SpielerInfo("2") With {.Vorname = "Flo"}),
            New SpielPartie("Runde 1",
                            New SpielerInfo("3") With {.Vorname = "Gerard"},
                            New SpielerInfo("4") With {.Vorname = "Rick"})
            }
        Dim n = New List(Of SpielPartie) From {
            New SpielPartie("Runde 2",
                            New SpielerInfo("5") With {.Vorname = "Hartmut"},
                            New SpielerInfo("6") With {.Vorname = "Stefan"})
            }
        b.DataContext = prev
        Dim spielstand = New Spielstand(3)
        b.SpielPartienListe.IstAbgeschlossen = AddressOf spielstand.IstAbgeschlossen
        b.IstAbgeschlossen = AddressOf spielstand.IstAbgeschlossen
        b.DetailGrid.IstAbgeschlossen = AddressOf spielstand.IstAbgeschlossen
        Dim w = New Window
        w.Content = b
        w.InputBindings.Add(New KeyBinding(NavigationCommands.NextPage, New KeyGesture(Key.N, ModifierKeys.Control)))
        w.CommandBindings.Add(New CommandBinding(NavigationCommands.NextPage, Sub(o, e)
                                                                                  If b.DataContext Is prev Then
                                                                                      b.DataContext = n
                                                                                      Return
                                                                                  End If
                                                                                  b.DataContext = prev
                                                                              End Sub))
        w.ShowDialog()
    End Sub
End Class
