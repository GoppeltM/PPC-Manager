<Explicit, Apartment(Threading.ApartmentState.STA)>
Public Class DruckEinstellungenDialogTests

    <Test>
    Public Sub DruckEinstellungen_Dummy()
        Dim einstellungen As New DruckEinstellungen
        einstellungen.DruckeNeuePaarungen = True
        einstellungen.DruckeSpielergebnisse = True
        Dim d = New DruckEinstellungenDialog()
        d.DataContext = einstellungen
        Dim result = d.ShowDialog()
    End Sub
End Class
