Imports System.Collections.ObjectModel

Class MainWindow


    Public Shared AktiveCompetition As Competition

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        With New LadenNeu
            If Not .ShowDialog() Then Return
            AktiveCompetition = Competition.FromXML(.XMLPathText.Text, .CompetitionCombo.SelectedItem.ToString, .GewinnsätzeAnzahl.Value, .SatzDiffCheck.IsChecked, .SonneBorn.IsChecked)
            Me.DataContext = AktiveCompetition
        End With
    End Sub

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
        AktiveCompetition.SaveXML()
    End Sub

    Private Sub RundeVerwerfen_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = AktiveCompetition.SpielRunden.Count > 0
    End Sub

    Private Sub RundeVerwerfen_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die aktuelle Runde verwerfen? Diese Aktion kann nicht rückgängig gemacht werden!", "Runde löschen?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) = MessageBoxResult.No Then
            Return
        End If
        With AktiveCompetition.SpielRunden
            .Pop()
            Dim überzählig = (From x In .AusgeschiedeneSpieler Where x.Runde > .Count).ToList

            For Each ausgeschieden In überzählig
                .AusgeschiedeneSpieler.Remove(ausgeschieden)
            Next
        End With

        NavigationCommands.Refresh.Execute(Nothing, Begegnungen)
    End Sub

    Private Sub NächsteRunde_CanExecute(ByVal sender As System.Object, ByVal e As CanExecuteRoutedEventArgs)
        With AktiveCompetition.SpielRunden
            If Not .Any Then
                e.CanExecute = True
                Return
            End If

            Dim AktuellePartien = .Peek.ToList

            Dim AlleAbgeschlossen = Aggregate x In AktuellePartien Into All(x.Abgeschlossen)

            e.CanExecute = AlleAbgeschlossen
        End With
        
    End Sub

    Private Sub NächsteRunde_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die nächste Runde starten? Sobald die nächste Runde beginnt, können die aktuellen Ergebnisse nicht mehr verändert werden.", _
                   "Nächste Runde?", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then

            Try
                If IO.File.Exists(AktiveCompetition.ExcelPfad) Then
                    Using file = IO.File.OpenRead(AktiveCompetition.ExcelPfad)

                    End Using
                End If            
            Catch ex As IO.IOException
                MessageBox.Show(String.Format("Kein Schreibzugriff auf Excel Datei {0} möglich. Bitte Excel vor Beginn der nächsten Runde schließen!", AktiveCompetition.ExcelPfad),
                                "Excel offen", MessageBoxButton.OK)
                Return
            End Try
            RundeBerechnen()
        End If

    End Sub

    Private Sub RundeBerechnen()

        If CBool(My.Settings.AutoSaveAn) Then
            MainWindow.AktiveCompetition.SaveXML()
        End If

        With AktiveCompetition
            Dim AktiveListe = .SpielerListe.ToList
            For Each Ausgeschieden In .SpielRunden.AusgeschiedeneSpieler
                AktiveListe.Remove(Ausgeschieden.Spieler)
            Next
            Dim RundenName = "Runde " & .SpielRunden.Count + 1
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
        End With


        NavigationCommands.Refresh.Execute(Nothing, Begegnungen)

        If CBool(My.Settings.AutoSaveAn) Then
            MainWindow.AktiveCompetition.SaveExcel()
        End If

    End Sub


    Private Sub PlayOff_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die Playoffs beginnen? Es wird eine leere Runde erzeugt, und die vorigen Ergebnisse können nicht verändert werden.", _
                   "Nächste Runde?", MessageBoxButton.YesNo) <> MessageBoxResult.Yes Then
            Return
        End If

        If CBool(My.Settings.AutoSaveAn) Then
            MainWindow.AktiveCompetition.SaveXML()
        End If

        With MainWindow.AktiveCompetition
            Dim spielRunde As New SpielRunde
            .SpielRunden.Push(spielRunde)
            Resources("PlayoffAktiv") = True            
        End With

        NavigationCommands.Refresh.Execute(Nothing, Begegnungen)

        If CBool(My.Settings.AutoSaveAn) Then
            MainWindow.AktiveCompetition.SaveExcel()
        End If
    End Sub

    Private Sub Drucken_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        With New PrintDialog
            .UserPageRangeEnabled = True
            If .ShowDialog Then
                Dim size = New Size(.PrintableAreaWidth, .PrintableAreaHeight)
                Dim PaarungenPaginator As New UserControlPaginator(Of NeuePaarungen)(From x In AktiveCompetition.SpielRunden.Peek Where Not TypeOf x Is FreiLosSpiel, size)
                Dim SchiriPaginator As New UserControlPaginator(Of SchiedsrichterZettel)(AktiveCompetition.SpielRunden.Peek, size)
                .PrintDocument(New PaginatingPaginator({PaarungenPaginator, SchiriPaginator}), "Neue Begegnungen - Aushang und Schiedsrichterzettel")
            End If
        End With
    End Sub

    Private Sub RanglisteDrucken_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        With New PrintDialog
            .UserPageRangeEnabled = True
            If .ShowDialog Then
                Dim size = New Size(.PrintableAreaWidth, .PrintableAreaHeight)

                Dim Spielpartien As IEnumerable(Of SpielPartie) = New List(Of SpielPartie)
                With AktiveCompetition.SpielRunden
                    If .Any Then
                        Spielpartien = From x In AktiveCompetition.SpielRunden.Peek Where Not TypeOf x Is FreiLosSpiel
                    End If
                End With

                Dim ErgebnissePaginator As New UserControlPaginator(Of SpielErgebnisse)(Spielpartien, size)
                Dim l = AktiveCompetition.SpielerListe.ToList
                l.Sort()
                l.Reverse()

                Dim RanglistePaginator As New UserControlPaginator(Of RanglisteSeite)(l, size)
                .PrintDocument(New PaginatingPaginator({ErgebnissePaginator, RanglistePaginator}), "Rundenende - Aushang und Rangliste")
            End If
        End With        
    End Sub

    Private Sub BegegnungenFiltern_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub Exportieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Exportieren.Click

        With LadenNeu.SpeichernDialog
            .Filter = "Excel 2007 (oder höher) Dateien|*.xlsx"
            .FileName = AktiveCompetition.ExcelPfad
            .InitialDirectory = My.Settings.LetztesVerzeichnis
            If .ShowDialog Then
                Dim spieler = AktiveCompetition.SpielerListe.ToList
                spieler.Sort()
                ExcelInterface.CreateFile(.FileName, spieler, AktiveCompetition.SpielRunden)
            End If

        End With

    End Sub

    Private Sub MyWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles MyWindow.Closing
        Select Case MessageBox.Show("Das Programm wird geschlossen. Sollen Änderungen gespeichert werden?" _
                          , "Speichern und schließen?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
            Case MessageBoxResult.Cancel : e.Cancel = True
            Case MessageBoxResult.Yes : AktiveCompetition.SaveXML()
        End Select
    End Sub

    Private Sub MyWindow_Closed(sender As Object, e As EventArgs) Handles MyWindow.Closed
        My.Settings.Save()
    End Sub

    
    Private Sub Drucken_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = AktiveCompetition.SpielRunden.Any
    End Sub

    Private Sub RanglisteDrucken_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = AktiveCompetition.SpielerListe.Any
    End Sub
End Class
