Public Class SpielErgebnisse
    Implements IPaginatibleUserControl


    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        Me.KlassementName.Text = MainWindow.AktiveCompetition.Altersgruppe
        Me.AktuellesDatum.Text = Date.Now.ToString("dd.MM.yyyy")
        Me.RundenNummer.Text = "Runde Nr. " & MainWindow.AktiveCompetition.SpielRunden.Count
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Function GetMaxItemCount() As Integer Implements IPaginatibleUserControl.GetMaxItemCount
        Dim width = SpielErgebnisseListe.ActualWidth
        Dim height = SpielErgebnisseListe.ActualHeight

        Dim ItemHeight = 20

        Dim Rows = CInt(height - 25) \ CInt(ItemHeight)

        If Rows = 0 Then
            Throw New Exception("Seitenformat zu klein! Kann kein Element darauf vollständig drucken!")
        End If
        Return Rows
    End Function

    Public Sub SetSource(startIndex As Integer, ByVal elements As IEnumerable(Of Object)) Implements IPaginatibleUserControl.SetSource
        RanglisteSeite.StartIndex = startIndex
        Dim res = CType(FindResource("Spieler"), SpielPartien)
        For Each s In elements.OfType(Of SpielPartie)()
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
            Me.Seitennummer.Text = "Seite " & value + 1
        End Set
    End Property
End Class

Public Class GewonneneSätzeConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim partie = TryCast(value, SpielPartie)
        If partie Is Nothing Then Return Nothing

        Dim gewonnenLinks = partie.MeineGewonnenenSätze(partie.SpielerLinks)
        Dim gewonnenRechts = partie.MeineGewonnenenSätze(partie.SpielerRechts)

        Return String.Format("{0}:{1}", gewonnenLinks.Count, gewonnenRechts.Count)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class

Public Class NeuePaarungen
    Inherits SpielErgebnisse

    Public Sub New()
        ErgebnisSpalte.Visibility = Windows.Visibility.Collapsed
    End Sub

End Class