Imports System.Collections.ObjectModel
Imports StartlistenEditor

<Apartment(System.Threading.ApartmentState.STA), Explicit>
Public Class UITests
    <Test>
    Sub View_Controller_mit_Dummy_Daten()
        Dim l As New ObservableCollection(Of SpielerInfo)
        Dim c = New StartlistenController(l)
        l.Add(New SpielerInfo() With {
                .ID = "123"
              })
        Dim view = New MainWindow(c, {"B-Klasse"}, Sub()

                                                   End Sub)
        view.ShowDialog()
    End Sub
End Class
