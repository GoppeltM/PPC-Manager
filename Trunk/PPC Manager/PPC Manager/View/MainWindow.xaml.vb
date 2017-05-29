Public Class PlayoffCommand
    Inherits RoutedCommand
End Class

Public Class MeineCommands

    Public Shared ReadOnly Property Playoff As RoutedUICommand = New RoutedUICommand("Überprüft ob Playoff aktiv ist",
                                                                                     "Playoff",
                                                                                     GetType(MeineCommands))

    Public Shared ReadOnly Property BegegnungenFilter As RoutedUICommand = New RoutedUICommand("Wird gefeuert wenn der Begegnungenfilter sich ändert",
                                                                                             "BegegnungenFilter", GetType(MeineCommands))

    Public Shared ReadOnly Property PartieAusgewählt As RoutedUICommand = New RoutedUICommand("Wird gefeuert wenn eine Partie ausgewählt wird", "PartieAusgewählt", GetType(MeineCommands))
End Class

Class MainWindow

    Private ReadOnly _Controller As IController
    Private ReadOnly _Spielrunden As SpielRunden
    Private ReadOnly _Spielstand As ISpielstand
    Private ReadOnly _DruckEinstellungen As DruckEinstellungen
    Private Property PlayoffIstAktiv As Boolean = False

    Sub New(controller As IController,
            spielerliste As IEnumerable(Of Spieler),
            spielrunden As SpielRunden,
            titel As String,
            spielstand As ISpielstand,
            spielerVergleicher As IComparer)
        InitializeComponent()
        _Controller = controller
        _Spielrunden = spielrunden
        _Spielstand = spielstand
        _DruckEinstellungen = New DruckEinstellungen
        Title = titel
        If controller Is Nothing Then Throw New ArgumentNullException("controller")
        Dim s = New SpielerListe
        For Each spieler In spielerliste.Where(AddressOf FilterSpieler)
            s.Add(spieler)
        Next
        LiveListe.DataContext = s
        LiveListe.SpielerComparer = New InvertComparer(spielerVergleicher)
        Begegnungen.SpielPartienListe.IstAbgeschlossen = AddressOf _Spielstand.IstAbgeschlossen
        Begegnungen.IstAbgeschlossen = AddressOf _Spielstand.IstAbgeschlossen
        Begegnungen.DetailGrid.IstAbgeschlossen = AddressOf _Spielstand.IstAbgeschlossen
        AktualisiereDaten()
    End Sub

    Private Class InvertComparer
        Implements IComparer

        Private ReadOnly _Comparer As IComparer

        Public Sub New(comparer As IComparer)
            _Comparer = comparer
        End Sub

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Return _Comparer.Compare(y, x)
        End Function
    End Class

    Private Sub AktualisiereDaten()
        If Not _Spielrunden.Any Then
            Me.Begegnungen.DataContext = New List(Of SpielPartie)
            Return
        End If
        Me.Begegnungen.DataContext = _Spielrunden.Peek
        Me.Begegnungen.DetailGrid.DataContext = Nothing
        NavigationCommands.Refresh.Execute(Nothing, LiveListe)
    End Sub

    Public Function FilterSpieler(s As SpielerInfo) As Boolean
        Return Not _Spielrunden.First.AusgeschiedeneSpielerIDs.Contains(s.Id)
    End Function

    Private Sub Ja(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub Close_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        My.Application.MainWindow.Close()
    End Sub

    Private Sub Save_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        _Controller.SaveXML()
    End Sub

    Private Sub RundeVerwerfen_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        Dim nullteRunde = 1
        e.CanExecute = _Spielrunden.Count > nullteRunde
    End Sub

    Private Sub RundeVerwerfen_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die aktuelle Runde verwerfen? Diese Aktion kann nicht rückgängig gemacht werden!", "Runde löschen?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) = MessageBoxResult.No Then
            Return
        End If
        With _Spielrunden
            .Pop()
        End With
        AktualisiereDaten()
    End Sub

    Private Sub Ausscheiden_CanExecute(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        If LiveListe.LiveListe.SelectedIndex <> -1 Then
            Dim Spieler = CType(LiveListe.LiveListe.SelectedItem, Spieler)
            If Not Spieler.Ausgeschieden Then
                e.CanExecute = True
                Return
            End If
        End If
        e.CanExecute = False
    End Sub

    Private Sub NeuePartie_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        If Not MeineCommands.Playoff.CanExecute(Nothing, Me) Then
            e.CanExecute = False
            Return
        End If
        e.CanExecute = LiveListe.LiveListe.SelectedItems.Count = 2
    End Sub

    Private Sub NächsteRunde_CanExecute(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        With _Spielrunden
            If Not .Any Then
                e.CanExecute = True
                Return
            End If

            Dim AktuellePartien = .Peek.ToList
            Dim AlleAbgeschlossen = Aggregate x In AktuellePartien Into All(_Spielstand.IstAbgeschlossen(x))

            e.CanExecute = AlleAbgeschlossen
        End With
    End Sub

    Private Sub Ausscheiden_Execute(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim spieler = CType(LiveListe.LiveListe.SelectedItem, SpielerInfo)
        If Not MessageBox.Show(
            String.Format("Sind Sie sicher dass sie Spieler {0} ausscheiden lassen wollen? Dieser Vorgang kann nicht rückgängig gemacht werden!",
                          spieler.Nachname),
                        "Spieler ausscheiden?", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
            Return
        End If
        _Spielrunden.Peek.AusgeschiedeneSpielerIDs.Add(spieler.Id)
        NavigationCommands.Refresh.Execute(Nothing, LiveListe)
    End Sub

    Private Sub NeuePartie_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim dialog = New NeueSpielPartieDialog
        dialog.RundenNameTextBox.Text = "Runde " & _Spielrunden.Count
        If Not dialog.ShowDialog Then Return
        Dim rundenName = dialog.RundenNameTextBox.Text

        Dim AusgewählteSpieler = LiveListe.LiveListe.SelectedItems.OfType(Of SpielerInfo)
        Dim spielerA = AusgewählteSpieler(0)
        Dim SpielerB = AusgewählteSpieler(1)
        Dim AktuelleRunde = _Spielrunden.Peek()
        Dim neueSpielPartie = New SpielPartie(rundenName,
                                              spielerA,
                                              SpielerB)
        neueSpielPartie.ZeitStempel = Date.Now
        AktuelleRunde.Add(neueSpielPartie)
    End Sub

    Private Sub NächsteRunde_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die nächste Runde starten? Sobald die nächste Runde beginnt, können die aktuellen Ergebnisse nicht mehr verändert werden.",
                   "Nächste Runde?", MessageBoxButton.YesNo) <> MessageBoxResult.Yes Then
            Return
        End If

        If My.Settings.AutoSaveAn Then
            _Controller.SaveXML()
        End If

        Try
            Dim rundenName = "Runde " & _Spielrunden.Count
            Dim runde = _Controller.NächsteRunde(rundenName)
            _Spielrunden.Push(runde)
        Catch ex As ExcelNichtBeschreibbarException
            MessageBox.Show(
                String.Format("Kein Schreibzugriff auf Excel Datei möglich. Bitte Excel vor Beginn der nächsten Runde schließen!",
                              _Controller.ExcelPfad),
                            "Excel offen", MessageBoxButton.OK)
            Return
        End Try

        If My.Settings.AutoSaveAn Then
            _Controller.SaveExcel()
        End If

        PlayoffIstAktiv = False
        AktualisiereDaten()
    End Sub

    Private Sub PlayOff_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die Playoffs beginnen? Es wird eine leere Runde erzeugt, und die vorigen Ergebnisse können nicht verändert werden.",
                   "Nächste Runde?", MessageBoxButton.YesNo) <> MessageBoxResult.Yes Then
            Return
        End If

        If My.Settings.AutoSaveAn Then
            _Controller.SaveXML()
        End If
        _Spielrunden.Push(New SpielRunde)

        If My.Settings.AutoSaveAn Then
            _Controller.SaveExcel()
        End If

        PlayoffIstAktiv = True
        AktualisiereDaten()
    End Sub

    Private Sub AllesDrucken_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim dialog = New DruckEinstellungenDialog
        dialog.DataContext = _DruckEinstellungen
        If Not dialog.ShowDialog Then
            Return
        End If
        With _DruckEinstellungen
            If .DruckeNeuePaarungen Then
                Dim p = New Printer(.EinstellungenNeuePaarungen)
                Dim doc = _Controller.DruckeNeuePaarungen(p.LeseKonfiguration)
                p.Drucken(doc, "Neue Begegnungen - Aushang")
            End If
            If .DruckeRangliste Then
                Dim p = New Printer(.EinstellungenRangliste)
                Dim doc = _Controller.DruckeNeuePaarungen(p.LeseKonfiguration)
                p.Drucken(doc, "Rangliste")
            End If
            If .DruckeSchiedsrichterzettel Then
                Dim p = New Printer(.EinstellungenSchiedsrichterzettel)
                Dim doc = _Controller.DruckeNeuePaarungen(p.LeseKonfiguration)
                p.Drucken(doc, "Schiedsrichterzettel")
            End If
            If .DruckeSpielergebnisse Then
                Dim p = New Printer(.EinstellungenSpielergebnisse)
                Dim doc = _Controller.DruckeNeuePaarungen(p.LeseKonfiguration)
                p.Drucken(doc, "Spielergebnisse")
            End If
        End With
    End Sub

    Private Sub Drucken_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim dialog = New PrintDialog
        If dialog.ShowDialog Then
            Dim p = New Printer(dialog)
            Dim doc = _Controller.DruckeNeuePaarungen(p.LeseKonfiguration)
            Dim doc2 = _Controller.DruckeSchiedsrichterzettel(p.LeseKonfiguration)
            p.Drucken(doc, "Neue Begegnungen - Aushang")
            p.Drucken(doc2, "Schiedsrichterzettel")
        End If

    End Sub

    Private Sub RanglisteDrucken_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim dialog = New PrintDialog
        If dialog.ShowDialog Then
            Dim p = New Printer(dialog)
            Dim doc = _Controller.DruckeRangliste(p.LeseKonfiguration)
            Dim doc2 = _Controller.DruckeSpielergebnisse(p.LeseKonfiguration)
            p.Drucken(doc, "Rangliste")
            p.Drucken(doc2, "Spielergebnisse")
        End If
    End Sub

    Private Sub Exportieren_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)

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
        e.CanExecute = _Spielrunden.Any
    End Sub

    Private Sub PlayoffAktiv(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = PlayoffIstAktiv
    End Sub

    Private Sub BegegnungenFiltergeändert_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim filtern = CBool(e.Parameter)
        Begegnungen.BegegnungenFiltern = filtern
        Begegnungen.Update()
    End Sub
End Class
