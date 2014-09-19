﻿Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

    Shared Function FromXML(ByVal xSpielerListe As IEnumerable(Of XElement), competition As Competition) As SpielerListe
        Dim l = New SpielerListe

        For Each xSpieler In xSpielerListe.<player>
            l.Add(Spieler.FromXML(xSpieler, competition))
        Next

        For Each xSpieler In xSpielerListe.<ppc:player>
            Dim s = Spieler.FromXML(xSpieler, competition)
            s.Fremd = True
            l.Add(s)
        Next
        Return l
    End Function

End Class



Public Class SpielPartien
    Inherits ObservableCollection(Of SpielPartie)

End Class