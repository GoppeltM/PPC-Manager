Imports PPC_Manager

Public Class SpielerInfoComparer
    Implements IComparer(Of SpielerInfo), IComparer

    Private ReadOnly _BeachteSatzverhältnis As Boolean
    Private ReadOnly _BeachteSonnebornBergerPunkte As Boolean
    Private ReadOnly _Spielverlauf As ISpielverlauf(Of SpielerInfo)

    Public Sub New(spielverlauf As ISpielverlauf(Of SpielerInfo),
                   beachteSatzverhältnis As Boolean,
                   beachteSonnebornBergerPunkte As Boolean)
        _Spielverlauf = spielverlauf
        _BeachteSatzverhältnis = beachteSatzverhältnis
        _BeachteSonnebornBergerPunkte = beachteSonnebornBergerPunkte
    End Sub

    Public Function Compare1(x As Object, y As Object) As Integer Implements IComparer.Compare
        Return Compare(DirectCast(x, SpielerInfo), DirectCast(y, SpielerInfo))
    End Function

    Public Function Compare(x As SpielerInfo, y As SpielerInfo) As Integer Implements IComparer(Of SpielerInfo).Compare
        Dim diff As Integer = 0
        diff = _Spielverlauf.IstAusgeschieden(y).CompareTo(_Spielverlauf.IstAusgeschieden(x))
        If diff <> 0 Then Return diff
        diff = _Spielverlauf.BerechnePunkte(x) - _Spielverlauf.BerechnePunkte(y)
        If diff <> 0 Then Return diff
        diff = _Spielverlauf.BerechneBuchholzPunkte(x) - _Spielverlauf.BerechneBuchholzPunkte(y)
        If diff <> 0 Then Return diff
        If _BeachteSonnebornBergerPunkte Then
            diff = _Spielverlauf.BerechneSonnebornBergerPunkte(x) - _Spielverlauf.BerechneSonnebornBergerPunkte(y)
            If diff <> 0 Then Return diff
        End If
        If _BeachteSatzverhältnis Then
            diff = _Spielverlauf.BerechneSatzDifferenz(x) - _Spielverlauf.BerechneSatzDifferenz(y)
            If diff <> 0 Then Return diff
        End If

        diff = x.TTRating - y.TTRating
        If diff <> 0 Then Return diff
        diff = x.TTRMatchCount - y.TTRMatchCount
        If diff <> 0 Then Return diff
        diff = y.Nachname.CompareTo(x.Nachname)
        If diff <> 0 Then Return diff
        diff = y.Vorname.CompareTo(x.Vorname)
        If diff <> 0 Then Return diff
        diff = x.Lizenznummer.CompareTo(y.Lizenznummer)
        If diff <> 0 Then Return diff
        Return 0
    End Function
End Class
