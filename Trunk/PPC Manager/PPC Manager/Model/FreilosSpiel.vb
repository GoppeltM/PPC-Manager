Public Class FreiLosSpiel
    Inherits SpielPartie

    Public Sub New(rundenName As String, ByVal freilosSpieler As SpielerInfo, gewinnsätze As Integer)
        MyBase.New(rundenName, freilosSpieler, freilosSpieler, gewinnsätze)
    End Sub

    Public Overrides Function ToString() As String
        Return "Freilos für " & Me.SpielerLinks.Nachname
    End Function

End Class

