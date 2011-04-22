Imports System.Collections.ObjectModel

Class MainWindow

    Private Sub Öffnen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Öffnen.Click
        Dim dialog = New OpenFileDialog
        With dialog
            .Filter = "Excel Dateien|*.xlsx;*.xls"
            If .ShowDialog(Me) = True Then
                Dim name = .SafeFileName
            End If
        End With
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


        Dim dialog As New SaveFileDialog
        dialog.Filter = "XML Dateien |*.xml"

        If dialog.ShowDialog() Then
            Dim aktiveSpieler = CType(FindResource("AktiveSpieler"), SpielerListe)
            Dim ausgeschiedeneSpieler = CType(FindResource("AusgeschiedeneSpieler"), SpielerListe)
            Dim SpielRunden = CType(FindResource("SpielRunden"), SpielRunden)

            Dim doc = New XDocument(<PPCTurnier>
                                        <AktiveSpieler>
                                            <%= From x In aktiveSpieler Let y = x.ToXML Select y %>
                                        </AktiveSpieler>
                                        <SpielRunden>
                                            <%= SpielRunden.ToXML %>
                                        </SpielRunden>
                                    </PPCTurnier>)

            doc.Save(dialog.FileName)
        End If
    End Sub

    Private Sub TurnierStarten_Executed(ByVal sender As System.Object, ByVal e As RoutedEventArgs) Handles TurnierStarten.Click
        Frame1.Navigate(New Uri("pack://application:,,,/Begegnungen.xaml"))
    End Sub

    Private Sub Vorsortieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Vorsortieren.Click
        Dim spielerListe As SpielerListe = CType(FindResource("myPersonen"), SpielerListe)


        Dim sortierteListe = From x In spielerListe
                             Let spielDiff = My.Settings.SpielKlassen.IndexOf(x.SpielKlasse) * 2 + x.Position
                             Order By x.Rating Descending, x.Rang Ascending, x.TurnierKlasse Ascending, spielDiff Descending Select x

        spielerListe.Clear()
        For Each Spieler In sortierteListe
            spielerListe.Add(Spieler)
        Next

    End Sub
End Class
