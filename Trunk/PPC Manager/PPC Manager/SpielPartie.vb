

''' <summary>
''' 
''' </summary>
''' <remarks>
''' Ein Spiel besteht aus den darin teilnehmenden Spielern, und den Ergebnissen die sie verbuchen konnten.
''' Jedes Ergebnis besteht aus 3 oder 5 Sätzen, und den Punkten die darin angehäuft wurden.
''' </remarks>
Public Class SpielPartie

    Public Sub New()
        Sätze.Add(New Satz)
    End Sub

    Public Property Spieler As New List(Of Spieler)

    Public Property Sätze As New List(Of Satz)


    Private Satz As KeyValuePair(Of Integer, Integer)

    Public ReadOnly Property MeinGegner(ByVal ich As Spieler) As Spieler
        Get            

        End Get
    End Property

    Public ReadOnly Property SpielerLinks As Spieler
        Get
            Return New Spieler
        End Get        
    End Property

    Public ReadOnly Property SpielerRechts As Spieler
        Get
            Return New Spieler
        End Get
    End Property

End Class

Public Class Satz

    Public Property PunkteLinks As Integer = 3

    Public Property PunkteRechts As Integer = 7

End Class
