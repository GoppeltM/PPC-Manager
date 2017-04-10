Class MainWindow

    Private ReadOnly _Controller As IController

    Sub New(controller As IController)
        InitializeComponent()
        _Controller = controller
        If controller Is Nothing Then Throw New ArgumentNullException("controller")
        Me.Begegnungen.LiveListe.DataContext = _Controller.AktiveCompetition.SpielerListe.Select(AddressOf FilterSpieler)
        Me.DataContext = _Controller
        Me.Title = controller.AktiveCompetition.Altersgruppe
    End Sub

    Public Function FilterSpieler(s As SpielerInfo) As Boolean
        Dim ausgeschiedeneSpieler = _Controller.AktiveCompetition.SpielRunden.AusgeschiedeneSpieler
        Dim AusgeschiedenVorBeginn = Aggregate x In ausgeschiedeneSpieler Where x.Runde = 0 And
                            x.Spieler = s Into Any()

        Return Not AusgeschiedenVorBeginn
    End Function

    Private Sub Close_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub Close_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        My.Application.MainWindow.Close()
    End Sub

    Private Sub Save_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub Save_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        _Controller.SaveXML()
    End Sub

    Private Sub RundeVerwerfen_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = _Controller.HatRunden
    End Sub

    Private Sub RundeVerwerfen_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die aktuelle Runde verwerfen? Diese Aktion kann nicht rückgängig gemacht werden!", "Runde löschen?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) = MessageBoxResult.No Then
            Return
        End If
        _Controller.RundeVerwerfen()
        NavigationCommands.Refresh.Execute(Nothing, Begegnungen)
    End Sub

    Private Sub NächsteRunde_CanExecute(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = _Controller.NächsteRunde_CanExecute()
    End Sub

    Private Sub Ausscheiden_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim Spieler = CType(e.Parameter, SpielerInfo)
        _Controller.SpielerAusscheiden(Spieler)
    End Sub

    Private Sub NeuePartie_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim dialog = New NeueSpielPartieDialog
        dialog.RundenNameTextBox.Text = "Runde " & _Controller.AktiveCompetition.SpielRunden.Count
        If Not dialog.ShowDialog Then Return
        Dim rundenName = dialog.RundenNameTextBox.Text

        Dim AusgewählteSpieler = CType(e.Parameter, IEnumerable(Of SpielerInfo))
        Dim spielerA = AusgewählteSpieler(0)
        Dim SpielerB = AusgewählteSpieler(1)
        _Controller.NeuePartie(rundenName, spielerA, SpielerB)
    End Sub

    Private Sub NächsteRunde_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die nächste Runde starten? Sobald die nächste Runde beginnt, können die aktuellen Ergebnisse nicht mehr verändert werden.",
                   "Nächste Runde?", MessageBoxButton.YesNo) <> MessageBoxResult.Yes Then
            Return
        End If

        If CBool(My.Settings.AutoSaveAn) Then
            _Controller.SaveXML()
        End If

        Try
            _Controller.NächsteRunde_Execute()
        Catch ex As ExcelNichtBeschreibbarException
            MessageBox.Show(
                String.Format("Kein Schreibzugriff auf Excel Datei möglich. Bitte Excel vor Beginn der nächsten Runde schließen!",
                              _Controller.ExcelPfad),
                            "Excel offen", MessageBoxButton.OK)
            Return
        End Try

        If CBool(My.Settings.AutoSaveAn) Then
            _Controller.SaveExcel()
        End If

        Resources("PlayoffAktiv") = False
        NavigationCommands.Refresh.Execute(Nothing, Begegnungen)
    End Sub

    Private Sub PlayOff_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die Playoffs beginnen? Es wird eine leere Runde erzeugt, und die vorigen Ergebnisse können nicht verändert werden.",
                   "Nächste Runde?", MessageBoxButton.YesNo) <> MessageBoxResult.Yes Then
            Return
        End If

        _Controller.NächstesPlayoff_Execute()

        Resources("PlayoffAktiv") = True
        NavigationCommands.Refresh.Execute(Nothing, Begegnungen)
    End Sub

    Private Sub Drucken_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        _Controller.RundenbeginnDrucken(New Printer)
    End Sub

    Private Sub RanglisteDrucken_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        _Controller.RundenendeDrucken(New Printer)
    End Sub

    Private Sub BegegnungenFiltern_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub Exportieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Exportieren.Click

        With LadenNeu.SpeichernDialog
            .Filter = "Excel 2007 (oder höher) Dateien|*.xlsx"
            .FileName = _Controller.ExcelPfad
            .InitialDirectory = My.Settings.LetztesVerzeichnis
            If .ShowDialog Then
                _Controller.ExcelExportieren(.FileName)
            End If
        End With
    End Sub

    Private Sub MyWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles MyWindow.Closing
        Select Case MessageBox.Show("Das Programm wird geschlossen. Sollen Änderungen gespeichert werden?" _
                          , "Speichern und schließen?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
            Case MessageBoxResult.Cancel : e.Cancel = True
            Case MessageBoxResult.Yes : _Controller.SaveXML()
        End Select
    End Sub

    Private Sub MyWindow_Closed(sender As Object, e As EventArgs) Handles MyWindow.Closed
        My.Settings.Save()
    End Sub

    Private Sub Drucken_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = _Controller.HatRunden
    End Sub

    Private Sub RanglisteDrucken_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

End Class
