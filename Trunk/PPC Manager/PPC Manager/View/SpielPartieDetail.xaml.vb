Imports PPC_Manager

Public Class SpielPartieDetail

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub
    Public Sub SetFocus()
        Punkte.Text = "0"
        Punkte.Focus()
        Punkte.SelectAll()
    End Sub

    Private Sub SpielPartieDetail_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

    End Sub

    Private Sub Satzbearbeiten(inverted As Boolean)
        If Not Integer.TryParse(Punkte.Text, Nothing) Then
            Punkte.Text = "0"
            SetFocus()
            Return
        End If

        Dim value = Integer.Parse(Punkte.Text)
        Dim partie = DirectCast(DataContext, SpielPartie)

        SatzEintragen(value, inverted, partie)

        NavigationCommands.Refresh.Execute(Nothing, CType(Application.Current.MainWindow, MainWindow).LiveListe)

        SetFocus()
    End Sub

    Public Sub SatzEintragen(value As Integer, linksGewonnen As Boolean, partie As SpielPartie)
        Dim oValue = OtherValue(value)
        If linksGewonnen Then
            Dim temp = value
            value = oValue
            oValue = temp
        End If
        Dim s = New Satz With {.PunkteLinks = value, .PunkteRechts = oValue}
        partie.Add(s)

        Dim finaltabelle = TryCast(Application.Current.MainWindow.FindName("PlayoffUI"), Finaltabelle)
        If finaltabelle IsNot Nothing Then finaltabelle.Update()
    End Sub

    Private Function OtherValue(value As Integer) As Integer
        Dim oValue = 11
        If value > 9 Then oValue = value + 2
        Return oValue
    End Function

    Private Sub NeuerSatz_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)

        Dim s = TryCast(DataContext, SpielPartie)
        If s Is Nothing Then Return
        e.CanExecute = Not s.IstAbgeschlossen
    End Sub

    Private Sub CommandBinding_Executed_1(sender As Object, e As ExecutedRoutedEventArgs)
        Satzbearbeiten(False)
    End Sub

    Private Sub CommandBinding_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Satzbearbeiten(True)
    End Sub

    Private Sub CommandBinding_Executed_2(sender As Object, e As ExecutedRoutedEventArgs)
        DirectCast(DataContext, SpielPartie).Remove(CType(Sätze.SelectedItem, Satz))
        NavigationCommands.Refresh.Execute(Nothing, CType(Application.Current.MainWindow, MainWindow).LiveListe)
    End Sub

    Private Sub Punkte_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles Punkte.PreviewTextInput
        Dim i As Integer
        e.Handled = Not Integer.TryParse(e.Text, i)
    End Sub

End Class
