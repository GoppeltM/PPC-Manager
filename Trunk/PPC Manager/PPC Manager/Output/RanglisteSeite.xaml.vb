Public Class RanglisteSeite
    Implements IPaginatibleUserControl


    Public Sub New(altersgruppe As String, rundenNummer As Integer, seitenNummer As Integer, offset As Integer, elemente As IEnumerable(Of Spieler))
        ' This call is required by the designer.
        InitializeComponent()
        Dim converter = CType(FindResource("GridIndexConverter"), GridIndexConverter)
        converter.Offset = offset
        KlassementName.Text = altersgruppe
        AktuellesDatum.Text = Date.Now.ToString("dd.MM.yyyy")
        Me.RundenNummer.Text = String.Format("Runde Nr. {0}", rundenNummer)
        Me.Seitennummer.Text = String.Format("Seite {0}", seitenNummer)


        Dim res = CType(FindResource("Spieler"), SpielerListe)
        For Each s In elemente
            res.Add(s)
        Next

    End Sub

    Public Function GetMaxItemCount() As Integer Implements IPaginatibleUserControl.GetMaxItemCount
        Dim width = SpielerRangListe.ActualWidth
        Dim height = SpielerRangListe.ActualHeight

        Dim ItemHeight = 20

        Dim Rows = CInt(height - 25) \ ItemHeight

        If Rows = 0 Then
            Throw New Exception("Seitenformat zu klein! Kann kein Element darauf vollständig drucken!")
        End If
        Return Rows
    End Function

    Public Sub SetSource(startIndex As Integer, ByVal elements As IEnumerable(Of Object)) Implements IPaginatibleUserControl.SetSource
        RanglisteSeite.StartIndex = startIndex
        Dim res = CType(FindResource("Spieler"), SpielerListe)
        For Each s In elements.OfType(Of Spieler)()
            res.Add(s)
        Next

    End Sub

    Public Shared StartIndex As Integer

    Private _PageNumber As Integer
    Public Property PageNumber As Integer Implements IPaginatibleUserControl.PageNumber
        Get
            Return _PageNumber
        End Get
        Set(value As Integer)
            _PageNumber = value
            Seitennummer.Text = "Seite " & value + 1
        End Set
    End Property
End Class