Public Class Druckvorschau
    Public Sub New(seiten As IEnumerable(Of FixedPage))

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Dim l = CType(FindResource("SeitenListe"), SeitenListe)
        For Each seite In seiten
            l.Add(seite)
            l.Add(New Rectangle With {.Width = seite.Width, .Height = 50, .Fill = Brushes.DarkGray})
        Next
    End Sub

End Class

Public Class SeitenListe
    Inherits ObjectModel.Collection(Of Visual)

End Class