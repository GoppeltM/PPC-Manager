Imports System.Globalization
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Class Begegnungen


    Public Property BegegnungenFiltern As Boolean = False

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        Dim app = CType(Application.Current.MainWindow, MainWindow)
    End Sub

    Private Sub BegegnungenListView_Filter(ByVal sender As System.Object, ByVal e As System.Windows.Data.FilterEventArgs)
        If Not BegegnungenFiltern Then
            e.Accepted = True
            Return
        End If

        Dim partie As SpielPartie = CType(e.Item, SpielPartie)
        e.Accepted = Not partie.IstAbgeschlossen
    End Sub

    Private WithEvents Begegnungsliste As ListBox

    Friend Sub Update()
        Dim c = CType(FindResource("PartieView"), CollectionViewSource)
        c.View.Refresh()
    End Sub

    Private Sub PartieAusgewählt_Execute(sender As Object, e As ExecutedRoutedEventArgs)
        DetailGrid.DataContext = e.Parameter
        DetailGrid.SetFocus()
    End Sub

    Private Sub SpielRegelnUI_DataContextChanged(sender As Object, e As DependencyPropertyChangedEventArgs)
        Dim listView = CType(Regeln.DataContext, ListCollectionView)
        If listView Is Nothing Then Return

        Dim partie = CType(listView.CurrentItem, SpielPartie)
        If partie Is Nothing Then
            Regeln.Visibility = Visibility.Visible
            Return
        End If

        Regeln.Visibility = If(partie.RundenName Is "Runde 0", Visibility.Visible, Visibility.Hidden)
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim App = CType(Application.Current, Application)

        Dim mainWindow = CType(App.MainWindow, MainWindow)
        mainWindow._Controller.SaveExcel()
        mainWindow.NächsteRunde()
        mainWindow._Controller.SaveXML()
        mainWindow.SkipDialog = True

        App.LadeCompetition(sender, App.competition)
    End Sub
End Class
