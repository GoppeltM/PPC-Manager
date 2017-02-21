Imports StartlistenEditor

<RequiresSTA>
Public Class FremdSpielerDialogTests


    <Explicit, Test>
    Public Sub UITest()
        Dim d = New FremdSpielerDialog(New XDocument, New Spieler)
        d.ShowDialog()
    End Sub
End Class
