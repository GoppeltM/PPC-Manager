Imports System.Collections.ObjectModel

Public Class Spieler

    Property Vorname As String
    Property Nachname As String
    Property Fremd As Boolean
    Property XmlKnoten As IEnumerable(Of XElement)
    Property Anwesend As Boolean
    Property TTR As Integer
    Property Klassements As IEnumerable(Of String)

    Shared Function FromXML(SpielerKnoten As IEnumerable(Of XElement), Competitions As IEnumerable(Of String)) As Spieler
        Return New Spieler With {
        .XmlKnoten = SpielerKnoten,
        .Klassements = Competitions
        }
    End Function

End Class

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

    Public Sub New()
        Add(New Spieler With {.Vorname = "Marius", .Nachname = "Goppelt", .Klassements = {"Herren A", "Herren B"}})
        Add(New Spieler With {.Vorname = "Flo", .Nachname = "Ewald", .Fremd = True})
    End Sub

End Class