Imports System.Windows
Imports System.Windows.Input

<Apartment(System.Threading.ApartmentState.STA), Explicit>
Public Class LiveListeTests
    Private _SpielerListe As List(Of Spieler)

    <SetUp>
    Public Sub Init()
        Dim s = Mock.Of(Of ISpielverlauf(Of SpielerInfo))(
            Function(x) x.IstAusgeschieden(It.Is(Of SpielerInfo)(
            Function(y) y.Id = "3")) = True)
        _SpielerListe = New List(Of Spieler) From {
            New Spieler(s) With {.Id = "1", .Nachname = "Bla"},
            New Spieler(s) With {.Id = "2", .Nachname = "Blubb"},
            New Spieler(s) With {.Id = "3", .Nachname = "Mustermann"}
            }
    End Sub

    <Test>
    Public Sub SchweizerMode_UITest()
        Dim command = New CommandBinding(ApplicationCommands.Delete, Sub(o, e)
                                                                         MessageBox.Show(e.Parameter.ToString)
                                                                     End Sub)
        Dim l = New LiveListe

        l.DataContext = _SpielerListe
        Dim w As New Window
        w.Content = l

        w.CommandBindings.Add(Command)
        w.ShowDialog()
    End Sub

    <Test>
    Public Sub PlayoffMode_UITest()
        Dim command = New CommandBinding(ApplicationCommands.[New], Sub(o, e)
                                                                        Dim liste = CType(e.Parameter, IEnumerable(Of SpielerInfo))
                                                                        MessageBox.Show(liste.First.ToString + ":" + liste.Last.ToString)
                                                                    End Sub)
        Dim l = New LiveListe
        l.DataContext = _SpielerListe
        Dim w As New Window
        w.Content = l
        w.CommandBindings.Add(command)
        w.CommandBindings.Add(New CommandBinding(MeineCommands.Playoff, Sub(o, e)

                                                                        End Sub,
                                                 Sub(o, e)
                                                     e.CanExecute = True
                                                 End Sub))
        w.ShowDialog()
    End Sub
End Class
