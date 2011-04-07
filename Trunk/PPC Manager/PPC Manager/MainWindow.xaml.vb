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
            Dim spielerListe As SpielerListe = CType(FindResource("myPersonen"), SpielerListe)
            Dim SpielPartien As SpielPartien = CType(FindResource("mySpielPartien"), SpielPartien)

            Dim doc = New XDocument(<PPCTurnier>
                                        <SpielerListe>
                                            <%= From x In spielerListe Let y = x.ToXML Select y %>
                                        </SpielerListe>
                                        <AktuelleBegegnungen>
                                            <%= From x In SpielPartien Let y = x.ToXML Select y %>
                                        </AktuelleBegegnungen>
                                    </PPCTurnier>)

            doc.Save(dialog.FileName)
        End If
    End Sub

    Private Sub TurnierStarten_Executed(ByVal sender As System.Object, ByVal e As RoutedEventArgs) Handles TurnierStarten.Click
        Frame1.Navigate(New Uri("pack://application:,,,/Begegnungen.xaml"))
    End Sub

End Class
