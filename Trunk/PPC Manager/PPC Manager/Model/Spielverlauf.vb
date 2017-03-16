Imports PPC_Manager

Public Class Spielverlauf
    Implements ISpielverlauf(Of Spieler)

    Private ReadOnly _Gewinnsätze As Integer
    Private ReadOnly _Spielpartien As IEnumerable(Of SpielPartie)

    Public Sub New(spielpartien As IEnumerable(Of SpielPartie), gewinnsätze As Integer)
        _Spielpartien = spielpartien
        _Gewinnsätze = gewinnsätze
    End Sub

    Public Function BerechnePunkte(t As Spieler) As Integer Implements ISpielverlauf(Of Spieler).BerechnePunkte
        Dim GewonneneSpiele = From x In _Spielpartien
                              Where x.SpielerLinks = t Or x.SpielerRechts = t
                              Let Meine = x.MeineGewonnenenSätze(t).Count
                              Where Meine >= _Gewinnsätze Select x

        Return GewonneneSpiele.Count()
    End Function

    Public Function Habengegeneinandergespielt(a As Spieler, b As Spieler) As Boolean
        Return _Spielpartien.Any(Function(m) As Boolean
                                     If (m.SpielerLinks = a And m.SpielerRechts = b) Then Return True
                                     If (m.SpielerRechts = a And m.SpielerLinks = b) Then Return True
                                     Return False
                                 End Function)
    End Function

    Public Function HatFreilos(t As Spieler) As Boolean Implements ISpielverlauf(Of Spieler).HatFreilos
        Return Aggregate x In _Spielpartien.OfType(Of FreiLosSpiel)
                   Where x.SpielerLinks = t Or x.SpielerRechts = t
                       Into Any
    End Function
End Class
