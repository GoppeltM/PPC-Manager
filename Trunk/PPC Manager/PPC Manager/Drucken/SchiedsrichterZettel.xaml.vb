Public Class SchiedsrichterZettel
    Implements IPaginatibleUserControl

    Public Sub New(altersgruppe As String, rundenzahl As Integer)
        ' This call is required by the designer.
        InitializeComponent()

        Resources("KlassementName") = KlassementName
        Me.AktuellesDatum.Text = Date.Now.ToString("dd.MM.yyyy")
        Me.RundenNummer.Text = "Runde Nr. " & rundenzahl

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Function GetMaxItemCount() As Integer Implements IPaginatibleUserControl.GetMaxItemCount
        Dim width = PageContent.ActualWidth
        Dim height = PageContent.ActualHeight

        Dim ItemHeight = 151
        Dim ItemWidth = 359.055118110236

        Dim Rows = CInt(width) \ CInt(ItemWidth)
        Dim Columns = CInt(height) \ CInt(ItemHeight)

        Dim total = Rows * Columns
        If total = 0 Then
            Throw New Exception("Seitenformat zu klein! Kann kein Element darauf vollständig drucken!")
        End If
        Return total
    End Function

    Public Sub SetSource(startIndex As Integer, ByVal elements As IEnumerable(Of Object)) Implements IPaginatibleUserControl.SetSource
        ItemsContainer.ItemsSource = elements
    End Sub

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
