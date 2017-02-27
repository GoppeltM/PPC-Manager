Public Class Speicher

    Private ReadOnly _Doc As XDocument
    Private ReadOnly _Pfad As String
    Public Sub New(pfad As String)
        _Pfad = pfad
        _Doc = XDocument.Load(pfad)
        Klassements = _Doc.Root.<competition>
        KlassementNamen = (From x In Klassements Select x.Attribute("age-group").Value).ToList
    End Sub

    Public ReadOnly Property Klassements As IEnumerable(Of XElement)

    Public ReadOnly Property KlassementNamen As IEnumerable(Of String)

    Public Sub Speichern()
        _Doc.Save(_Pfad)
    End Sub
End Class
