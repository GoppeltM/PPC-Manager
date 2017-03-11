Public Class FreiLosSpiel
    Inherits SpielPartie

    Public Sub New(rundenName As String, ByVal freilosSpieler As Spieler, gewinnsätze As Integer)
        MyBase.New(rundenName, freilosSpieler, freilosSpieler, gewinnsätze)
    End Sub

    Public Overrides ReadOnly Property MeineGewonnenenSätze(ByVal ich As Spieler) As IList(Of Satz)
        Get
            Dim l As New List(Of Satz)
            For i = 0 To _GewinnSätze - 1
                l.Add(New Satz() With {.PunkteLinks = My.Settings.GewinnPunkte})
            Next
            Return l
        End Get
    End Property

    Public Overrides ReadOnly Property Abgeschlossen As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return "Freilos für " & Me.SpielerLinks.Nachname
    End Function

End Class

