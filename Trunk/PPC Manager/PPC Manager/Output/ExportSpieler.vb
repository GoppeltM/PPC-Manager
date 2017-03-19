Imports PPC_Manager
''' 
''' Diese Erweiterungen sind notwendig, weil zum Zeitpunkt der Paarung bereits die aktuellen Buchholzpunkte verändert werden.
''' Ergo: ich brauche für den Export nur die Werte der Vergangenheit - exklusive der aktuellen Runde
Public Class ExportSpieler
    Inherits Spieler

    Private ReadOnly _SpielPartien As IEnumerable(Of SpielPartie)

    Sub New(spieler As Spieler, spielpartien As IEnumerable(Of SpielPartie))
        MyBase.New(spieler)
        _SpielPartien = spielpartien
    End Sub

    Public ReadOnly Property GegnerProfil As IEnumerable(Of String)
        Get
            Dim gespieltePartien = From x In _SpielPartien Where x.SpielerLinks = Me Or x.SpielerRechts = Me
            Return From x In GespieltePartien Select x.MeinGegner(Me).Id
        End Get
    End Property



End Class
