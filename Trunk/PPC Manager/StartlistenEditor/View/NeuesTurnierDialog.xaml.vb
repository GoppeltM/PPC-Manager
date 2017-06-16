Imports Microsoft.Win32

Public Class NeuesTurnierDialog
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        KlassementTyp.ItemsSource = New String() {"Einzel", "Doppel"}
        DataContext = New NeuesTurnierKontext
    End Sub

    Private Sub OKButton_Click(sender As Object, e As RoutedEventArgs) Handles OKButton.Click
        DialogResult = True
        Close()
    End Sub

    Public ReadOnly Property NeuesTurnierKontext As NeuesTurnierKontext
        Get
            Return CType(DataContext, NeuesTurnierKontext)
        End Get
    End Property

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim d = New SaveFileDialog With {
            .FileName = NeuesTurnierKontext.Dateiname,
            .Filter = "Turnierdaten|*.xml",
            .CheckPathExists = True,
            .Title = "Turnier speichern als...",
            .AddExtension = True
        }

        If d.ShowDialog() Then
            NeuesTurnierKontext.Dateiname = d.FileName
        End If
    End Sub
End Class
