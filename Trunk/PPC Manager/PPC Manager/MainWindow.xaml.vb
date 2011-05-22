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
        Einstellungen_Click(sender, e)
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

        Dim doc = getXmlDocument()

        doc.Save(My.Settings.AktuelleSpeicherdatei)
    End Sub

    Private Function getXmlDocument() As XDocument

        Dim doc = New XDocument(<PPCTurnier TurnierName=<%= My.Settings.TurnierName %> GewinnSätze=<%= My.Settings.GewinnSätze %> SatzDifferenz=<%= My.Settings.BerücksichtigeSatzDiff %>>
                                    <AktiveSpieler>
                                        <%= From x In AktiveSpieler Let y = x.ToXML Select y %>
                                    </AktiveSpieler>
                                    <AusgeschiedeneSpieler>
                                        <%= From x In AusgeschiedeneSpieler Let y = x.ToXML Select y %>
                                    </AusgeschiedeneSpieler>

                                    <%= SpielRunden.ToXML %>
                                </PPCTurnier>)
        Return doc
    End Function

    Friend Sub Open_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
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

                My.Settings.GewinnSätze = Integer.Parse(doc.Root.@GewinnSätze)
                My.Settings.BerücksichtigeSatzDiff = Boolean.Parse(doc.Root.@SatzDifferenz)
                My.Settings.TurnierName = doc.Root.@TurnierName
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

    Private Sub Einstellungen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Einstellungen.Click
        Dim dialog As New Optionen
        dialog.ShowDialog()
        If dialog.LoadTriggered Then
            Open_Executed(sender, Nothing)
        End If
    End Sub

    Private Sub NächsteRunde_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NächsteRunde.Click
        If MessageBox.Show(Me, "Wollen Sie wirklich die nächste Runde starten? Sobald die nächste Runde beginnt, können die aktuellen Ergebnisse nicht mehr verändert werden.", _
                           "Nächste Runde?", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
            RundeBerechnen()
        End If
    End Sub

    Private Sub TurnierStarten_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        If MessageBox.Show(Me, "Wollen Sie wirklich das Turnier starten? Vergewissern Sie sich, dass alle Spielregeln und die Startreihenfolge richtig ist bevor Sie beginnen.", _
                           "Turnier Starten?", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
            RundeBerechnen()
            EditorArea.Navigate(New Begegnungen)
        End If

    End Sub

    Private Sub RundeBerechnen()
        Dim begegnungen = PaketBildung.organisierePakete(AktiveSpieler.ToList, SpielRunden.Count)

        Dim spielRunde As New SpielRunde
        
        For Each begegnung In begegnungen
            spielRunde.Add(begegnung)
        Next
        SpielRunden.Push(spielRunde)

        If My.Settings.AutoSaveAn Then
            Dim document = SpielRunden.ToXML
            document.Save(My.Settings.AutoSavePath)
        End If

    End Sub

    Private Sub EditorArea_Navigated(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.NavigationEventArgs) Handles EditorArea.Navigated
        Select Case e.Content.GetType
            Case GetType(Begegnungen)
                TurnierStarten.Visibility = Windows.Visibility.Collapsed
                Vorsortieren.Visibility = Windows.Visibility.Collapsed
                NächsteRunde.Visibility = Windows.Visibility.Visible
                Einstellungen.Visibility = Windows.Visibility.Collapsed
            Case GetType(StartListe)
                TurnierStarten.Visibility = Windows.Visibility.Visible
                Vorsortieren.Visibility = Windows.Visibility.Visible
                NächsteRunde.Visibility = Windows.Visibility.Collapsed
                Einstellungen.Visibility = Windows.Visibility.Visible
            Case Else
                Throw New Exception("Unbekannter Seitentyp")
        End Select
    End Sub
End Class
