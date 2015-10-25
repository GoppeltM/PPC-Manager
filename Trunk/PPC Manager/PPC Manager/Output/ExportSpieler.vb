Imports PPC_Manager
''' 
''' Diese Erweiterungen sind notwendig, weil zum Zeitpunkt der Paarung bereits die aktuellen Buchholzpunkte verändert werden.
''' Ergo: ich brauche für den Export nur die Werte der Vergangenheit - exklusive der aktuellen Runde
Public Class ExportSpieler
    Inherits Spieler

    Sub New(spieler As Spieler)
        MyBase.New(spieler)
    End Sub

    Protected Overrides ReadOnly Property GespieltePartien As IEnumerable(Of SpielPartie)
        Get
            Dim r = From x In SpielRunden.Skip(1).Reverse From y In x Where y.SpielerLinks = Me Or y.SpielerRechts = Me Select y
            Return r.ToList
        End Get
    End Property

    Public ReadOnly Property GegnerProfil As IEnumerable(Of String)
        Get
            Return From x In GespieltePartien Select x.MeinGegner(Me).Id
        End Get
    End Property



End Class
