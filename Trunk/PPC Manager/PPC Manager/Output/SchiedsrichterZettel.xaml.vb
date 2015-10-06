Imports PPC_Manager

Public Class SchiedsrichterZettel
    Implements IPaginatibleUserControl

    Public Shared ReadOnly KlassementProperty As DependencyProperty =
        DependencyProperty.Register("Klassement", GetType(String), GetType(SchiedsrichterZettel),
                                    New FrameworkPropertyMetadata("MyValue"))
    Public Property Klassement() As String
        Get
            Return CType(GetValue(KlassementProperty), String)
        End Get
        Set(ByVal value As String)
            SetValue(KlassementProperty, value)
            Dim alt = Klassement
            OnPropertyChanged(New DependencyPropertyChangedEventArgs(KlassementProperty, alt, value))
        End Set
    End Property

    Public Sub New(el As IEnumerable(Of SpielPartie), altersGruppe As String, rundenNummer As Integer, seitenNr As Integer)
        ' This call is required by the designer.
        InitializeComponent()
        Klassement = altersGruppe
        AktuellesDatum.Text = Date.Now.ToString("dd.MM.yyyy")
        Me.RundenNummer.Text = "Runde Nr. " & rundenNummer
        Seitennummer.Text = "Seite " & seitenNr + 1
        ItemsContainer.ItemsSource = el
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

    Public Property PageNumber As Integer Implements IPaginatibleUserControl.PageNumber
        Get

        End Get
        Set(value As Integer)
            Me.Seitennummer.Text = "Seite " & value + 1
        End Set
    End Property
End Class
