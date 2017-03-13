Public Class Spielverlauf

    Private ReadOnly _Spielpartien As IEnumerable(Of SpielPartie)

    Public Sub New(spielpartien As IEnumerable(Of SpielPartie))
        _Spielpartien = spielpartien
    End Sub

    Public Function Habengegeneinandergespielt(a As Spieler, b As Spieler) As Boolean
        Return _Spielpartien.Any(Function(m) As Boolean
                                     If (m.SpielerLinks = a And m.SpielerRechts = b) Then Return True
                                     If (m.SpielerRechts = a And m.SpielerLinks = b) Then Return True
                                     Return False
                                 End Function)
    End Function
End Class
