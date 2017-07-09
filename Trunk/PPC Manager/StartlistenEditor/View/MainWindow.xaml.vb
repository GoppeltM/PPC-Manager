Imports System.Windows.Controls.Primitives

Public Class MainWindowContext
    Public Property Spielerliste As ICollection(Of SpielerInfo)
    Public Property KlassementListe As IEnumerable(Of KlassementName)
End Class

Public Class MeineCommands

    Public Shared ReadOnly Property FremdSpielerLöschen As RoutedUICommand = New RoutedUICommand("Löscht ausschließlich Fremdspieler",
                                                                                     "Löschen",
                                                                                     GetType(MeineCommands))

End Class

Class MainWindow

    Private ReadOnly _KlassementListe As IEnumerable(Of String)
    Private ReadOnly _SpeichernAction As Action
    Private ReadOnly _SpielerListe As ICollection(Of SpielerInfo)

    Public Sub New(spielerListe As ICollection(Of SpielerInfo),
                    klassementListe As IEnumerable(Of String), speichernAction As Action)
        _SpielerListe = spielerListe
        _KlassementListe = klassementListe
        _SpeichernAction = speichernAction
        Dim klassements = (From x In klassementListe Select New KlassementName With {.Name = x}).ToList
        DataContext = New MainWindowContext With {
            .KlassementListe = klassements,
            .Spielerliste = spielerListe
        }

        InitializeComponent()
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        SpielerGrid.CommitEdit(DataGridEditingUnit.Row, True)
        Select Case MessageBox.Show("Dieses Programm wird jetzt geschlossen. Sollen Änderungen gespeichert werden?" _
                           , "Speichern und schließen?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
            Case MessageBoxResult.Cancel : e.Cancel = True
            Case MessageBoxResult.Yes : _SpeichernAction()
        End Select
    End Sub

    Private Sub CommandBinding_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub CommandFremdSpieler_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = SpielerGrid?.SelectedItem IsNot Nothing AndAlso DirectCast(SpielerGrid.SelectedItem, SpielerInfo).Fremd
    End Sub

    Private Sub CommandNew_Executed(sender As Object, e As ExecutedRoutedEventArgs)

        Dim neuerTTR As Integer = 0
        If SpielerGrid.SelectedItem IsNot Nothing Then
            neuerTTR = DirectCast(SpielerGrid.SelectedItem, SpielerInfo).TTR - 1
        End If

        Dim dialog As New FremdSpielerDialog(_KlassementListe, New SpielerInfo With {.TTR = neuerTTR})
        If dialog.ShowDialog() Then
            SpielerGrid.BeginInit()
            _SpielerListe.Add(dialog.EditierterSpieler)
            SpielerGrid.EndInit()
        End If
    End Sub

    Private Sub CommandDelete_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim aktuellerSpieler = DirectCast(SpielerGrid.SelectedItem, SpielerInfo)
        If MessageBox.Show("Wollen Sie wirklich diesen Spieler entfernen?", "Löschen?", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) = MessageBoxResult.OK Then
            _SpielerListe.Remove(aktuellerSpieler)
        End If
    End Sub

    Private Sub CommandReplace_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim aktuellerSpieler = DirectCast(SpielerGrid.SelectedItem, SpielerInfo)
        Dim dialog As New FremdSpielerDialog(_KlassementListe, New SpielerInfo(aktuellerSpieler))
        If dialog.ShowDialog() Then
            SpielerGrid.BeginInit()
            _SpielerListe.Remove(aktuellerSpieler)
            _SpielerListe.Add(dialog.EditierterSpieler)
            SpielerGrid.EndInit()
        End If

    End Sub

    Private Sub Save_Click(sender As Object, e As RoutedEventArgs)
        SpielerGrid.CommitEdit(DataGridEditingUnit.Row, True)
        _SpeichernAction()
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

    Public Sub DataGridCell_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
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
        Dim KlassementButtons = FindVisualChildren(Of ToggleButton)(KlassementFilterListe)
        Dim KlassementFilterAktiv = Aggregate x In KlassementButtons Where x.IsChecked Into Any()

        With DirectCast(e.Item, SpielerInfo)


            If Unbezahlt.IsChecked AndAlso .Bezahlt Then
                e.Accepted = False
                Return
            End If
            If NichtAnwesend.IsChecked AndAlso .Anwesend Then
                e.Accepted = False
                Return
            End If

            If KlassementFilterAktiv Then
                e.Accepted = False
                For Each tb In FindVisualChildren(Of ToggleButton)(KlassementFilterListe)
                    If tb.IsChecked AndAlso tb.Content.ToString = .Klassement Then
                        e.Accepted = True
                    End If
                Next
                If Not e.Accepted Then Return
            End If


            If Not e.Accepted Then Return

            e.Accepted = True
            If String.IsNullOrEmpty(Suche.Text) Then Return

            Dim SuchText = Suche.Text.ToLower

            If .Vorname.ToLower.Contains(SuchText) Then Return
            If .Nachname.ToLower.Contains(SuchText) Then Return
            If .Verein.ToLower.Contains(SuchText) Then Return
            If .Klassement.ToLower.Contains(SuchText) Then Return
        End With
        e.Accepted = False
    End Sub

    Shared Function FindVisualChildren(Of T As DependencyObject)(parent As DependencyObject) As IEnumerable(Of T)
        Dim Children As New List(Of T)
        If TypeOf parent Is T Then
            Children.Add(DirectCast(parent, T))
            Return Children
        End If

        For i = 0 To VisualTreeHelper.GetChildrenCount(parent) - 1
            Dim child = VisualTreeHelper.GetChild(parent, i)
            Children.AddRange(FindVisualChildren(Of T)(child))
        Next
        Return Children
    End Function

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim View = DirectCast(FindResource("FilteredSpielerListe"), CollectionViewSource)
        For Each column In SpielerGrid.Columns
            column.SortDirection = Nothing
        Next
        SpielerGrid.CommitEdit(DataGridEditingUnit.Row, True)
        With View.SortDescriptions
            .Clear()
            .Add(New System.ComponentModel.SortDescription With {.PropertyName = "TTR", .Direction = ComponentModel.ListSortDirection.Descending})
            .Add(New System.ComponentModel.SortDescription With {.PropertyName = "TTRMatchCount", .Direction = ComponentModel.ListSortDirection.Descending})
        End With

    End Sub

    Private Sub UpdateFilter(sender As Object, e As RoutedEventArgs)
        Dim View = DirectCast(FindResource("FilteredSpielerListe"), CollectionViewSource)
        SpielerGrid.CommitEdit(DataGridEditingUnit.Row, True)
        View.View.Refresh()
    End Sub

    Private Sub Drucken_Click(sender As Object, e As RoutedEventArgs)
        With New PrintDialog
            If .ShowDialog Then
                Dim CollectionSource = DirectCast(FindResource("FilteredSpielerListe"), CollectionViewSource)
                Dim l As New List(Of SpielerInfo)
                For Each item As SpielerInfo In CollectionSource.View
                    l.Add(item)
                Next

                Dim doc As New FixedDocument

                For Each page In New UserControlPaginator(l, New Size(.PrintableAreaWidth, .PrintableAreaHeight)).ErzeugePageContent
                    doc.Pages.Add(page)
                Next
                .PrintDocument(doc.DocumentPaginator, "Spielerliste - Aktuelle Sicht")
            End If
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
        Throw New NotImplementedException
    End Function
End Class

Public Class KlassementListe
    Inherits ObjectModel.ObservableCollection(Of KlassementName)
End Class

Public Class KlassementName
    Property Name As String
End Class

Public Class GeschlechtKonverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim geschlecht = DirectCast(value, Integer)
        Select Case geschlecht
            Case 0 : Return "w"
            Case 1 : Return "m"
        End Select
        Throw New ArgumentException("Unbekanntes geschlecht: " & geschlecht)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class