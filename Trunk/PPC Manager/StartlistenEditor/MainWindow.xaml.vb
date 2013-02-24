Imports Microsoft.Win32

Class MainWindow

    Private Doc As XDocument

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        With New OpenFileDialog
            .Filter = "Click-TT Turnierdaten|*.xml"
            If Not .ShowDialog Then
                Application.Current.Shutdown()
                Return
            End If

            Doc = XDocument.Load(.FileName)
        End With

        Dim SpielerListe = From competition In Doc.Root.<competition>
                           From Spieler In competition...<player>
                           Group Spieler, competition By Spieler.<person>.First.Attribute("internal-nr").Value Into Group
                           Select Group

        Dim AlleSpieler As New List(Of Spieler)

        For Each s In SpielerListe

            Dim SpielerKnoten = From x In s Select x.Spieler

            Dim Competitions = From x In s Select x.competition.Attribute("age-group").Value
            AlleSpieler.Add(Spieler.FromXML(SpielerKnoten, Competitions))
        Next

        AlleSpieler.Sort()
        Dim res = DirectCast(FindResource("SpielerListe"), SpielerListe)
        For Each s In AlleSpieler
            res.Add(s)
        Next


    End Sub

    Private Sub CommandBinding_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = SpielerGrid.SelectedItem IsNot Nothing
    End Sub

    Private Sub CommandFremdSpieler_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = SpielerGrid.SelectedItem IsNot Nothing AndAlso DirectCast(SpielerGrid.SelectedItem, Spieler).Fremd
    End Sub

    Private Sub CommandNew_Executed(sender As Object, e As ExecutedRoutedEventArgs)

    End Sub

    Private Sub CommandDelete_Executed(sender As Object, e As ExecutedRoutedEventArgs)

    End Sub

    Private Sub CommandReplace_Executed(sender As Object, e As ExecutedRoutedEventArgs)

    End Sub
End Class
