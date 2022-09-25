Imports PPC_Manager

Public Class Spielverlauf
    Implements ISpielverlauf(Of SpielerInfo)

    Private ReadOnly _Spielpartien As IEnumerable(Of SpielPartie)
    Private ReadOnly _AusgeschiedeneIDs As IEnumerable(Of String)
    Private ReadOnly _Spielstand As ISpielstand

    Public Sub New(spielpartien As IEnumerable(Of SpielPartie),
                   ausgeschiedeneIDs As IEnumerable(Of String),
                   spielstand As ISpielstand)
        _Spielpartien = spielpartien
        _AusgeschiedeneIDs = ausgeschiedeneIDs
        _Spielstand = spielstand
    End Sub

    Public Function BerechnePunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechnePunkte
        Dim GewonneneSpiele = From x In _Spielpartien
                              Where _Spielstand.HatPartieGewonnen(x, t)


        Return GewonneneSpiele.Count
    End Function

    Private Function BerechnePunkteOhneFreilos(t As SpielerInfo) As Integer
        Dim GewonneneSpiele = From x In _Spielpartien
                              Where Not TypeOf x Is FreiLosSpiel
                              Where _Spielstand.HatPartieGewonnen(x, t)

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
        Dim GewonneneSpiele = From x In _Spielpartien
                              Where Not TypeOf x Is FreiLosSpiel
                              Where _Spielstand.HatPartieGewonnen(x, t)

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
        Return Aggregate x In _AusgeschiedeneIDs Where x = t.Id Into Any()
    End Function

    Public Function BerechneGewonneneSätze(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneGewonneneSätze
        Return Aggregate x In _Spielpartien
                   Where x.SpielerLinks = t Or x.SpielerRechts = t
                   Where Not TypeOf x Is FreiLosSpiel
                       Into Sum(_Spielstand.MeineGewonnenenSätze(x, t))
    End Function

    Public Function BerechneVerloreneSätze(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneVerloreneSätze
        Return Aggregate x In _Spielpartien
                   Where x.SpielerLinks = t Or x.SpielerRechts = t
                   Where Not TypeOf x Is FreiLosSpiel
                       Into Sum(_Spielstand.MeineVerlorenenSätze(x, t))
    End Function

    Public Function BerechneSatzDifferenz(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneSatzDifferenz
        Return BerechneGewonneneSätze(t) - BerechneVerloreneSätze(t)
    End Function

    Public Function BerechneGegnerProfil(s As SpielerInfo) As IEnumerable(Of String) Implements ISpielverlauf(Of SpielerInfo).BerechneGegnerProfil
        Dim gespieltePartien = From x In _Spielpartien
                               Where x.SpielerLinks = s Or x.SpielerRechts = s
        Return From x In gespieltePartien Select x.GegnerVon(s).Id
    End Function
End Class
