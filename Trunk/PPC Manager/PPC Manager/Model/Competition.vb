
Public Class Competition

    ReadOnly Property Altersgruppe As String

    ReadOnly Property SpielRunden As SpielRunden
    ReadOnly Property SpielerListe As IEnumerable(Of SpielerInfo)
    ReadOnly Property SpielRegeln As SpielRegeln

    Public Sub New(spielerListe As IEnumerable(Of SpielerInfo), altersgruppe As String)
        Me.SpielRegeln = New SpielRegeln(3, True, True)
        Me.SpielerListe = spielerListe
        Me.SpielRunden = New SpielRunden()
        Me.Altersgruppe = altersgruppe

    End Sub

    Public Sub New(spielRegeln As SpielRegeln,
                   spielrunden As SpielRunden,
                   spielerListe As IEnumerable(Of SpielerInfo),
                   altersgruppe As String)
        Me.SpielRegeln = spielRegeln
        Me.SpielerListe = spielerListe
        Me.SpielRunden = spielrunden
        Me.Altersgruppe = altersgruppe
    End Sub

End Class
