Imports System.Globalization

Class Begegnungen

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Friend Property BegegnungenView As CollectionViewSource

    Private Sub BegegnungenListView_Filter(ByVal sender As System.Object, ByVal e As System.Windows.Data.FilterEventArgs)

        If Not My.Settings.BegegnungenFiltern Then
            e.Accepted = True
            Return
        End If

        Dim partie As SpielPartie = CType(e.Item, SpielPartie)
        e.Accepted = Not partie.Abgeschlossen

    End Sub

    Private Class SpielerComparer
        Implements IComparer

        Public Function Compare1(x As Object, y As Object) As Integer Implements IComparer.Compare
            Return DirectCast(y, Spieler).CompareTo(DirectCast(x, Spieler))
        End Function
    End Class

    Private Property AktiveCompetition As Competition

    Private Sub Begegnungen_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        AktiveCompetition = DirectCast(Me.DataContext, Competition)
        If AktiveCompetition Is Nothing Then Return
        BegegnungenView = CType(FindResource("PartieView"), CollectionViewSource)
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        Dim v = DirectCast(SpielerView.View, ListCollectionView)
        v.CustomSort = New SpielerComparer
        SpielerView.View.Refresh()
        ViewSource.View.Refresh()
        Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist.
        ViewSource.View.MoveCurrentToFirst()
        SetFocus()
        Begegnungsliste = SpielPartienListe.SpielPartienView
    End Sub

    Private Sub Ausscheiden_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        If LifeListe.SelectedIndex <> -1 Then
            Dim Spieler = CType(LifeListe.SelectedItem, Spieler)
            If Not Spieler.Ausgeschieden Then
                e.CanExecute = True
                Return
            End If
        End If
        e.CanExecute = False
    End Sub

    Private Sub Ausscheiden_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)        
        Dim Spieler = CType(LifeListe.SelectedItem, Spieler)
        If MessageBox.Show(String.Format("Sind Sie sicher dass sie Spieler {0} ausscheiden lassen wollen? Dieser Vorgang kann nicht rückgängig gemacht werden!", Spieler.Nachname), _
                        "Spieler ausscheiden?", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
            Spieler.AusscheidenLassen()
        End If
    End Sub

    Private Sub BegegnungenFiltern_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        If BegegnungenView.View Is Nothing Then Return
        BegegnungenView.View.Refresh()
    End Sub

  

    Private Sub Window_Initialized(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Initialized
        Dim res = CType(FindResource("RanglisteDataProvider"), ObjectDataProvider)
        Dim liste = CType(FindResource("SpielerListe"), SpielerListe)
        res.ObjectInstance = liste
    End Sub

    Private Sub NeuePartie_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = DirectCast(FindResource("PlayoffAktiv"), Boolean) AndAlso LifeListe.SelectedItems.Count = 2
    End Sub

    Private Sub NeuePartie_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim AktuelleRunde = AktiveCompetition.SpielRunden.Peek()
        Dim AusgewählteSpieler = LifeListe.SelectedItems
        Dim dialog = New NeueSpielPartieDialog
        dialog.RundenNameTextBox.Text = "Runde " & AktiveCompetition.SpielRunden.Count
        If Not dialog.ShowDialog Then Return

        Dim neueSpielPartie = New SpielPartie(dialog.RundenNameTextBox.Text, CType(AusgewählteSpieler(0), Spieler), CType(AusgewählteSpieler(1), Spieler), AktiveCompetition.SpielRegeln.Gewinnsätze)
        neueSpielPartie.ZeitStempel = Date.Now
        AktuelleRunde.Add(neueSpielPartie)
    End Sub


    Private Sub RefreshView()
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        'Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist
        ViewSource.View.Refresh()
        ViewSource.View.MoveCurrentToFirst()
        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        SpielerView.View.Refresh()
    End Sub

    Private Sub Refresh_Executed(sender As Object, e As ExecutedRoutedEventArgs)        
        If DirectCast(FindResource("PlayoffAktiv"), Boolean) Then
            LifeListe.SelectionMode = SelectionMode.Extended
        Else
            LifeListe.SelectionMode = SelectionMode.Single
        End If
        RefreshView()
    End Sub

    Private Sub CommandBinding_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Satzbearbeiten(True)
    End Sub

    Private Sub SetFocus()
        Punkte.Text = "0"
        Punkte.Focus()
        Punkte.SelectAll()
    End Sub

    Private Function OtherValue(value As Integer) As Integer
        Dim oValue = 11
        If value > 9 Then oValue = value + 2
        Return oValue
    End Function

    Private Sub Satzbearbeiten(inverted As Boolean)
        If Not Integer.TryParse(Punkte.Text, Nothing) Then
            Punkte.Text = "0"
            SetFocus()
            Return
        End If

        Dim value = Integer.Parse(Punkte.Text)
        Dim oValue = OtherValue(value)
        If inverted Then
            Dim temp = value
            value = oValue
            oValue = temp
        End If
        Dim s = New Satz With {.PunkteLinks = value, .PunkteRechts = oValue}
        DirectCast(DetailGrid.DataContext, SpielPartie).Add(s)

        SetFocus()
    End Sub

    Private Sub CommandBinding_Executed_1(sender As Object, e As ExecutedRoutedEventArgs)
        Satzbearbeiten(False)
    End Sub

    Private Sub CommandBinding_Executed_2(sender As Object, e As ExecutedRoutedEventArgs)
        DirectCast(DetailGrid.DataContext, SpielPartie).Remove(CType(Sätze.SelectedItem, Satz))
    End Sub

    Private Sub Punkte_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles Punkte.PreviewTextInput
        Dim i As Integer
        e.Handled = Not Integer.TryParse(e.Text, i)
    End Sub


    Private Sub CommandBinding_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)

        If DetailGrid.DataContext Is Nothing Then
            e.CanExecute = False
            Return
        End If

        Dim s = DirectCast(DetailGrid.DataContext, SpielPartie)

        Dim GewinnLinks = Aggregate x In s Where x.PunkteLinks > x.PunkteRechts Into Count()
        Dim GewinnRechts = Aggregate x In s Where x.PunkteLinks < x.PunkteRechts Into Count()

        e.CanExecute = Math.Max(GewinnLinks, GewinnRechts) < 3
    End Sub

    Private WithEvents Begegnungsliste As ListBox

    Private Sub NeuePartieAusgewählt(sender As Object, args As EventArgs) Handles Begegnungsliste.SelectionChanged, Begegnungsliste.MouseDown
        If BegegnungenView.View IsNot Nothing Then
            BegegnungenView.View.Refresh()
            Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
            SpielerView.View.Refresh()
        End If

        SetFocus()
    End Sub

    Private Sub SpielerView_Filter(sender As Object, e As FilterEventArgs)
        Dim s As Spieler = DirectCast(e.Item, Spieler)
        If AktiveCompetition Is Nothing Then
            e.Accepted = True
            Return
        End If
        Dim ausgeschiedeneSpieler = AktiveCompetition.SpielRunden.AusgeschiedeneSpieler

        Dim AusgeschiedenVorBeginn = Aggregate x In ausgeschiedeneSpieler Where x.Runde = 0 And
                            x.Spieler = s Into Any()

        e.Accepted = Not AusgeschiedenVorBeginn

    End Sub
End Class



Public Class DoppelbreitenGrid
    Inherits Grid

    'Protected Overrides Function MeasureOverride(constraint As Size) As Size
    '    MyBase.MeasureOverride(constraint)

    '    Return New Size(400, 60)
    'End Function

    'Protected Overrides Function ArrangeOverride(arrangeSize As Size) As Size
    '    Dim size = MyBase.ArrangeOverride(arrangeSize)

    '    Return New Size(400, 60)
    'End Function
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

Public Class SpielKlassenkonverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim Geburtsjahr = DirectCast(value, Integer)

        Select Case Date.Now.Year - Geburtsjahr
            Case Is <= 13 : Return "u13"
            Case Is <= 15 : Return "u15"
            Case Is <= 18 : Return "u18"
            Case Else : Return "Ü18"
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class


Public Class AusgeschiedenPainter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If Not targetType Is GetType(Brush) Then
            Throw New Exception("Must be a brush!")
        End If

        Dim val = CType(value, Boolean)
        If val Then
            Return Brushes.Bisque
        Else
            Return Brushes.Transparent
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class
Class HintergrundLinksKonverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        With DirectCast(value, Satz)
            If .PunkteLinks >= 11 AndAlso .PunkteLinks > .PunkteRechts Then
                Return Brushes.Yellow
            Else
                Return Brushes.Transparent
            End If
        End With
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class

Class HintergrundRechtsKonverter
    Implements IValueConverter


    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        With DirectCast(value, Satz)
            If .PunkteRechts >= 11 AndAlso .PunkteRechts > .PunkteLinks Then
                Return Brushes.Yellow
            Else
                Return Brushes.Transparent
            End If
        End With
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class