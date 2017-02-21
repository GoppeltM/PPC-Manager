Imports System.Windows.Controls.Primitives
Imports Microsoft.Win32

Class MainWindow

    Private ReadOnly Controller As IStartlistenController

    Public Sub New(controller As IStartlistenController, spielerListe As IEnumerable(Of Spieler),
                    klassementListe As IEnumerable(Of KlassementName))
        Resources("SpielerListe") = spielerListe
        Resources("KlassementListe") = klassementListe
        InitializeComponent()
        Me.Controller = controller
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        SpielerGrid.CommitEdit(DataGridEditingUnit.Row, True)
        Select Case MessageBox.Show("Dieses Programm wird jetzt geschlossen. Sollen Änderungen gespeichert werden?" _
                           , "Speichern und schließen?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
            Case MessageBoxResult.Cancel : e.Cancel = True
            Case MessageBoxResult.Yes : Controller.Speichern()
        End Select
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        With New OpenFileDialog
            .Filter = "Click-TT Turnierdaten|*.xml"
            If Not .ShowDialog Then
                Application.Current.Shutdown()
                Return
            End If

            Dim doc = XDocument.Load(.FileName)
            Dim pfad = .FileName
            Controller.Öffnend(doc, pfad)
        End With
    End Sub

    Private Sub CommandBinding_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = SpielerGrid.SelectedItem IsNot Nothing
    End Sub

    Private Sub CommandFremdSpieler_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = SpielerGrid.SelectedItem IsNot Nothing AndAlso DirectCast(SpielerGrid.SelectedItem, Spieler).Fremd
    End Sub

    Private Sub CommandNew_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim neuerTTR = DirectCast(SpielerGrid.SelectedItem, Spieler).TTR - 1
        SpielerGrid.BeginInit()
        Controller.NeuerFremdSpieler(neuerTTR)
        SpielerGrid.EndInit()
    End Sub

    Private Sub CommandDelete_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim aktuellerSpieler = DirectCast(SpielerGrid.SelectedItem, Spieler)
        If MessageBox.Show("Wollen Sie wirklich diesen Spieler entfernen?", "Löschen?", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) = MessageBoxResult.OK Then
            Controller.LöscheFremdSpieler(aktuellerSpieler)
        End If
    End Sub

    Private Sub CommandReplace_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim aktuellerSpieler = DirectCast(SpielerGrid.SelectedItem, Spieler)
        SpielerGrid.BeginInit()
        Controller.EditiereFremdSpieler(aktuellerSpieler)
        SpielerGrid.EndInit()
    End Sub

    Private Sub Save_Click(sender As Object, e As RoutedEventArgs)
        SpielerGrid.CommitEdit(DataGridEditingUnit.Row, True)
        Controller.Speichern()
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

        With DirectCast(e.Item, Spieler)


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
                Dim l As New List(Of Spieler)
                For Each item As Spieler In CollectionSource.View
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