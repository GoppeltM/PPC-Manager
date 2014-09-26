Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

    Shared Function FromXML(ByVal xSpielerListe As IEnumerable(Of XElement), spielRunden As SpielRunden, spielRegeln As SpielRegeln) As SpielerListe
        Dim l = New SpielerListe

        For Each xSpieler In xSpielerListe.<player>
            l.Add(Spieler.FromXML(xSpieler, spielRunden, spielRegeln))
        Next

        For Each xSpieler In xSpielerListe.<ppc:player>
            Dim s = Spieler.FromXML(xSpieler, spielRunden, spielRegeln)
            s.Fremd = True
            l.Add(s)
        Next
        Return l
    End Function

End Class



Public Class SpielPartien
    Inherits ObservableCollection(Of SpielPartie)

End Class