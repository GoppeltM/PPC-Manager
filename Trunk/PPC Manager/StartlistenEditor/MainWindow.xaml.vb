Imports Microsoft.Win32
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de/">

Class MainWindow

    Public Shared Doc As XDocument
    Public Pfad As String

    Private ReadOnly Property SpielerListe As SpielerListe
        Get
            Dim res = DirectCast(FindResource("SpielerListe"), SpielerListe)
            Return res
        End Get
    End Property

    Private Sub MainWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        Select Case MessageBox.Show("Sollen Änderungen gespeichert und dieses Programm geschlossen werden?" _
                           , "Speichern und schließen?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
            Case MessageBoxResult.Cancel : e.Cancel = True
            Case MessageBoxResult.Yes : If Pfad IsNot Nothing Then Speichern()
        End Select

    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        With New OpenFileDialog
            .Filter = "Click-TT Turnierdaten|*.xml"
            If Not .ShowDialog Then
                Application.Current.Shutdown()
                Return
            End If

            Doc = XDocument.Load(.FileName)
            Pfad = .FileName
        End With

        Dim AlleSpieler = XmlZuSpielerListe(Doc)

        For Each s In AlleSpieler
            SpielerListe.Add(s)
        Next

    End Sub

    Shared Function XmlZuSpielerListe(doc As XDocument) As IList(Of Spieler)

        Dim SpielerListe = From competition In doc.Root.<competition>
                           From Spieler In competition...<player>.Concat(competition...<ppc:player>)
                           Group Spieler, competition By Spieler.<person>.First.Attribute("licence-nr").Value Into Group
                           Select Group

        Dim AlleSpieler As New List(Of Spieler)

        For Each s In SpielerListe

            Dim SpielerKnoten = From x In s Select x.Spieler

            AlleSpieler.Add(Spieler.FromXML(SpielerKnoten))
        Next

        AlleSpieler.Sort()
        Return AlleSpieler
    End Function

    Private Sub CommandBinding_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = SpielerGrid.SelectedItem IsNot Nothing
    End Sub

    Private Sub CommandFremdSpieler_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = SpielerGrid.SelectedItem IsNot Nothing AndAlso DirectCast(SpielerGrid.SelectedItem, Spieler).Fremd
    End Sub

    Private Sub CommandNew_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim neuerTTR = DirectCast(SpielerGrid.SelectedItem, Spieler).TTR - 1
        Dim Lizenznummern = (From x In SpielerListe Select x.LizenzNr).ToList
        Dim NeueLizenzNummer = -1
        While Lizenznummern.Contains(NeueLizenzNummer)
            NeueLizenzNummer -= 1
        End While
        Dim dialog = FremdSpielerDialog.NeuerFremdSpieler(neuerTTR, NeueLizenzNummer)
        If dialog.ShowDialog() Then
            SpielerGrid.BeginInit()
            Dim res = DirectCast(FindResource("SpielerListe"), SpielerListe)
            res.Add(dialog.Spieler)
            SpielerGrid.EndInit()
        End If
    End Sub

    Private Sub CommandDelete_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim aktuellerSpieler = DirectCast(SpielerGrid.SelectedItem, Spieler)
        If MessageBox.Show("Wollen Sie wirklich diesen Spieler aus allen Klassements entfernen?", "Löschen?", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) = MessageBoxResult.OK Then
            Dim res = DirectCast(FindResource("SpielerListe"), SpielerListe)
            res.Remove(aktuellerSpieler)
        End If
    End Sub

    Private Sub CommandReplace_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim aktuellerSpieler = DirectCast(SpielerGrid.SelectedItem, Spieler)        
        Dim dialog = FremdSpielerDialog.EditiereFremdSpieler(aktuellerSpieler)
        aktuellerSpieler.BeginEdit()
        SpielerGrid.BeginInit()
        If Not dialog.ShowDialog() Then
            aktuellerSpieler.CancelEdit()
        End If
        SpielerGrid.EndInit()
    End Sub

    Private Sub Speichern()
        Doc.Save(Pfad)
    End Sub

End Class
