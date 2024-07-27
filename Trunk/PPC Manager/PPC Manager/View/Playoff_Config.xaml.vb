﻿
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
                filteredSpieler = filteredSpieler.Where(Function(x) x.TTRating < ttrValue).ToList
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
        Dim filter = Turnierfilter.SelectedItems

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
