
Public Class Competition

    ReadOnly Property Altersgruppe As String

    ReadOnly Property SpielRunden As SpielRunden
    ReadOnly Property SpielerListe As SpielerListe
    ReadOnly Property SpielRegeln As SpielRegeln

    Public Sub New(spielRegeln As SpielRegeln,
                   spielrunden As SpielRunden,
                   spielerListe As SpielerListe,
                   altersgruppe As String)
        Me.SpielRegeln = spielRegeln
        Me.SpielerListe = spielerListe
        Me.SpielRunden = spielrunden
        Me.Altersgruppe = altersgruppe
    End Sub

End Class
