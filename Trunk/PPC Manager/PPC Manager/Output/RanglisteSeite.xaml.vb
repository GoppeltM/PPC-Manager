Imports System.Collections.ObjectModel

Public Class RanglisteSeite

    Public Sub New(altersgruppe As String, rundenNummer As Integer, seitenNummer As Integer, offset As Integer, elemente As IEnumerable(Of Spieler))
        ' This call is required by the designer.
        InitializeComponent()
        Dim converter = CType(FindResource("GridIndexConverter"), GridIndexConverter)
        converter.Offset = offset
        KlassementName.Text = altersgruppe
        AktuellesDatum.Text = Date.Now.ToString("dd.MM.yyyy")
        Me.RundenNummer.Text = String.Format("Runde Nr. {0}", rundenNummer)
        Me.Seitennummer.Text = String.Format("Seite {0}", seitenNummer + 1)

        Dim druckSpieler = From x In elemente Select New RangListeSpieler(x)

        Dim res = CType(FindResource("Spieler"), RangListeSpielerListe)
        For Each s In druckSpieler
            res.Add(s)
        Next

    End Sub

    Public Function GetMaxItemCount() As Integer
        Dim width = SpielerRangListe.ActualWidth
        Dim height = SpielerRangListe.ActualHeight

        Dim ItemHeight = 20

        Dim Rows = CInt(height - 25) \ ItemHeight

        If Rows = 0 Then
            Throw New Exception("Seitenformat zu klein! Kann kein Element darauf vollständig drucken!")
        End If
        Return Rows
    End Function

End Class

Public Class RangListeSpieler
    Inherits Spieler

    Public Sub New(spieler As Spieler)
        MyBase.New(spieler)
    End Sub

    Public ReadOnly Property MeineSpieleDruck As IEnumerable(Of String)
        Get
            Dim l As New List(Of String)
            For Each s In GespieltePartien

                Dim text = s.MeinGegner(Me).StartNummer.ToString
                If s.Gewonnen(Me) Then
                    text &= "G"
                Else
                    text &= "V"
                End If
                l.Add(text)
            Next
            Return l
        End Get
    End Property

End Class

Public Class RangListeSpielerListe
    Inherits ObservableCollection(Of RangListeSpieler)
End Class