Imports System.Collections.ObjectModel

Class MainWindow


    Friend AktiveCompetition As New Competition

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded        
        With New LadenNeu
            .ShowDialog()            
            AktiveCompetition = Competition.FromXML(.XMLPathText.Text, .CompetitionCombo.SelectedItem.ToString, .GewinnsätzeAnzahl.Value, .SatzDiffCheck.IsChecked)
            Application.Current.Resources("SpielRunden") = AktiveCompetition.SpielRunden
            Application.Current.Resources("SpielerListe") = AktiveCompetition.SpielerListe
            AktiveCompetition.Save()
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
        AktiveCompetition.Save()
    End Sub


    Private Sub Vorsortieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Vorsortieren.Click

        Dim sortierteListe = (From x In AktiveCompetition.SpielerListe
                             Order By x.TTRating Descending).ToList

        AktiveCompetition.SpielerListe.Clear()
        Dim current = 1
        For Each Spieler In sortierteListe            
            AktiveCompetition.SpielerListe.Add(Spieler)
            current += 1
        Next

    End Sub

 

    Private Sub TurnierStarten_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        If MessageBox.Show(Me, "Wollen Sie wirklich das Turnier starten? Vergewissern Sie sich, dass alle Spielregeln und die Startreihenfolge richtig ist bevor Sie beginnen.", _
                           "Turnier Starten?", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then

            EditorArea.Navigate(New Begegnungen)
        End If

    End Sub

    
    Private Sub EditorArea_Navigated(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.NavigationEventArgs) Handles EditorArea.Navigated
        Select Case e.Content.GetType
            Case GetType(Begegnungen)
                TurnierStarten.Visibility = Windows.Visibility.Collapsed
                Vorsortieren.Visibility = Windows.Visibility.Collapsed
                NächsteRunde.Visibility = Windows.Visibility.Visible
                Einstellungen.Visibility = Windows.Visibility.Collapsed
                OffeneBegegnungen.Visibility = Windows.Visibility.Visible
            Case GetType(StartListe)
                TurnierStarten.Visibility = Windows.Visibility.Visible
                Vorsortieren.Visibility = Windows.Visibility.Visible
                NächsteRunde.Visibility = Windows.Visibility.Collapsed
                Einstellungen.Visibility = Windows.Visibility.Visible
                OffeneBegegnungen.Visibility = Windows.Visibility.Collapsed
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
            .FileName = IO.Path.ChangeExtension(AktiveCompetition.DateiPfad, "xlsx")
            If .ShowDialog Then
                ExcelInterface.CreateFile(.FileName, AktiveCompetition.SpielerListe, AktiveCompetition.SpielRunden)
            End If

        End With

    End Sub
End Class
