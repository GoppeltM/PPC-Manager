Imports PPC_Manager

Public Class DruckEinstellungenDialog

    Private ReadOnly _Controller As IController
    Private ReadOnly _DruckerFabrik As IDruckerFabrik

    Public Sub New(Controller As IController, DruckerFabrik As IDruckerFabrik)
        _Controller = Controller
        _DruckerFabrik = DruckerFabrik
        InitializeComponent()
    End Sub

    Public ReadOnly Property Einstellungen As DruckEinstellungen
        Get
            Return CType(DataContext, DruckEinstellungen)
        End Get
    End Property


    Private Sub PrintSettingsClick(sender As Object, e As RoutedEventArgs)
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
            If .DruckeNeuePaarungen And .EinstellungenNeuePaarungen Is Nothing _
                Or .DruckeRangliste And .EinstellungenRangliste Is Nothing _
                Or .DruckeSchiedsrichterzettel And .EinstellungenSchiedsrichterzettel Is Nothing _
                Or .DruckeSpielergebnisse And .EinstellungenSpielergebnisse Is Nothing Then
                MessageBox.Show("Es wurden noch nicht für alle ausgewählten Druckansichten Einstellungen gewählt",
                                "Fehlende Einstellungen",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation)
                Return
            End If
        End With

        CType(FindName("Drucken"), Button).IsEnabled = False 'Verhindert doppelklicken, da das Drucken eine Sekunde oder so dauert
        PrintSelectedDocuments()
        DialogResult = True
        Close()
    End Sub

    Private Sub PrintSelectedDocuments()
        With Einstellungen
            If .DruckeNeuePaarungen Then
                _Focus(NeuePaarungen)
                Dim p = _DruckerFabrik.Neu(.EinstellungenNeuePaarungen)
                Dim doc = _Controller.DruckeNeuePaarungen(p.LeseKonfiguration)
                p.Drucken(doc, "Neue Begegnungen - Aushang")
                _Blur(NeuePaarungen)
            End If
            If .DruckeSchiedsrichterzettel Then
                _Focus(Schiedsrichterzettel)
                Dim p = _DruckerFabrik.Neu(.EinstellungenSchiedsrichterzettel)
                Dim doc = _Controller.DruckeSchiedsrichterzettel(p.LeseKonfiguration)
                p.Drucken(doc, "Schiedsrichterzettel")
                _Blur(Schiedsrichterzettel)
            End If
            If .DruckeSpielergebnisse Then
                _Focus(Spielergebnisse)
                Dim p = _DruckerFabrik.Neu(.EinstellungenSpielergebnisse)
                Dim doc = _Controller.DruckeSpielergebnisse(p.LeseKonfiguration)
                p.Drucken(doc, "Spielergebnisse")
                _Blur(Spielergebnisse)
            End If
            If .DruckeRangliste Then
                _Focus(Rangliste)
                Dim p = _DruckerFabrik.Neu(.EinstellungenRangliste)
                Dim doc = _Controller.DruckeRangliste(p.LeseKonfiguration)
                p.Drucken(doc, "Rangliste")
                _Blur(Rangliste)
            End If
        End With
    End Sub

    Private _OriginalContent As Object

    Private Sub _Focus(Button As Primitives.ToggleButton)
        With Button
            _OriginalContent = .Content
            .Content = "→ " + .Content.ToString()
            .FontWeight = FontWeights.Bold
        End With
    End Sub

    Private Sub _Blur(Button As Primitives.ToggleButton)
        With Button
            .Content = _OriginalContent
            .FontWeight = FontWeights.Normal
        End With
    End Sub
End Class