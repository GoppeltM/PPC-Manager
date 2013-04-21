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
            aktuellerSpieler.XmlKnoten.Remove()            
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


    Shared Function FindVisualParent(Of T As UIElement)(element As UIElement) As T
        Dim parent = element
        While parent IsNot Nothing
            Dim correctlyTyped = TryCast(parent, T)
            If correctlyTyped IsNot Nothing Then
                Return correctlyTyped
            End If                
            parent = TryCast(VisualTreeHelper.GetParent(parent), UIElement)
        End While
            
        Return Nothing
    End Function
        

    ' Click-Through Verhalten adaptiert von
    ' http://wpf.codeplex.com/wikipage?title=Single-Click%20Editing&referringTitle=Tips%20%26%20Tricks&ProjectName=wpf

    Private Sub DataGridCell_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        Dim cell = DirectCast(sender, DataGridCell)
        If cell Is Nothing OrElse cell.IsEditing OrElse cell.IsReadOnly Then Return

        If Not cell.IsFocused Then
            cell.Focus()
        End If
        Dim DataGrid = FindVisualParent(Of DataGrid)(cell)
        If DataGrid Is Nothing Then Return
        If DataGrid.SelectionUnit <> DataGridSelectionUnit.FullRow Then
            If Not cell.IsSelected Then
                cell.IsSelected = True
            End If
        Else
            Dim row = FindVisualParent(Of DataGridRow)(cell)
            If (row IsNot Nothing AndAlso Not row.IsSelected) Then
                row.IsSelected = True
            End If
        End If
    End Sub

    Private Sub SuchFilter(sender As Object, e As FilterEventArgs)
        e.Accepted = True
        If Suche Is Nothing Then Return
        If String.IsNullOrEmpty(Suche.Text) Then Return

        Dim SuchText = Suche.Text.ToLower
        With DirectCast(e.Item, Spieler)
            If .Vorname.ToLower.Contains(SuchText) Then Return
            If .Nachname.ToLower.Contains(SuchText) Then Return
            If .Verein.ToLower.Contains(SuchText) Then Return
            For Each klassement In .Klassements
                If klassement.ToLower.Contains(SuchText) Then Return
            Next
        End With
        e.Accepted = False
    End Sub

    Private Sub Suche_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Suche.TextChanged        
        Dim View = DirectCast(FindResource("FilteredSpielerListe"), CollectionViewSource)
        View.View.Refresh()
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim View = DirectCast(FindResource("FilteredSpielerListe"), CollectionViewSource)
        For Each column In SpielerGrid.Columns
            column.SortDirection = Nothing
        Next
        With View.SortDescriptions
            .Clear()
            .Add(New System.ComponentModel.SortDescription With {.PropertyName = "TTR", .Direction = ComponentModel.ListSortDirection.Descending})
            .Add(New System.ComponentModel.SortDescription With {.PropertyName = "TTRMatchCount", .Direction = ComponentModel.ListSortDirection.Descending})
        End With
        
    End Sub
End Class

Class StringIsEmptyConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If String.IsNullOrEmpty(DirectCast(value, String)) Then
            Return Visibility.Visible
        Else
            Return Visibility.Hidden
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack

    End Function
End Class
