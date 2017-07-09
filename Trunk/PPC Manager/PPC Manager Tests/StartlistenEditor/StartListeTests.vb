Imports System.Text
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports Moq
Imports System.Collections.ObjectModel

<Apartment(Threading.ApartmentState.STA)>
Public Class StartListeTests

    <Explicit, Test>
    Sub UIDummy_Starten()
        Dim SpielerListe = New ObservableCollection(Of StartlistenEditor.SpielerInfo)
        Dim KlassementListe = {"A", "B"}

        Dim mainWindow As New StartlistenEditor.MainWindow(SpielerListe, KlassementListe, Sub()

                                                                                          End Sub)
        mainWindow.ShowDialog()
    End Sub

End Class