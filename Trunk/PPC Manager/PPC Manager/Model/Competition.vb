
Public Class Competition

    Property Altersgruppe As String

    Property SpielRunden As New SpielRunden
    Property SpielerListe As New SpielerListe
    Property DateiPfad As String

    Private _SpielRegeln As SpielRegeln
    ReadOnly Property SpielRegeln As SpielRegeln
        Get
            Return _SpielRegeln
        End Get
    End Property

    Public Sub New(spielRegeln As SpielRegeln)
        _SpielRegeln = spielRegeln
    End Sub

    Public Sub SaveExcel()
        Dim spieler = SpielerListe.ToList
        TurnierReport.CreateFile(ExcelPfad, spieler, Me)
    End Sub

    Public ReadOnly Property ExcelPfad As String
        Get
            Dim DateiName = IO.Path.GetFileNameWithoutExtension(DateiPfad)
            DateiName &= "_" & Altersgruppe
            For Each c In IO.Path.GetInvalidFileNameChars
                DateiName = DateiName.Replace(c, "_"c)
            Next
            DateiName = DateiName.Replace(" "c, "_"c)
            DateiName &= ".xlsx"
            Dim Unterpfad = IO.Path.Combine(IO.Path.GetDirectoryName(DateiPfad), "Protokolle")
            DateiName = IO.Path.Combine(Unterpfad, DateiName)
            Return DateiName
        End Get
    End Property


End Class
