Public Class DruckEinstellungenDialog

    Public ReadOnly Property Einstellungen As DruckEinstellungen
        Get
            Return CType(DataContext, DruckEinstellungen)
        End Get
    End Property


    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim p = New PrintDialog
        If p.ShowDialog Then
            Dim b = CType(sender, Button)
            Select Case b.Name
                Case "NeuePaarungenEinstellungen"
                    Einstellungen.EinstellungenNeuePaarungen = p
                Case "RanglisteEinstellungen"
                    Einstellungen.EinstellungenRangliste = p
                Case "SchiedsrichterzettelEinstelllungen"
                    Einstellungen.EinstellungenSchiedsrichterzettel = p
                Case "SpielergebnisseEinstellungen"
                    Einstellungen.EinstellungenSpielergebnisse = p
                Case Else
                    Throw New ApplicationException("Nicht gefunden:" & b.Name)
            End Select
        End If
    End Sub

    Private Sub Drucken_Click(sender As Object, e As RoutedEventArgs) Handles Drucken.Click
        With Einstellungen
            If .EinstellungenNeuePaarungen Is Nothing _
                Or .EinstellungenRangliste Is Nothing _
                Or .EinstellungenSchiedsrichterzettel Is Nothing _
                Or .EinstellungenSpielergebnisse Is Nothing Then
                MessageBox.Show("Es wurden noch nicht für alle Druckansichten Einstellungen gewählt",
                                "Fehlende Einstellungen",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation)
                Return
            End If
        End With
        DialogResult = True
        Close()
    End Sub
End Class