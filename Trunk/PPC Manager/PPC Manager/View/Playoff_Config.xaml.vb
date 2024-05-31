
Imports System.Collections.ObjectModel

Public Class MainWindowContext
    Public Property Spielerliste As ICollection(Of SpielerInfoTurnier)
    Public Property KlassementListe As IEnumerable(Of String)
End Class

Public Class SpielerInfoTurnier
    Inherits SpielerInfo
    Public Property Klassement As String
    Public Property Punkte As Integer = 0
    Public Property Platz As Integer = 99
    Public Property Ausgeschieden As Boolean = False
    Public Property CanRejoin As Boolean = False

    Sub New(spieler As SpielerInfo, comp As String)
        MyBase.New(spieler)

        Klassement = comp
    End Sub
End Class

Public Enum FinalMode
    Viertelfinale
    Halbfinale
    Finalrunde
    Sieger
End Enum


Public Class Playoff_Config

    Public Property Doc As XDocument = Nothing

    Public Property mode As Integer = -1

    Public Sub New()
        UpdateContext(Nothing)

        InitializeComponent()
    End Sub

    Public Sub UpdateContext(doc As XDocument)
        Me.Doc = doc

        If Me.Doc Is Nothing Then
            DataContext = New MainWindowContext With {
                .Spielerliste = New List(Of SpielerInfoTurnier),
                .KlassementListe = New List(Of String)
            }
            Return
        End If

        DataContext = New MainWindowContext With {
            .KlassementListe = New Collection(Of String)(doc.Root.<competition>.Where(Function(x) x.<players>.<player>.Any).Select(Function(x) x.Attribute("age-group").Value).ToList),
            .Spielerliste = LeseSpieler()
        }

        UpdateFilteredList()

    End Sub

    Public Function LeseSpieler() As IList(Of SpielerInfoTurnier)
        Dim allespieler As New List(Of SpielerInfoTurnier)
        Dim klassements = Doc.Root.<competition>.OrderBy(Function(x) x.Attribute("ttr-from").Value).Reverse

        For Each klassement In klassements
            Dim comp = klassement.Attribute("age-group").Value

            allespieler.AddRange(SpielerNachRangliste(comp))
        Next
        Return allespieler
    End Function

    Public Sub RetireSelectedPlayers()
        Dim selected = linkeListe.SelectedItems.Cast(Of SpielerInfoTurnier).ToList
        If selected.Count = 0 Then Return

        For Each s In selected
            Dim comp = s.Klassement
            Dim Regeln = SpielRegeln.Parse(Doc, comp)
            Dim spielRunden = New SpielRunden
            Dim spielpartien = spielRunden.SelectMany(Function(m) m)
            Dim ausgeschiedeneIds = spielRunden.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs)
            Dim spielstand = New Spielstand(Regeln.Gewinnsätze)
            Dim spielverlauf = New Spielverlauf(spielpartien, ausgeschiedeneIds, spielstand)

            'befüllt spielRunden
            Dim AktiveCompetition = AusXML.CompetitionFromXML("", Doc, comp, Regeln, spielRunden)

            spielRunden.Peek.AusgeschiedeneSpielerIDs.Add(s.Id)

            ZuXML.SaveXML(CType(Application.Current, Application).xmlPfad, Regeln, comp, spielRunden)
        Next

        Doc = XDocument.Load(CType(Application.Current, Application).xmlPfad)
        UpdateFilteredList()
    End Sub

    Public Sub UndoRetireSelectedPlayers()
        Dim selected = linkeListe.SelectedItems.Cast(Of SpielerInfoTurnier).ToList
        If selected.Count = 0 Then Return

        For Each s In selected
            If Not s.Ausgeschieden Then Continue For

            Dim comp = s.Klassement
            Dim Regeln = SpielRegeln.Parse(Doc, comp)
            Dim spielRunden = New SpielRunden
            Dim spielpartien = spielRunden.SelectMany(Function(m) m)
            Dim ausgeschiedeneIds = spielRunden.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs)
            Dim spielstand = New Spielstand(Regeln.Gewinnsätze)
            Dim spielverlauf = New Spielverlauf(spielpartien, ausgeschiedeneIds, spielstand)

            'befüllt spielRunden
            Dim AktiveCompetition = AusXML.CompetitionFromXML("", Doc, comp, Regeln, spielRunden)

            spielRunden.Peek.AusgeschiedeneSpielerIDs.Remove(s.Id)

            ZuXML.SaveXML(CType(Application.Current, Application).xmlPfad, Regeln, comp, spielRunden)
        Next

        Doc = XDocument.Load(CType(Application.Current, Application).xmlPfad)
        UpdateFilteredList()
    End Sub

    Private Function SpielerNachRangliste(comp As String) As IList(Of SpielerInfoTurnier)
        Dim Regeln = SpielRegeln.Parse(Doc, comp)
        Dim spielRunden = New SpielRunden
        Dim spielpartien = spielRunden.SelectMany(Function(m) m)
        Dim ausgeschiedeneIds = spielRunden.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs)
        Dim spielstand = New Spielstand(Regeln.Gewinnsätze)
        Dim spielverlauf = New Spielverlauf(spielpartien, ausgeschiedeneIds, spielstand)

        'befüllt spielRunden
        Dim AktiveCompetition = AusXML.CompetitionFromXML("", Doc, comp, Regeln, spielRunden)
        Dim vergleicher = New SpielerInfoComparer(spielverlauf, Regeln.SatzDifferenz, Regeln.SonneBornBerger)

        Dim Spieler = New List(Of SpielerInfoTurnier)
        For Each x In AktiveCompetition.SpielerListe
            Spieler.Add(New SpielerInfoTurnier(x, comp) With {
                .Ausgeschieden = spielRunden.Any(Function(m) m.AusgeschiedeneSpielerIDs.Contains(x.Id)),
                .CanRejoin = spielRunden.Peek.AusgeschiedeneSpielerIDs.Contains(x.Id),
                .Punkte = spielverlauf.BerechnePunkte(x)
            })
        Next

        Spieler.Sort(vergleicher)
        Spieler.Reverse()

        Dim platz = 1
        For Each s In Spieler
            s.Platz = platz
            platz += 1
        Next

        Return Spieler
    End Function

    Private Sub ListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        UpdateFilteredList()
    End Sub

    Private Sub UpdateFilteredList()

        If Turnierfilter Is Nothing Then Return

        Dim filteredSpieler = LeseSpieler().ToList

        Dim filter = Turnierfilter.SelectedItems
        If filter.Count > 0 Then
            filteredSpieler = filteredSpieler.Where(Function(x) filter.Contains(x.Klassement)).ToList
        End If

        filteredSpieler = filteredSpieler.Where(Function(x) x.Geschlecht = If(m.IsChecked, 1, 0)).ToList

        Dim ttrValue = -1
        If ttr.IsChecked AndAlso Integer.TryParse(ttrwert.Text, ttrValue) Then
            If min.IsChecked Then
                filteredSpieler = filteredSpieler.Where(Function(x) x.TTRating >= ttrValue).ToList
            Else
                filteredSpieler = filteredSpieler.Where(Function(x) x.TTRating <= ttrValue).ToList
            End If
        End If

        CType(DataContext, MainWindowContext).Spielerliste = filteredSpieler
        linkeListe.ItemsSource = filteredSpieler

        UpdateRightList()
    End Sub

    Private Sub RadioCheckBoxHandler(sender As Object, e As RoutedEventArgs) Handles m.Checked, w.Checked, min.Checked, max.Checked, ttr.Click
        UpdateFilteredList()
    End Sub

    Private Sub TTR_TextChanged(sender As Object, e As TextChangedEventArgs) Handles ttrwert.TextChanged
        'validate text is a number
        e.Handled = Not Integer.TryParse(ttrwert.Text, Nothing)

        'background light red when invalid
        If e.Handled Then
            CType(sender, TextBox).Background = Brushes.LightCoral
        Else
            CType(sender, TextBox).Background = Brushes.White
            UpdateFilteredList()
        End If

    End Sub

    'Modus Combobox Handler
    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles modus.SelectionChanged
        'map selection to FinalMode Enum
        mode = modus.SelectedIndex
        UpdateRightList()
    End Sub

    Private Sub UpdateRightList()
        warning.Text = ""
        warning.Background = Brushes.Transparent

        If mode = -1 Then Return

        Select Case mode
            Case FinalMode.Viertelfinale
                TransferPlayers(8)
            Case FinalMode.Halbfinale
                TransferPlayers(4)
            Case FinalMode.Finalrunde
                TransferPlayers(Integer.MaxValue)
            Case FinalMode.Sieger
                TransferPlayers(Integer.MaxValue)
        End Select
    End Sub

    Private Sub TransferPlayers(amount As Integer)
        Dim leftList = CType(DataContext, MainWindowContext).Spielerliste.Where(Function(x) Not x.Ausgeschieden).ToList
        If leftList.Count = 0 Then Return
        If amount > leftList.Count Then
            amount = leftList.Count
            If mode = FinalMode.Viertelfinale OrElse mode = FinalMode.Halbfinale Then
                warning.Text = "Nicht genug Spieler für das gewählte Finale"
                warning.Background = Brushes.LightCoral
            End If
        End If

        rechteListe.ItemsSource = leftList.Take(amount)
    End Sub

End Class
