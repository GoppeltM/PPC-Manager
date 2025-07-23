
Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class MainWindowContext
    Public Property Spielerliste As ICollection(Of SpielerInfoTurnier)
    Public Property KlassementListe As IEnumerable(Of TabHeader)
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

    'during loading of props we don't want to re-save them as both window instances might be present, leading to issues
    Private Property SupressSaveProps As Boolean = False

    Public Sub New()
        UpdateContext(Nothing)

        InitializeComponent()
    End Sub

    Public Sub UpdateContext(doc As XDocument)
        Me.Doc = doc

        If Me.Doc Is Nothing Then
            DataContext = New MainWindowContext With {
                .Spielerliste = New List(Of SpielerInfoTurnier),
                .KlassementListe = New List(Of TabHeader)
            }
            Return
        End If

        Dim app = CType(Application.Current, Application)

        DataContext = New MainWindowContext With {
            .KlassementListe = app.MakeTabHeaders("").Where(Function(tab) tab.PlayerCount > 0),
            .Spielerliste = LeseSpieler()
        }

        Try
            LoadFilters()
        Catch ex As Exception
            'Filters not yet initialized
            SupressSaveProps = False
            SaveFilters(True)
            LoadFilters()
        End Try

        UpdateFilteredList()

    End Sub

    Public Function LeseSpieler() As IList(Of SpielerInfoTurnier)
        Dim allespieler As New List(Of SpielerInfoTurnier)
        Dim klassements = Doc.Root.<competition>.OrderBy(Function(x) x.Attribute("ttr-from").Value).Reverse

        For Each klassement In klassements
            Dim comp = klassement.Attribute("ttr-remarks").Value

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
            Doc = XDocument.Load(CType(Application.Current, Application).xmlPfad)
        Next

        UpdateFilteredList()
        linkeListe.UnselectAll()
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
            Doc = XDocument.Load(CType(Application.Current, Application).xmlPfad)
        Next

        UpdateFilteredList()
        linkeListe.UnselectAll()
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

    Private Sub LoadFilters()
        If Turnierfilter Is Nothing Then Return
        Dim app = CType(Application.Current, Application)

        Dim CompetitionNode = (From x In Doc.Root.<competition> Where x.Attribute("ttr-remarks").Value = app.competition).Single
        Dim players = CompetitionNode.<players>
        If players IsNot Nothing AndAlso players.<player> IsNot Nothing AndAlso players.<player>.Count > 0 Then Return

        SupressSaveProps = True

        With CompetitionNode
            mode = Integer.Parse(.@ppc:finalsmodeSetting)
            modus.SelectedIndex = mode
            ttr.IsChecked = Boolean.Parse(.@ppc:ttrActive)
            ttrwert.Text = .@ppc:ttrLimit
            min.IsChecked = Boolean.Parse(.@ppc:ttrIsMin)
            max.IsChecked = Boolean.Parse(.@ppc:ttrIsMin) = False
            m.IsChecked = .@ppc:sex.Equals("m")
            w.IsChecked = .@ppc:sex.Equals("w")
            u.IsChecked = .@ppc:sex.Equals("u")
            birthyearmin.Text = If(If(.@ppc:birthyearMin, .Attribute("age-to").Value), "1900")
            birthyearmax.Text = If(If(.@ppc:birthyearMax, .Attribute("age-from").Value), "3000")
        End With

        Dim compss = CompetitionNode.@ppc:competitions

        Dim comps = CompetitionNode.@ppc:competitions.Split(";".ToCharArray()(0)).ToList
        For Each comp In comps
            If comp.Equals("") Then Continue For
            Dim tab = Turnierfilter.Items.Cast(Of TabHeader).Single(Function(t) t.HeaderText = comp)
            Turnierfilter.SelectedItems.Add(tab)
        Next

        SupressSaveProps = False

    End Sub

    Private Sub SaveFilters(Optional force As Boolean = False)
        If Turnierfilter Is Nothing Then Return
        If SupressSaveProps Then Return

        Dim app = CType(Application.Current, Application)
        If Not force AndAlso Not CType(app.MainWindow, MainWindow).PlayOffConfigVisible Then Return

        Dim CompetitionNode = (From x In Doc.Root.<competition> Where x.Attribute("ttr-remarks").Value = app.competition).Single
        Dim players = CompetitionNode.<players>
        If players IsNot Nothing AndAlso players.<player> IsNot Nothing AndAlso players.<player>.Count > 0 Then Return

        Dim competitions = Turnierfilter.SelectedItems.Cast(Of TabHeader).Select(Function(tab) tab.HeaderText).ToList()

        With CompetitionNode
            .@ppc:finalsmodeSetting = mode.ToString
            .@ppc:ttrActive = ttr.IsChecked.ToString
            .@ppc:ttrLimit = ttrwert.Text
            .@ppc:ttrIsMin = min.IsChecked.ToString
            .@ppc:sex = If(m.IsChecked, "m", If(u.IsChecked, "u", "w"))
            .@ppc:competitions = String.Join(";", competitions)
            .@ppc:birthyearMin = birthyearmin.Text
            .@ppc:birthyearMax = birthyearmax.Text
        End With

        Doc.Save(app.xmlPfad)
    End Sub

    Private Sub UpdateFilteredList()

        If Turnierfilter Is Nothing Then Return

        Dim filteredSpieler = LeseSpieler().ToList

        'Klassement Filter
        Dim filter = Turnierfilter.SelectedItems.Cast(Of TabHeader).Select(Function(tab) tab.HeaderText)
        If filter.Count > 0 Then
            filteredSpieler = filteredSpieler.Where(Function(x) filter.Contains(x.Klassement)).ToList
        End If

        'Geschlecht Filter
        If Not u.IsChecked Then
            filteredSpieler = filteredSpieler.Where(Function(x) x.Geschlecht = If(m.IsChecked, 1, 0)).ToList
        End If

        'TTR Filter
        Dim ttrValue = -1
        If ttr.IsChecked AndAlso Integer.TryParse(ttrwert.Text, ttrValue) Then
            If min.IsChecked Then
                filteredSpieler = filteredSpieler.Where(Function(x) x.TTRating >= ttrValue).ToList
            Else
                filteredSpieler = filteredSpieler.Where(Function(x) x.TTRating < ttrValue).ToList
            End If
        End If

        'Geburtsjahr Filter
        Dim minyear As Integer = 0
        If Integer.TryParse(birthyearmin.Text, minyear) Then
            filteredSpieler = filteredSpieler.Where(Function(x) x.Geburtsjahr >= minyear).ToList
        End If

        Dim maxyear As Integer = Integer.MaxValue
        If Integer.TryParse(birthyearmax.Text, maxyear) Then
            filteredSpieler = filteredSpieler.Where(Function(x) x.Geburtsjahr <= maxyear).ToList
        End If

        CType(DataContext, MainWindowContext).Spielerliste = filteredSpieler
        linkeListe.ItemsSource = filteredSpieler

        UpdateRightList()
        SaveFilters()
    End Sub

    Private Sub RadioCheckBoxHandler(sender As Object, e As RoutedEventArgs) Handles m.Checked, w.Checked, u.Checked, min.Checked, max.Checked, ttr.Click
        UpdateFilteredList()
    End Sub

    Private Sub TTR_TextChanged(sender As Object, e As TextChangedEventArgs) Handles ttrwert.TextChanged, birthyearmax.TextChanged, birthyearmin.TextChanged
        'validate text is a number
        e.Handled = Not Integer.TryParse(CType(sender, TextBox).Text, Nothing)

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

        UpdateFilteredList()
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
        Start.IsEnabled = True

        Dim leftList = CType(DataContext, MainWindowContext).Spielerliste.Where(Function(x) Not x.Ausgeschieden).ToList
        If leftList.Count = 0 Then
            Start.IsEnabled = False
            rechteListe.ItemsSource = New List(Of SpielerInfoTurnier)
            Return
        End If

        If amount > leftList.Count Then
            If mode = FinalMode.Viertelfinale OrElse mode = FinalMode.Halbfinale Then
                ' Mit Freilosen gehen zB. 5 Spieler fürs Viertelfinale. ab 4 Spielern muss dann Halbfinale gewählt werden.
                If (amount / 2) + 1 > leftList.Count Then
                    warning.Text = "Nicht genug Spieler für das gewählte Finale"
                    warning.Background = Brushes.LightCoral
                    Start.IsEnabled = False
                Else
                    warning.Text = "Freilose werden verwendet"
                    warning.Background = Brushes.Yellow
                End If
            End If
            amount = leftList.Count
        End If

        rechteListe.ItemsSource = leftList.Take(amount)
    End Sub

    Private Sub Start_Playoff(sender As Object, e As RoutedEventArgs) Handles Start.Click
        'show confirm popup with a summary of configuration, if confirmed, copy players from right list to competition and start playoff

        Dim rightList = rechteListe.ItemsSource.Cast(Of SpielerInfoTurnier).ToList
        If rightList.Count = 0 Then Return

        If MessageBox.Show(GetRulesDescription, "Bestätigung", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
            Dim app = CType(Application.Current, Application)

            ZuXML.AddSpieler(Doc, rightList, app.competition, mode)

            CType(app.MainWindow, MainWindow).SkipDialog = True
            app.LadeCompetition(Nothing, app.competition)
        End If

    End Sub

    Private Function GetRulesDescription() As String
        Dim modeName As String = "Starte "
        Select Case mode
            Case FinalMode.Viertelfinale
                modeName &= "Viertelfinale"
            Case FinalMode.Halbfinale
                modeName &= "Halbfinale"
            Case FinalMode.Finalrunde
                modeName &= "Finalrunde"
            Case FinalMode.Sieger
                modeName &= "Sieger"
        End Select
        modeName &= " mit folgender Konfiguration:" & vbCrLf & vbCrLf
        'map selected Items to Names
        Dim filter = Turnierfilter.SelectedItems.Cast(Of TabHeader).Select(Function(tab) tab.HeaderText)

        If filter.Count > 0 Then
            modeName &= "Altersklasse: " & String.Join(", ", filter.Cast(Of String)) & vbCrLf
        End If
        If m.IsChecked Then
            modeName &= "Geschlecht: männlich" & vbCrLf
        ElseIf w.IsChecked Then
            modeName &= "Geschlecht: weiblich" & vbCrLf
        End If
        If ttr.IsChecked Then
            modeName &= "TTR-Wert: " & ttrwert.Text & " ("
            If min.IsChecked Then
                modeName &= "min)" & vbCrLf
            ElseIf max.IsChecked Then
                modeName &= "max)" & vbCrLf
            End If
        End If

        modeName &= "Anzahl Spieler: " & rechteListe.ItemsSource.Cast(Of SpielerInfoTurnier).ToList.Count

        Return modeName
    End Function
End Class
