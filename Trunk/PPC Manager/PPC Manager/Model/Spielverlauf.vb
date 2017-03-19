Imports PPC_Manager

Public Class Spielverlauf
    Implements ISpielverlauf(Of SpielerInfo)

    Private ReadOnly _Spielregeln As SpielRegeln
    Private ReadOnly _Spielpartien As IEnumerable(Of SpielPartie)
    Private ReadOnly _AusgeschiedeneSpieler As IEnumerable(Of SpielerInfo)

    Public Sub New(spielpartien As IEnumerable(Of SpielPartie),
                   ausgeschiedeneSpieler As IEnumerable(Of SpielerInfo),
                   spielRegeln As SpielRegeln)
        _Spielpartien = spielpartien
        _Spielregeln = spielRegeln
        _AusgeschiedeneSpieler = ausgeschiedeneSpieler
    End Sub

    Public Function BerechnePunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechnePunkte
        Dim GewonneneSpiele = From x In _Spielpartien
                              Where x.SpielerLinks = t Or x.SpielerRechts = t
                              Let Meine = x.MeineGewonnenenSätze(t).Count
                              Where Meine >= _Spielregeln.Gewinnsätze Select x

        Return GewonneneSpiele.Count()
    End Function

    Private Function BerechnePunkteOhneFreilos(t As SpielerInfo) As Integer
        Dim GewonneneSpiele = From x In _Spielpartien
                              Where Not TypeOf x Is FreiLosSpiel
                              Where x.SpielerLinks = t Or x.SpielerRechts = t
                              Let Meine = x.MeineGewonnenenSätze(t).Count
                              Where Meine >= _Spielregeln.Gewinnsätze Select x

        Return GewonneneSpiele.Count()
    End Function


    Public Function BerechneBuchholzPunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneBuchholzPunkte
        Dim spieleOhneFreilos = From x In _Spielpartien
                                Where Not TypeOf x Is FreiLosSpiel
                                Select x
        Dim gegner = From x In spieleOhneFreilos
                     Where x.SpielerLinks = t
                     Select x.SpielerRechts

        gegner = gegner.Concat(From x In spieleOhneFreilos
                               Where x.SpielerRechts = t
                               Select x.SpielerLinks)

        Return Aggregate x In gegner
                   Let punkte = BerechnePunkteOhneFreilos(x)
                       Into Sum(punkte)

    End Function

    Public Function BerechneSonnebornBergerPunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneSonnebornBergerPunkte
        If Not _Spielregeln.SonneBornBerger Then Return 0
        Dim GewonneneSpiele = From x In _Spielpartien
                              Where Not TypeOf x Is FreiLosSpiel
                              Let Meine = x.MeineGewonnenenSätze(t).Count
                              Where Meine >= _Spielregeln.Gewinnsätze Select x
        Dim gegner = From x In GewonneneSpiele
                     Where x.SpielerLinks = t
                     Select x.SpielerRechts

        gegner = gegner.Concat(From x In GewonneneSpiele
                               Where x.SpielerRechts = t
                               Select x.SpielerLinks)

        Return Aggregate x In gegner
                   Let punkte = BerechnePunkteOhneFreilos(x)
                       Into Sum(punkte)
    End Function

    Public Function Habengegeneinandergespielt(a As SpielerInfo, b As SpielerInfo) As Boolean Implements ISpielverlauf(Of SpielerInfo).Habengegeneinandergespielt
        Return _Spielpartien.Any(Function(m) As Boolean
                                     If (m.SpielerLinks = a And m.SpielerRechts = b) Then Return True
                                     If (m.SpielerRechts = a And m.SpielerLinks = b) Then Return True
                                     Return False
                                 End Function)
    End Function

    Public Function HatFreilos(t As SpielerInfo) As Boolean Implements ISpielverlauf(Of SpielerInfo).HatFreilos
        Return Aggregate x In _Spielpartien.OfType(Of FreiLosSpiel)
                   Where x.SpielerLinks = t Or x.SpielerRechts = t
                       Into Any
    End Function

    Public Function IstAusgeschieden(t As SpielerInfo) As Boolean Implements ISpielverlauf(Of SpielerInfo).IstAusgeschieden
        Return Aggregate x In _AusgeschiedeneSpieler Where x = t Into Any()
    End Function

    Public Function BerechneGewonneneSätze(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneGewonneneSätze
        Return Aggregate x In _Spielpartien
                   Where x.SpielerLinks = t Or x.SpielerRechts = t
                   Where Not TypeOf x Is FreiLosSpiel
                       Into Sum(x.MeineGewonnenenSätze(t).Count)
    End Function

    Public Function BerechneVerloreneSätze(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneVerloreneSätze
        Return Aggregate x In _Spielpartien
                   Where x.SpielerLinks = t Or x.SpielerRechts = t
                   Where Not TypeOf x Is FreiLosSpiel
                       Into Sum(x.MeineVerlorenenSätze(t).Count)
    End Function

    Public Function BerechneSatzDifferenz(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneSatzDifferenz
        If Not _Spielregeln.SatzDifferenz Then Return 0
        Return BerechneGewonneneSätze(t) - BerechneVerloreneSätze(t)
    End Function
End Class
