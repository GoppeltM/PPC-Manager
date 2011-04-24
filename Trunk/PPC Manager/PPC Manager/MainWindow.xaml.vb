Imports System.Collections.ObjectModel
Imports <xmlns="http://PPCManager/SpeicherStand">

Class MainWindow

    Friend SpielRunden As SpielRunden
    Friend AktiveSpieler As SpielerListe
    Friend AusgeschiedeneSpieler As SpielerListe

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        SpielRunden = CType(FindResource("SpielRunden"), SpielRunden)
        AktiveSpieler = CType(FindResource("AktiveSpieler"), SpielerListe)
        AusgeschiedeneSpieler = CType(FindResource("AusgeschiedeneSpieler"), SpielerListe)
    End Sub

    Private Sub Vereine_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Einstellungen.Click
        Dim dialog As New Optionen
        dialog.ShowDialog()
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

        
        If My.Settings.AktuelleSpeicherdatei = String.Empty Then
            SaveAs_Executed(sender, e)
            Return
        End If

        Dim doc = New XDocument(<PPCTurnier>
                                    <AktiveSpieler>
                                        <%= From x In AktiveSpieler Let y = x.ToXML Select y %>
                                    </AktiveSpieler>
                                    <AusgeschiedeneSpieler>
                                        <%= From x In AusgeschiedeneSpieler Let y = x.ToXML Select y %>
                                    </AusgeschiedeneSpieler>

                                    <%= SpielRunden.ToXML %>
                                </PPCTurnier>)

        doc.Save(My.Settings.AktuelleSpeicherdatei)
    End Sub

    Private Sub Open_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim dialog = New OpenFileDialog
        With dialog
            .Filter = "XML Dateien |*.xml"
            If My.Settings.AktuelleSpeicherdatei = String.Empty Then
                .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            Else
                .InitialDirectory = IO.Path.GetDirectoryName(My.Settings.AktuelleSpeicherdatei)
            End If

            If .ShowDialog(Me) = True Then

                Dim doc = XDocument.Load(.FileName)

                aktiveSpieler.FromXML(SpielRunden, doc.Root.<AktiveSpieler>)
                ausgeschiedeneSpieler.FromXML(SpielRunden, doc.Root.<AusgeschiedeneSpieler>)

                SpielRunden.FromXML(AktiveSpieler.Concat(AusgeschiedeneSpieler), doc.Root.<SpielRunden>.<SpielRunde>)

                If SpielRunden.Any Then
                    EditorArea.Navigate(New Begegnungen)
                Else
                    EditorArea.Navigate(New StartListe)
                End If

            End If
        End With
    End Sub

    Private Sub SaveAs_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim dialog As New SaveFileDialog
        dialog.Filter = "XML Dateien |*.xml"
        If dialog.ShowDialog() Then
            My.Settings.AktuelleSpeicherdatei = dialog.FileName
            Save_Executed(sender, e)
        End If

    End Sub

    Private Sub Vorsortieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Vorsortieren.Click
       
        Dim sortierteListe = From x In AktiveSpieler
                             Let spielDiff = My.Settings.SpielKlassen.IndexOf(x.SpielKlasse) * 2 + x.Position
                             Order By x.Rating Descending, x.Rang Ascending, x.TurnierKlasse Ascending, spielDiff Descending Select x

        AktiveSpieler.Clear()
        For Each Spieler In sortierteListe
            AktiveSpieler.Add(Spieler)
        Next

    End Sub

    Private Sub NächsteRunde_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NächsteRunde.Click

    End Sub

    Private Sub TurnierStarten_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        EditorArea.Navigate(New Begegnungen)
    End Sub

    Private Sub EditorArea_Navigated(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.NavigationEventArgs) Handles EditorArea.Navigated
        Select Case e.Content.GetType
            Case GetType(Begegnungen)
                TurnierStarten.Visibility = Windows.Visibility.Collapsed
                Vorsortieren.Visibility = Windows.Visibility.Collapsed
                NächsteRunde.Visibility = Windows.Visibility.Visible
            Case GetType(StartListe)
                TurnierStarten.Visibility = Windows.Visibility.Visible
                Vorsortieren.Visibility = Windows.Visibility.Visible
                NächsteRunde.Visibility = Windows.Visibility.Collapsed
            Case Else
                Throw New Exception("Unbekannter Seitentyp")
        End Select
    End Sub
End Class
