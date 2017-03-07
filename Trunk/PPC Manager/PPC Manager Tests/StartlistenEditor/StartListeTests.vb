Imports System.Text
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports Moq

<Apartment(Threading.ApartmentState.STA)>
Public Class StartListeTests

    <Explicit, Test>
    Sub UIDummy_Starten()
        Dim SpielerListe = New List(Of StartlistenEditor.SpielerInfo)
        Dim KlassementListe = New List(Of String)

        Dim mainWindow As New StartlistenEditor.MainWindow(SpielerListe, KlassementListe, Sub()

                                                                                          End Sub)
        mainWindow.ShowDialog()
    End Sub

End Class