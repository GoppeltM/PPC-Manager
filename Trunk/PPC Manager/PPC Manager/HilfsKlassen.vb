Imports System.Collections.ObjectModel

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

    Shared Function FromXML(ByVal xSpielerListe As IEnumerable(Of XElement), runden As SpielRunden) As SpielerListe
        Dim l = New SpielerListe

        For Each xSpieler In xSpielerListe
            l.Add(Spieler.FromXML(xSpieler, runden))
        Next
        Return l
    End Function

End Class

Public Class SpielRunden
    Inherits Stack(Of SpielRunde)

    Public Sub New()

    End Sub

    Friend Sub FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal runden As IEnumerable(Of XElement))
        Clear()
        Dim xRunden = From x In runden Group By x.@group Into Runde = Group Order By Runde.@group, Runde.@nr Ascending
        For Each xRunde In xRunden
            Push(SpielRunde.FromXML(spielerListe, xRunde.Runde))
        Next
    End Sub


    Friend Function ToXML() As XElement

        Dim xSpielRunden As New List(Of XElement)

        Dim current = 0
        For Each SpielRunde In Me.Reverse
            Dim CurrentSpiel = 0
            For Each Spiel In SpielRunde
                xSpielRunden.Add(Spiel.ToXML(current, CurrentSpiel))
                CurrentSpiel += 1
            Next
            current += 1
        Next

        Return <matches>
                   <%= xSpielRunden %>
               </matches>
    End Function

    Public Shared ReadOnly Empty As SpielRunden = New SpielRunden
        
End Class


Public Class SpielRunde
    Inherits ObservableCollection(Of SpielPartie)

    Friend Property AusgeschiedeneSpieler As New ObservableCollection(Of Spieler)

    Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xSpiele As IEnumerable(Of XElement)) As SpielRunde
        Dim runde As New SpielRunde
        For Each xSpielPartie In xSpiele
            runde.Add(SpielPartie.FromXML(spielerListe, xSpielPartie))
        Next

        ' TODO: Freilosspiele und ausgeschiedene Spieler implementieren
        'Dim xFreilos = xRunde.<FreiLosSpiel>.SingleOrDefault
        'If xFreilos IsNot Nothing Then
        '    runde.Add(FreiLosSpiel.FromXML(spielerListe, xFreilos))
        'End If

        'For Each xSpieler In xRunde.<AusgeschiedenerSpieler>
        '    Dim StartNummer = xSpieler.@ID
        '    runde.AusgeschiedeneSpieler.Add((From x In spielerListe Where x.Id = StartNummer Select x).First)
        'Next
        Return runde
    End Function

    Friend Function ToXML(ByVal spielRunde As Integer) As XElement


        Return <SpielRunde>
                   <%= From x In Me Let y = x.ToXML(spielRunde + 1, 1) Select y %>
                   <%= From x In AusgeschiedeneSpieler Select <AusgeschiedenerSpieler>
                                                                  <%= x.Id %>
                                                              </AusgeschiedenerSpieler> %>
               </SpielRunde>
    End Function

    Public Overrides Function ToString() As String
        Return "SpielRunde"
    End Function

End Class


Public Class SpielPartien
    Inherits ObservableCollection(Of SpielPartie)

    Friend Sub PaarungenBerechnen(ByVal spielerListe As SpielerListe)
        Me.Clear()
        For Each SpielPartie In Berechnen(spielerListe)
            Add(SpielPartie)
        Next
    End Sub

    Public Sub ErgebnisseEintragen()
        For Each begegnung In Me
            begegnung.SpielerLinks.GespieltePartien.Add(begegnung)
            begegnung.SpielerRechts.GespieltePartien.Add(begegnung)
        Next
    End Sub

    Private Function Berechnen(ByVal spielerListe As SpielerListe) As IList(Of SpielPartie)
        Dim list As New List(Of SpielPartie)
        For i = 0 To spielerListe.Count - 2 Step 2
            Dim partie As New SpielPartie(spielerListe(i), spielerListe(i + 1))
            list.Add(partie)
        Next
        Return list
    End Function

End Class

Public Class Vereine
    Inherits Collections.ObjectModel.Collection(Of String)

End Class


