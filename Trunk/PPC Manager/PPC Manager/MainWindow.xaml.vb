Imports System.Collections.ObjectModel

Class MainWindow

    Friend SpielRunden As SpielRunden
    Public ReadOnly Property SpielerListe As SpielerListe
        Get
            Return AktiveCompetition.SpielerListe
        End Get
    End Property
    Private DateiPfad As String
    Private SatzDifferenz As Boolean
    Private AutoSaveAn As Boolean
    Private TurnierName As String
    Private StartDatum As String
    Private EndDatum As String
    Private TurnierID As String
    Private AktiveCompetition As New Competition


    Public Function getXmlDocument() As XDocument

        Dim doc = New XDocument(New XDocumentType("tournament", Nothing, "http://www.datenautomaten.nu/dtd/nuLiga/TournamentPortal.dtd", Nothing),
                                <tournament end-date=<%= EndDatum %> start-date=<%= StartDatum %> name=<%= TurnierName %> tournament-id=<%= TurnierID %>>
                                    <%= AktiveCompetition.ToXML %>                                                                        
                                </tournament>)
        Return doc
    End Function

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        SpielRunden = CType(FindResource("SpielRunden"), SpielRunden)
        AktiveCompetition.SpielerListe = CType(FindResource("SpielerListe"), SpielerListe)
        With New LadenNeu
            Dim result = .ShowDialog
            If .Canceled Then
                Me.Close()
                Return
            End If            
            If result Then
                DateiPfad = .SpeicherPfad
                Einstellungen_Click(sender, e)
                Save_Executed(sender, Nothing)
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



    Friend Sub Open_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)        
        With LadenNeu.LadenDialog            
            If Not .ShowDialog(Me) Then Return

            My.Settings.LetztesVerzeichnis = IO.Path.GetDirectoryName(.FileName)

            Dim doc = XDocument.Load(.FileName)
            DateiPfad = .FileName
            SpielerListe.FromXML(doc.Root.<SpielerListe>)

            SpielRunden.FromXML(SpielerListe, doc.Root.<SpielRunden>.<SpielRunde>)
            My.Settings.GewinnSätze = Integer.Parse(doc.Root.@GewinnSätze)
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
                My.Settings.LetztesVerzeichnis = IO.Path.GetDirectoryName(.FileName)
                Save_Executed(sender, e)
            End If
        End With
        
    End Sub

    Private Sub Vorsortieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Vorsortieren.Click

        Dim sortierteListe = (From x In SpielerListe
                             Order By x.TTRating Descending).ToList

        SpielerListe.Clear()
        Dim current = 1
        For Each Spieler In sortierteListe
            Spieler.StartNummer = current
            SpielerListe.Add(Spieler)
            current += 1
        Next

    End Sub

    Private Sub Einstellungen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Einstellungen.Click

        With New Optionen
            If .ShowDialog Then

            End If
        End With        
    End Sub

    Private Sub TurnierStarten_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        If MessageBox.Show(Me, "Wollen Sie wirklich das Turnier starten? Vergewissern Sie sich, dass alle Spielregeln und die Startreihenfolge richtig ist bevor Sie beginnen.", _
                           "Turnier Starten?", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then

            Dim current = 1
            For Each row As Spieler In SpielerListe
                row.StartNummer = current
                current += 1
            Next            
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
                        Dim paginator As New UserControlPaginator(Of RanglisteSeite)(SpielerListe, _
                                                                                     New Size(.PrintableAreaWidth, .PrintableAreaHeight))
                        .PrintDocument(paginator, "Spieler Rangliste")
                    End If
                End With
            Case "Begegnungen"
                With New PrintDialog
                    If .ShowDialog Then
                        Dim paginator As New UserControlPaginator(Of SpielErgebnisZettel)(SpielRunden.Peek, _
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
            .FileName = IO.Path.ChangeExtension(DateiPfad, "xlsx")
            If .ShowDialog Then
                ExcelInterface.CreateFile(.FileName, SpielerListe, SpielRunden)
            End If

        End With

    End Sub
End Class
