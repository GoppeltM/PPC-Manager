Imports PPC_Manager

Public Class SpielerInfoComparer
    Implements IComparer(Of SpielerInfo)

    Private ReadOnly _Spielverlauf As ISpielverlauf(Of SpielerInfo)

    Public Sub New(spielverlauf As ISpielverlauf(Of SpielerInfo))
        _Spielverlauf = spielverlauf
    End Sub

    Public Function Compare(x As SpielerInfo, y As SpielerInfo) As Integer Implements IComparer(Of SpielerInfo).Compare
        Dim diff As Integer = 0
        diff = _Spielverlauf.IstAusgeschieden(y).CompareTo(_Spielverlauf.IstAusgeschieden(x))
        If diff <> 0 Then Return diff
        diff = _Spielverlauf.BerechnePunkte(x) - _Spielverlauf.BerechnePunkte(y)
        If diff <> 0 Then Return diff
        diff = _Spielverlauf.BerechneBuchholzPunkte(x) - _Spielverlauf.BerechneBuchholzPunkte(y)
        If diff <> 0 Then Return diff

        diff = _Spielverlauf.BerechneSonnebornBergerPunkte(x) - _Spielverlauf.BerechneSonnebornBergerPunkte(y)
        If diff <> 0 Then Return diff
        diff = _Spielverlauf.BerechneSatzDifferenz(x) - _Spielverlauf.BerechneSatzDifferenz(y)
        If diff <> 0 Then Return diff
        diff = x.TTRating - y.TTRating
        If diff <> 0 Then Return diff
        diff = x.TTRMatchCount - y.TTRMatchCount
        If diff <> 0 Then Return diff
        diff = y.Nachname.CompareTo(x.Nachname)
        If diff <> 0 Then Return diff
        diff = y.Vorname.CompareTo(x.Vorname)
        If diff <> 0 Then Return diff
        Dim longDiff = x.Lizenznummer - y.Lizenznummer
        If longDiff > 0 Then Return 1
        If longDiff < 0 Then Return -1
        Return 0
    End Function
End Class
