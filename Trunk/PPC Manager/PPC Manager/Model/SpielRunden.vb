Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports System.Collections.ObjectModel


Public Class SpielRunden
    Inherits Stack(Of SpielRunde)

    <DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>
    Public Property AusgeschiedeneSpieler As New ObservableCollection(Of Ausgeschieden)

    Public Function ToXML() As IEnumerable(Of XElement)

        Dim xSpielRunden As New List(Of XElement)

        Dim current = 0
        For Each SpielRunde In Me.Reverse
            Dim matchNr = 0
            For Each Match In SpielRunde.ToXML(current, Function() As Integer
                                                            matchNr += 1
                                                            Return matchNr
                                                        End Function)
                xSpielRunden.Add(Match)
            Next
            current += 1
        Next

        For Each s In AusgeschiedeneSpieler
            xSpielRunden.Add(<ppc:inactiveplayer player=<%= s.Spieler.Id %> group=<%= s.Runde %>/>)
        Next

        Return xSpielRunden
    End Function

End Class



