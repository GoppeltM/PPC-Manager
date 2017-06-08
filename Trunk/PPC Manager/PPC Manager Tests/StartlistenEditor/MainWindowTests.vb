Imports System.Collections.ObjectModel

<Apartment(Threading.ApartmentState.STA), Explicit>
Public Class StartlisteMainWindowTests

    <Test>
    Public Sub Leerer_StartListenEditor()
        Dim d = New StartlistenEditor.MainWindow(New ObservableCollection(Of StartlistenEditor.SpielerInfo),
                                                 {"A", "B", "C"}, Sub()

                                                                  End Sub)
        d.ShowDialog()
    End Sub

End Class
