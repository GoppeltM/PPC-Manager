Imports System.Globalization

Class Begegnungen

    Private Property MainWindow As MainWindow

    Friend Sub New(main As MainWindow)
        InitializeComponent()
        MainWindow = main
    End Sub

    Friend Property BegegnungenView As CollectionViewSource

    Private Sub BegegnungenListView_Filter(ByVal sender As System.Object, ByVal e As System.Windows.Data.FilterEventArgs)

        If Not My.Settings.BegegnungenFiltern Then
            e.Accepted = True
            Return
        End If

        Dim partie As SpielPartie = CType(e.Item, SpielPartie)        
        e.Accepted = Not Abgeschlossen(partie)

    End Sub

    Friend Function Abgeschlossen(ByVal partie As SpielPartie) As Boolean

        Dim SätzeLinks = Aggregate x In partie Where x.PunkteLinks = My.Settings.GewinnPunkte Into Count()
        Dim SätzeRechts = Aggregate x In partie Where x.PunkteRechts = My.Settings.GewinnPunkte Into Count()

        Dim gesamtAbgeschlossen = Math.Max(SätzeLinks, SätzeRechts)
        Dim gewinnSätze = MainWindow.AktiveCompetition.Gewinnsätze
        Return gesamtAbgeschlossen >= gewinnSätze
    End Function

    Private Sub Begegnungen_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded        
        Dim SpielRunden = CType(FindResource("SpielRunden"), SpielRunden)
        BegegnungenView = CType(FindResource("PartieView"), CollectionViewSource)        
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        SpielerView.View.Refresh()
        ViewSource.View.Refresh()
        'Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist.
        ViewSource.View.MoveCurrentToFirst()
    End Sub

    Private Sub SatzLinks_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim x = CType(CType(sender, Button).DataContext, Satz)
        x.PunkteLinks = My.Settings.GewinnPunkte
        x.PunkteRechts = 0
        SatzUpdate()
    End Sub

    Private Sub SatzRechts_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim x = CType(CType(sender, Button).DataContext, Satz)
        x.PunkteRechts = My.Settings.GewinnPunkte
        x.PunkteLinks = 0
        SatzUpdate()
    End Sub

    Private Sub SatzUpdate()
        Dim partie = CType(DetailGrid.DataContext, SpielPartie)
        Dim AbgeschlosseneSätze = Aggregate x In partie Where Math.Max(x.PunkteLinks, x.PunkteRechts) = My.Settings.GewinnPunkte Into Count()

        Dim Fertig = Abgeschlossen(partie)

        If AbgeschlosseneSätze >= partie.Count AndAlso Not Fertig Then
            partie.Add(New Satz)
        End If
        partie.Update()
        ' Refresh erzwingt u.a. Filter
        If Fertig Then
            BegegnungenView.View.Refresh()
            Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
            SpielerView.View.Refresh()
        End If
        
    End Sub

    Private Sub Ausscheiden_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        If LifeListe.SelectedIndex <> -1 Then
            Dim Spieler = CType(LifeListe.SelectedItem, Spieler)
            If Not Spieler.Ausgeschieden Then
                e.CanExecute = True
                Return
            End If
        End If
        e.CanExecute = False
    End Sub

    Private Sub Ausscheiden_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim runde = CType(FindResource("SpielRunden"), SpielRunden).Peek
        Dim Spieler = CType(LifeListe.SelectedItem, Spieler)
        If MessageBox.Show(String.Format("Sind Sie sicher dass sie Spieler {0} ausscheiden lassen wollen? Dieser Vorgang kann nicht rückgängig gemacht werden!", Spieler.Nachname), _
                        "Spieler ausscheiden?", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
            Spieler.AusscheidenLassen()
        End If
    End Sub

    Private Sub BegegnungenFiltern_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        BegegnungenView.View.Refresh()
    End Sub

    Private Sub NächsteRunde_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die nächste Runde starten? Sobald die nächste Runde beginnt, können die aktuellen Ergebnisse nicht mehr verändert werden.", _
                   "Nächste Runde?", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
            RundeBerechnen()
        End If

    End Sub

    Private Sub RundeBerechnen()

        If CBool(My.Settings.AutoSaveAn) Then
            MainWindow.AktiveCompetition.SaveXML()
        End If

        With MainWindow.AktiveCompetition
            Dim AktiveListe = .SpielerListe.ToList
            For Each Runde In .SpielRunden
                For Each Ausgeschieden In Runde.AusgeschiedeneSpieler
                    AktiveListe.Remove(Ausgeschieden)
                Next
            Next
            Dim RundenName = "Runde " & .SpielRunden.Count
            Dim begegnungen = PaketBildung.organisierePakete(RundenName, AktiveListe, .SpielRunden.Count)
            Dim Zeitstempel = Date.Now
            For Each partie In begegnungen
                partie.ZeitStempel = Zeitstempel
            Next

            Dim spielRunde As New SpielRunde

            For Each begegnung In begegnungen
                spielRunde.Add(begegnung)
            Next
            .SpielRunden.Push(spielRunde)            
            PlayOffAktiv = True
            LifeListe.SelectionMode = SelectionMode.Single
        End With

        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        'Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist
        ViewSource.View.Refresh()

        ViewSource.View.MoveCurrentToFirst()

        If CBool(My.Settings.AutoSaveAn) Then
            MainWindow.AktiveCompetition.SaveExcel()
        End If

    End Sub

    Private Sub NächsteRunde_CanExecute(ByVal sender As System.Object, ByVal e As CanExecuteRoutedEventArgs)
        Dim SpielRunden = CType(FindResource("SpielRunden"), SpielRunden)
        If Not SpielRunden.Any Then
            e.CanExecute = True
            Return
        End If

        Dim AktuellePartien = SpielRunden.Peek.ToList

        Dim AlleAbgeschlossen = Aggregate x In AktuellePartien Into All(Abgeschlossen(x))

        e.CanExecute = AlleAbgeschlossen
    End Sub

    Private Sub Window_Initialized(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Initialized
        Dim res = CType(FindResource("RanglisteDataProvider"), ObjectDataProvider)
        Dim liste = CType(FindResource("SpielerListe"), SpielerListe)
        res.ObjectInstance = liste
    End Sub

    Private Property PlayOffAktiv As Boolean = False

    Private Sub PlayOff_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        With MainWindow.AktiveCompetition        
            Dim spielRunde As New SpielRunde
            .SpielRunden.Push(spielRunde)
            PlayOffAktiv = True
            LifeListe.SelectionMode = SelectionMode.Extended            
        End With

        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        'Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist
        ViewSource.View.Refresh()

        ViewSource.View.MoveCurrentToFirst()

        If CBool(My.Settings.AutoSaveAn) Then
            ApplicationCommands.Save.Execute(Nothing, Me)
        End If
    End Sub

    Private Sub NeuePartie_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = PlayOffAktiv AndAlso LifeListe.SelectedItems.Count = 2
    End Sub

    Private Sub NeuePartie_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim AktuelleRunde = MainWindow.AktiveCompetition.SpielRunden.Peek()
        Dim AusgewählteSpieler = LifeListe.SelectedItems
        Dim dialog = New NeueSpielPartieDialog
        dialog.RundenNameTextBox.Text = "Runde " & MainWindow.AktiveCompetition.SpielRunden.Count
        If Not dialog.ShowDialog Then Return

        Dim neueSpielPartie = New SpielPartie(dialog.RundenNameTextBox.Text, CType(AusgewählteSpieler(0), Spieler), CType(AusgewählteSpieler(1), Spieler))
        neueSpielPartie.Add(New Satz)
        AktuelleRunde.Add(neueSpielPartie)
    End Sub
End Class



Public Class DoppelbreitenGrid
    Inherits Grid

    'Protected Overrides Function MeasureOverride(constraint As Size) As Size
    '    MyBase.MeasureOverride(constraint)

    '    Return New Size(400, 60)
    'End Function

    'Protected Overrides Function ArrangeOverride(arrangeSize As Size) As Size
    '    Dim size = MyBase.ArrangeOverride(arrangeSize)

    '    Return New Size(400, 60)
    'End Function
End Class