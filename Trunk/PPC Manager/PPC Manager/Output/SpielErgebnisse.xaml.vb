Imports PPC_Manager

Public Class SpielErgebnisse


    Public Sub New(partien As IEnumerable(Of SpielPartie), altersGruppe As String, rundenNr As Integer, seitenNr As Integer)
        ' This call is required by the designer.
        InitializeComponent()

        KlassementName.Text = altersGruppe
        AktuellesDatum.Text = Date.Now.ToString("dd.MM.yyyy")
        RundenNummer.Text = String.Format("Runde Nr. {0}", rundenNr)
        Seitennummer.Text = String.Format("Seite {0}", seitenNr + 1)
        Dim res = CType(FindResource("Spieler"), SpielPartien)
        For Each s In partien
            res.Add(s)
        Next
    End Sub

    Public Function GetMaxItemCount() As Integer
        Dim width = SpielErgebnisseListe.ActualWidth
        Dim height = SpielErgebnisseListe.ActualHeight

        Dim ItemHeight = 20

        Dim Rows = CInt(height - 25) \ ItemHeight

        If Rows = 0 Then
            Throw New Exception("Seitenformat zu klein! Kann kein Element darauf vollständig drucken!")
        End If
        Return Rows
    End Function
End Class

Public Class NeuePaarungen
    Inherits SpielErgebnisse

    Public Sub New(partien As IEnumerable(Of SpielPartie), altersgruppe As String, rundenzahl As Integer, seitenNr As Integer)
        MyBase.New(partien, altersgruppe, rundenzahl, seitenNr)
        ErgebnisSpalte.Visibility = Visibility.Collapsed
    End Sub

End Class