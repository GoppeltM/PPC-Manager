Imports StartlistenEditor

<RequiresSTA>
Public Class FremdSpielerDialogTests


    <Explicit, Test>
    Public Sub UITest()
        Dim d = New FremdSpielerDialog(New String() {"Klasse A", "Klasse B"}, New SpielerInfo)
        d.ShowDialog()
    End Sub
End Class
