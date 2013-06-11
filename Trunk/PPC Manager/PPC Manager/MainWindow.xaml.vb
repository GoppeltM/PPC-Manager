Imports System.Collections.ObjectModel

Class MainWindow


    Public Shared AktiveCompetition As Competition

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        With New LadenNeu
            If Not .ShowDialog() Then Return
            AktiveCompetition = Competition.FromXML(.XMLPathText.Text, .CompetitionCombo.SelectedItem.ToString, .GewinnsätzeAnzahl.Value, .SatzDiffCheck.IsChecked)
            Application.Current.Resources("SpielRunden") = AktiveCompetition.SpielRunden
            Application.Current.Resources("SpielerListe") = AktiveCompetition.SpielerListe                        
            Title = AktiveCompetition.Altersgruppe
            EditorArea.Navigate(New Begegnungen(Me))
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
    
    Private Sub EditorArea_Navigated(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.NavigationEventArgs) Handles EditorArea.Navigated
        Select Case e.Content.GetType
            Case GetType(Begegnungen)                
                NächsteRunde.Visibility = Windows.Visibility.Visible
                Einstellungen.Visibility = Windows.Visibility.Collapsed
                OffeneBegegnungen.Visibility = Windows.Visibility.Visible            
            Case Else
                Throw New Exception("Unbekannter Seitentyp")
        End Select
    End Sub

    Private Sub Drucken_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Select Case e.Parameter.ToString
            Case "Rangliste"
                With New PrintDialog
                    If .ShowDialog Then
                        Dim paginator As New UserControlPaginator(Of RanglisteSeite)(AktiveCompetition.SpielerListe, _
                                                                                     New Size(.PrintableAreaWidth, .PrintableAreaHeight))
                        .PrintDocument(paginator, "Spieler Rangliste")
                    End If
                End With
            Case "Begegnungen"
                With New PrintDialog
                    If .ShowDialog Then
                        Dim paginator As New UserControlPaginator(Of SpielErgebnisZettel)(AktiveCompetition.SpielRunden.Peek, _
                                                                                     New Size(.PrintableAreaWidth, .PrintableAreaHeight))
                        .PrintDocument(paginator, "Spieler Rangliste")
                    End If
                End With
            Case Else
                Throw New ArgumentException("Unbekannter Command parameter", e.Parameter.ToString)
        End Select
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
        Select Case MessageBox.Show("Sollen Änderungen gespeichert und dieses Programm geschlossen werden?" _
                          , "Speichern und schließen?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
            Case MessageBoxResult.Cancel : e.Cancel = True
            Case MessageBoxResult.Yes : AktiveCompetition.SaveXML()
        End Select
    End Sub

    Private Sub MyWindow_Closed(sender As Object, e As EventArgs) Handles MyWindow.Closed
        My.Settings.Save()
    End Sub

    Private Sub RundeVerwerfen_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = AktiveCompetition.SpielRunden.Count > 1
    End Sub

    Private Sub RundeVerwerfen_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die aktuelle Runde verwerfen? Diese Aktion kann nicht rückgängig gemacht werden!", "Runde löschen?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) = MessageBoxResult.No Then
            Return
        End If
        AktiveCompetition.SpielRunden.Pop()
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        'Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist
        ViewSource.View.Refresh()

        ViewSource.View.MoveCurrentToFirst()
    End Sub
End Class
