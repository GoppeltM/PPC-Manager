''' 
''' Diese Erweiterungen sind notwendig, weil zum Zeitpunkt der Paarung bereits die aktuellen Buchholzpunkte verändert werden.
''' Ergo: ich brauche für den Export nur die Werte der Vergangenheit - exklusive der aktuellen Runde
Public Class ExportSpieler
    Inherits Spieler

    Sub New(spieler As Spieler)
        MyBase.New(spieler)
    End Sub

    Private ReadOnly Property VergangenePartien As IEnumerable(Of SpielPartie)
        Get
            Dim r = From x In SpielRunden.Skip(1).Reverse From y In x Where y.SpielerLinks = Me Or y.SpielerRechts = Me Select y
            Return r.ToList
        End Get
    End Property

    Private ReadOnly Property MeineGewonnenenSpieleExport As IEnumerable(Of SpielPartie)
        Get
            Dim GewonneneSpiele = From x In VergangenePartien Let Meine = x.MeineGewonnenenSätze(Me).Count
                                  Where Meine >= SpielRegeln.Gewinnsätze Select x

            Return GewonneneSpiele.ToList
        End Get
    End Property

    Public ReadOnly Property ExportPunkte As Integer
        Get
            Return MeineGewonnenenSpieleExport.Count
        End Get
    End Property

    Private ReadOnly Property ExportPunkteOhneFreilos As Integer
        Get
            Return Aggregate x In MeineGewonnenenSpieleExport Where Not TypeOf x Is FreiLosSpiel Into Count()
        End Get
    End Property

    Public ReadOnly Property ExportBHZ As Integer
        Get
            Dim _punkte = Aggregate x In VergangenePartien
                              Where Not TypeOf x Is FreiLosSpiel
                                  Let y = New ExportSpieler(x.MeinGegner(Me)).ExportPunkteOhneFreilos Into Sum(y)
            Return _punkte
        End Get
    End Property

    Public ReadOnly Property ExportSonneborn As Integer
        Get
            Dim _punkte = Aggregate x In MeineGewonnenenSpieleExport
                              Where Not TypeOf x Is FreiLosSpiel
                                  Let y = New ExportSpieler(x.MeinGegner(Me)).ExportPunkteOhneFreilos Into Sum(y)
            Return _punkte
        End Get
    End Property

    Public ReadOnly Property ExportSätzeGewonnen As Integer
        Get
            Return Aggregate x In VergangenePartien Where Not TypeOf x Is FreiLosSpiel Into Sum(x.MeineGewonnenenSätze(Me).Count)
        End Get
    End Property

    Public ReadOnly Property ExportSätzeVerloren As Integer
        Get
            Return Aggregate x In VergangenePartien Where Not TypeOf x Is FreiLosSpiel Into Sum(x.MeineVerlorenenSätze(Me).Count)
        End Get
    End Property

    Public ReadOnly Property GegnerProfil As IEnumerable(Of String)
        Get
            Return From x In GespieltePartien Select x.MeinGegner(Me).Id
        End Get
    End Property



End Class
