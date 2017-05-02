Imports PPC_Manager

Public Class SpielverlaufCache
    Implements ISpielverlauf(Of SpielerInfo)

    Private ReadOnly _Spielverlauf As ISpielverlauf(Of SpielerInfo)

    Public Sub New(spielverlauf As ISpielverlauf(Of SpielerInfo))
        _Spielverlauf = spielverlauf
    End Sub

    Private ReadOnly _BHZCache As New Dictionary(Of SpielerInfo, Integer)

    Public Function BerechneBuchholzPunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneBuchholzPunkte
        If _BHZCache.ContainsKey(t) Then Return _BHZCache(t)
        _BHZCache(t) = _Spielverlauf.BerechneBuchholzPunkte(t)
        Return _BHZCache(t)
    End Function

    Public Function BerechneGewonneneSätze(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneGewonneneSätze
        Return _Spielverlauf.BerechneGewonneneSätze(t)
    End Function

    Private ReadOnly _PunkteCache As New Dictionary(Of SpielerInfo, Integer)

    Public Function BerechnePunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechnePunkte
        If _PunkteCache.ContainsKey(t) Then Return _PunkteCache(t)
        _PunkteCache(t) = _Spielverlauf.BerechnePunkte(t)
        Return _PunkteCache(t)
    End Function

    Public Function BerechneSatzDifferenz(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneSatzDifferenz
        Return _Spielverlauf.BerechneSatzDifferenz(t)
    End Function

    Public Function BerechneSonnebornBergerPunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneSonnebornBergerPunkte
        Return _Spielverlauf.BerechneSonnebornBergerPunkte(t)
    End Function

    Public Function BerechneVerloreneSätze(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneVerloreneSätze
        Return _Spielverlauf.BerechneVerloreneSätze(t)
    End Function

    Public Function Habengegeneinandergespielt(a As SpielerInfo, b As SpielerInfo) As Boolean Implements ISpielverlauf(Of SpielerInfo).Habengegeneinandergespielt
        Return _Spielverlauf.Habengegeneinandergespielt(a, b)
    End Function

    Public Function HatFreilos(t As SpielerInfo) As Boolean Implements ISpielverlauf(Of SpielerInfo).HatFreilos
        Return _Spielverlauf.HatFreilos(t)
    End Function

    Public Function IstAusgeschieden(t As SpielerInfo) As Boolean Implements ISpielverlauf(Of SpielerInfo).IstAusgeschieden
        Return _Spielverlauf.IstAusgeschieden(t)
    End Function

    Public Function BerechneGegnerProfil(s As SpielerInfo) As IEnumerable(Of String) Implements ISpielverlauf(Of SpielerInfo).BerechneGegnerProfil
        Return _Spielverlauf.BerechneGegnerProfil(s)
    End Function
End Class
