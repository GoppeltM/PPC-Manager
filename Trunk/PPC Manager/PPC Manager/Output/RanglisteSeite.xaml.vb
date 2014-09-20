Public Class RanglisteSeite
    Implements IPaginatibleUserControl


    Public Sub New(altersgruppe As String, rundenzahl As Integer)
        ' This call is required by the designer.
        InitializeComponent()

        Me.KlassementName.Text = altersgruppe
        Me.AktuellesDatum.Text = Date.Now.ToString("dd.MM.yyyy")
        Me.RundenNummer.Text = "Runde Nr. " & rundenzahl
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Function GetMaxItemCount() As Integer Implements IPaginatibleUserControl.GetMaxItemCount
        Dim width = SpielerRangListe.ActualWidth
        Dim height = SpielerRangListe.ActualHeight

        Dim ItemHeight = 20

        Dim Rows = CInt(height - 25) \ CInt(ItemHeight)

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
            Me.Seitennummer.Text = "Seite " & value + 1
        End Set
    End Property
End Class


Public Class IndexConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If Not TypeOf value Is Spieler Then Return ""
        Dim spieler = CType(value, Spieler)
        Dim liste = CType(parameter, IEnumerable(Of Spieler))

        Return liste.ToList.IndexOf(spieler) + 1 + RanglisteSeite.StartIndex
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class
