Imports System.Collections.ObjectModel
Imports <xmlns="http://PPCManager/SpeicherStand">

Class MainWindow

    Friend SpielRunden As SpielRunden
    Friend SpielerListe As SpielerListe
    Private DateiPfad As String
    Private Gewinnsätze As Integer
    Private SatzDifferenz As Boolean
    Private AutoSaveAn As Boolean

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        SpielRunden = CType(FindResource("SpielRunden"), SpielRunden)
        SpielerListe = CType(FindResource("SpielerListe"), SpielerListe)

        With New LadenNeu
            Dim result = .ShowDialog
            If .Canceled Then
                Me.Close()
                Return
            End If            
            If result Then
                DateiPfad = .SpeicherPfad
                Save_Executed(sender, Nothing)
                Einstellungen_Click(sender, e)
            Else
                Open_Executed(sender, Nothing)
                If String.IsNullOrEmpty(DateiPfad) Then
                    Me.Close()
                End If
            End If
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
        Dim doc = getXmlDocument()

        doc.Save(DateiPfad)
    End Sub

    Private Function getXmlDocument() As XDocument

        Dim doc = New XDocument(<PPCTurnier GewinnSätze=<%= Gewinnsätze %> SatzDifferenz=<%= SatzDifferenz %>>
                                    <SpielerListe>
                                        <%= From x In SpielerListe Let y = x.ToXML Select y %>
                                    </SpielerListe>

                                    <%= SpielRunden.ToXML %>
                                </PPCTurnier>)
        Return doc
    End Function

    Friend Sub Open_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)        
        With LadenNeu.LadenDialog            
            If Not .ShowDialog(Me) Then Return

            Dim doc = XDocument.Load(.FileName)
            DateiPfad = .FileName
            SpielerListe.FromXML(SpielRunden, doc.Root.<SpielerListe>)

            SpielRunden.FromXML(SpielerListe, doc.Root.<SpielRunden>.<SpielRunde>)

            Gewinnsätze = Integer.Parse(doc.Root.@GewinnSätze)
            SatzDifferenz = Boolean.Parse(doc.Root.@SatzDifferenz)
            If SpielRunden.Any Then
                EditorArea.Navigate(New Begegnungen)
            Else
                EditorArea.Navigate(New StartListe)
            End If

        End With
    End Sub

    Private Sub SaveAs_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)

        With LadenNeu.SpeichernDialog
            If .ShowDialog() Then
                DateiPfad = .FileName
                Save_Executed(sender, e)
            End If
        End With
        
    End Sub

    Private Sub Vorsortieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Vorsortieren.Click

        Dim sortierteListe = (From x In SpielerListe
                             Let spielDiff = My.Settings.SpielKlassen.IndexOf(x.SpielKlasse) * 2 + x.Position
                             Order By x.Rating Descending, x.Rang Ascending, x.TurnierKlasse Ascending, spielDiff Descending Select x).ToList

        SpielerListe.Clear()
        For Each Spieler In sortierteListe
            SpielerListe.Add(Spieler)
        Next

    End Sub

    Private Sub Einstellungen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Einstellungen.Click

        With New Optionen
            If .ShowDialog Then

            End If
        End With
        
    End Sub

    Private Sub NächsteRunde_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NächsteRunde.Click
        If MessageBox.Show(Me, "Wollen Sie wirklich die nächste Runde starten? Sobald die nächste Runde beginnt, können die aktuellen Ergebnisse nicht mehr verändert werden.", _
                           "Nächste Runde?", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
            Dim BegegnungenView = CType(FindResource("PartieView"), CollectionViewSource)
            BegegnungenView.View.MoveCurrentToLast()
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
        Dim begegnungen = PaketBildung.organisierePakete(SpielerListe.ToList, SpielRunden.Count)

        Dim spielRunde As New SpielRunde

        For Each begegnung In begegnungen
            spielRunde.Add(begegnung)
        Next
        SpielRunden.Push(spielRunde)

        If CBool(FindResource("")) Then
            Dim document = SpielRunden.ToXML
            document.Save(DateiPfad)
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
End Class
