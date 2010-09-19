

''' <summary>
''' 
''' </summary>
''' <remarks>
''' Ein Spiel besteht aus den darin teilnehmenden Spielern, und den Ergebnissen die sie verbuchen konnten.
''' Jedes Ergebnis besteht aus 3 oder 5 Sätzen, und den Punkten die darin angehäuft wurden.
''' </remarks>
Public Class SpielPartie


    Public Property Spieler As New List(Of Spieler)

    Public Property Sätze As New List(Of Dictionary(Of Spieler, Integer))


    Private Satz As KeyValuePair(Of Integer, Integer)

    Public ReadOnly Property Sätze(ByVal spieler As Spieler) As List(Of Integer)
        Get

        End Get
    End Property

    Public ReadOnly Property MeinGegner(ByVal ich As Spieler) As Spieler
        Get            

        End Get
    End Property

End Class

