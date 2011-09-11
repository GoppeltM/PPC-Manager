Imports System.Globalization

Class Begegnungen

    Friend Property BegegnungenView As CollectionViewSource

    Private Sub BegegnungenListView_Filter(ByVal sender As System.Object, ByVal e As System.Windows.Data.FilterEventArgs)

        If Not My.Settings.BegegnungenFiltern Then
            e.Accepted = True
            Return
        End If

        Dim partie As SpielPartie = CType(e.Item, SpielPartie)        
        e.Accepted = Not Abgeschlossen(partie)

    End Sub

    Friend Shared Function Abgeschlossen(ByVal partie As SpielPartie) As Boolean

        Dim SätzeLinks = Aggregate x In partie Where x.PunkteLinks = My.Settings.GewinnPunkte Into Count()
        Dim SätzeRechts = Aggregate x In partie Where x.PunkteRechts = My.Settings.GewinnPunkte Into Count()

        Dim gesamtAbgeschlossen = Math.Max(SätzeLinks, SätzeRechts)
        Dim gewinnSätze = My.Settings.GewinnSätze
        Return gesamtAbgeschlossen = gewinnSätze
    End Function

    Private Sub Begegnungen_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded        
        Dim SpielRunden = CType(FindResource("SpielRunden"), SpielRunden)
        BegegnungenView = CType(FindResource("PartieView"), CollectionViewSource)
        If SpielRunden.Count = 0 Then
            RundeBerechnen()        
        End If
        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
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

        If AbgeschlosseneSätze >= partie.Count AndAlso Not Abgeschlossen(partie) Then
            partie.Add(New Satz)
        End If
        BegegnungenView.View.Refresh()
        Dim SpielerView = CType(FindResource("SpielerView"), CollectionViewSource)
        SpielerView.View.Refresh()
    End Sub

    Private Sub Ausscheiden_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        If ListView1.SelectedIndex <> -1 Then
            Dim runde = CType(FindResource("SpielRunden"), SpielRunden).Peek
            Dim Spieler = CType(ListView1.SelectedItem, Spieler)
            If Not runde.AusgeschiedeneSpieler.Contains(Spieler) Then
                e.CanExecute = True
                Return
            End If
        End If
        e.CanExecute = False
    End Sub

    Private Sub Ausscheiden_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim runde = CType(FindResource("SpielRunden"), SpielRunden).Peek
        Dim Spieler = CType(ListView1.SelectedItem, Spieler)
        If MessageBox.Show(String.Format("Sind Sie sicher dass sie Spieler {0} ausscheiden lassen wollen? Dieser Vorgang kann nicht rückgängig gemacht werden!", Spieler.Nachname), _
                        "Spieler ausscheiden?", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
            Spieler.AusscheidenLassen()
        End If
    End Sub

    Private Sub BegegnungenFiltern_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        BegegnungenView.View.Refresh()
    End Sub

    Private Sub NächsteRunde_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        If MessageBox.Show("Wollen Sie wirklich die nächste Runde starten? Sobald die nächste Runde beginnt, können die aktuellen Ergebnisse nicht mehr verändert werden.", _
                   "Nächste Runde?", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then            
            RundeBerechnen()
        End If

    End Sub

    Private Sub RundeBerechnen()
        Dim SpielRunden = CType(FindResource("SpielRunden"), SpielRunden)
         Dim begegnungen = PaketBildung.organisierePakete(CType(FindResource("SpielerListe"), SpielerListe).ToList, _
                                                         SpielRunden.Count)


        Dim spielRunde As New SpielRunde
        
        For Each begegnung In begegnungen
            spielRunde.Add(begegnung)
        Next
        SpielRunden.Push(spielRunde)

        Dim ViewSource = CType(FindResource("SpielRundenView"), CollectionViewSource)
        Dim x = ViewSource.View.IsEmpty ' HACK: Diese Dummy Abfrage garantiert, 
        ' dass die View aktualisiert wird bevor die Position verschoben wird.
        ' Weiß die Hölle warum das so ist.
        ViewSource.View.MoveCurrentToFirst()
        RundenAnzeige.Content = "Runde " & SpielRunden.Count
        If CBool(My.Settings.AutoSaveAn) Then
            ApplicationCommands.Save.Execute(Nothing, Me)
        End If        
    End Sub

    Private Sub NächsteRunde_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        Dim SpielRunden = CType(FindResource("SpielRunden"), SpielRunden)
        If Not SpielRunden.Any Then Return
        Dim AktuellePartien = SpielRunden.Peek.ToList

        Dim AlleAbgeschlossen = Aggregate x In AktuellePartien Into All(Abgeschlossen(x))

        e.CanExecute = AlleAbgeschlossen
    End Sub

    Private Sub Window_Initialized(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Initialized
        Dim res = CType(FindResource("RanglisteDataProvider"), ObjectDataProvider)
        Dim liste = CType(FindResource("SpielerListe"), SpielerListe)
        res.ObjectInstance = liste
    End Sub
End Class

Class StringFormatter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
        Return String.Format(parameter.ToString, value)
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class

Public Class SatzFarbenPainter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert

        If Not targetType Is GetType(Brush) Then
            Throw New Exception("Must be a brush!")
        End If

        Dim x As Integer = CInt(value)
        If x >= My.Settings.GewinnPunkte Then
            Return Brushes.GreenYellow
        Else
            Return Brushes.Transparent
        End If

    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
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
            Return Brushes.Red
        Else
            Return Brushes.Transparent
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class

Public Class MeineGewonnenenSätze
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim points = CType(value, Integer)
        If points >= My.Settings.GewinnPunkte Then Return Visibility.Visible
        Return Visibility.Hidden
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class

Public Class RanglisteIndexConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim provider = CType(parameter, ObjectDataProvider)
        If provider Is Nothing Then Return Nothing
        If provider.ObjectInstance Is Nothing Then Return Nothing
        Dim myList = CType(provider.ObjectInstance, SpielerListe).ToList

        myList.Sort(Function(x, y) x.CompareTo(y))
        If myList IsNot Nothing Then
            Return myList.ToList.IndexOf(CType(value, Spieler)) + 1
        End If
        Return Nothing
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Return value
    End Function
End Class