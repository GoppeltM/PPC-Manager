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
        If TypeOf partie Is FreiLosSpiel Then
            e.Accepted = False
            Return
        End If
        e.Accepted = Not Abgeschlossen(partie)

    End Sub

    Friend Function Abgeschlossen(ByVal partie As SpielPartie) As Boolean

        Dim SätzeLinks = Aggregate x In partie Where x.PunkteLinks = My.Settings.GewinnPunkte Into Count()
        Dim SätzeRechts = Aggregate x In partie Where x.PunkteRechts = My.Settings.GewinnPunkte Into Count()

        Dim gesamtAbgeschlossen = Math.Max(SätzeLinks, SätzeRechts)
        Dim gewinnSätze = MainWindow.AktiveCompetition.Gewinnsätze
        Return gesamtAbgeschlossen >= gewinnSätze
    End Function

    Private Class SpielerComparer
        Implements IComparer

        Public Function Compare1(x As Object, y As Object) As Integer Implements IComparer.Compare
            Return DirectCast(x, Spieler).CompareTo(DirectCast(y, Spieler))
        End Function
    End Class

    Private Sub Begegnungen_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If MainWindow.AktiveCompetition Is Nothing Then Return
        Me.DataContext = MainWindow.AktiveCompetition
        BegegnungenView = CType(FindResource("PartieView"), CollectionViewSource)
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        Dim v = DirectCast(SpielerView.View, ListCollectionView)
        v.CustomSort = New SpielerComparer
        SpielerView.View.Refresh()
        ViewSource.View.Refresh()
        'Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist.
        ViewSource.View.MoveCurrentToFirst()
    End Sub

    Private Sub SatzLinks_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim x = CType(CType(sender, Button).DataContext, Satz)
        x.PunkteLinks = My.Settings.GewinnPunkte
        x.PunkteRechts = 0
        SatzUpdate()
    End Sub

    Private Sub SatzRechts_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim x = CType(CType(sender, Button).DataContext, Satz)
        x.PunkteRechts = My.Settings.GewinnPunkte
        x.PunkteLinks = 0
        SatzUpdate()
    End Sub

    Private Sub SatzUpdate()
        Dim partie = CType(DetailGrid.DataContext, SpielPartie)
        Dim AbgeschlosseneSätze = Aggregate x In partie Where Math.Max(x.PunkteLinks, x.PunkteRechts) = My.Settings.GewinnPunkte Into Count()

        Dim Fertig = Abgeschlossen(partie)

        If AbgeschlosseneSätze >= partie.Count AndAlso Not Fertig Then
            partie.Add(New Satz)
        End If
        partie.Update()
        ' Refresh erzwingt u.a. Filter
        If Fertig Then
            BegegnungenView.View.Refresh()
            Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
            SpielerView.View.Refresh()
        End If
        
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
        Dim AktuelleRunde = MainWindow.AktiveCompetition.SpielRunden.Peek()
        Dim AusgewählteSpieler = LifeListe.SelectedItems
        Dim dialog = New NeueSpielPartieDialog
        dialog.RundenNameTextBox.Text = "Runde " & MainWindow.AktiveCompetition.SpielRunden.Count
        If Not dialog.ShowDialog Then Return

        Dim neueSpielPartie = New SpielPartie(dialog.RundenNameTextBox.Text, CType(AusgewählteSpieler(0), Spieler), CType(AusgewählteSpieler(1), Spieler))
        neueSpielPartie.Add(New Satz)
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
