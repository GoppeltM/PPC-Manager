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
        Dim c = Mock.Of(Of IComparer(Of SpielerInfo))
        _SpielerListe = New List(Of Spieler) From {
            New Spieler(New SpielerInfo("1"), s, c) With {.Nachname = "Bla"},
            New Spieler(New SpielerInfo("2"), s, c) With {.Nachname = "Blubb"},
            New Spieler(New SpielerInfo("3"), s, c) With {.Nachname = "Mustermann", .Vorname = "Max"}
            }
    End Sub

    Private Class RandomVergleicher
        Implements IComparer

        Private R As New Random

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Return R.Next(-10, 10)
        End Function
    End Class
    <Test>
    Public Sub SchweizerMode_UITest()
        Dim command = New CommandBinding(ApplicationCommands.Delete, Sub(o, e)
                                                                         MessageBox.Show(e.Parameter.ToString)
                                                                     End Sub)
        Dim l = New LiveListe
        l.InputBindings.Add(New KeyBinding(NavigationCommands.Refresh, New KeyGesture(Key.N, ModifierKeys.Control)))

        l.SpielerComparer = New RandomVergleicher
        l.DataContext = _SpielerListe
        Dim w As New Window
        w.Content = l

        w.CommandBindings.Add(command)
        w.ShowDialog()
    End Sub

    <Test>
    Public Sub PlayoffMode_UITest()
        Dim command = New CommandBinding(ApplicationCommands.[New], Sub(o, e)
                                                                        Dim liste = CType(e.Parameter, IEnumerable(Of SpielerInfo))
                                                                        MessageBox.Show(liste.First.ToString + ":" + liste.Last.ToString)
                                                                    End Sub)
        Dim l = New LiveListe
        l.InputBindings.Add(New KeyBinding(NavigationCommands.Refresh, New KeyGesture(Key.N, ModifierKeys.Control)))
        l.SpielerComparer = New RandomVergleicher
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
