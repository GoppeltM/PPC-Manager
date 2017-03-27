Imports StartlistenEditor

Public Class Dateisystem
    Implements IDateisystem

    Private pfad As String

    Public Sub New(pfad As String)
        Me.pfad = pfad
    End Sub

    Public Sub SpeichereXml(doc As XDocument) Implements IDateisystem.SpeichereXml
        doc.Save(pfad)
    End Sub

    Public Function LadeXml() As XDocument Implements IDateisystem.LadeXml
        Return XDocument.Load(pfad)
    End Function
End Class
