Imports StartlistenEditor

<Explicit, Apartment(Threading.ApartmentState.STA)>
Public Class NeuesTurnierDialogTests
    <Test>
    Public Sub NeuesTurnierDummy()
        Dim w = New NeuesTurnierDialog
        w.ShowDialog()
    End Sub
End Class
