﻿
Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class MainWindowContext
    Public Property Spielerliste As ICollection(Of SpielerInfoTurnier)
    Public Property KlassementListe As IEnumerable(Of String)
End Class

Public Class SpielerInfoTurnier
    Inherits SpielerInfo
    Public Property Klassement As String

    Sub New(spieler As SpielerInfo, comp As String)
        MyBase.New(spieler)

        Klassement = comp
    End Sub
End Class

Public Class Playoff_Config

    Public Property Doc As XDocument = Nothing

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
            .KlassementListe = New Collection(Of String)(doc.Root.<competition>.Select(Function(x) x.Attribute("age-group").Value).ToList),
            .Spielerliste = LeseSpieler()
        }

    End Sub

    Public Function LeseSpieler() As IList(Of SpielerInfoTurnier)

        'Todo: need to recalculate player ordering based on match results

        Dim l As New List(Of SpielerInfoTurnier)
        Dim klassements = Doc.Root.<competition>
        For Each klassement In klassements
            Dim comp = klassement.Attribute("age-group").Value
            Dim neuerSpieler = Function(id As String, fremd As Boolean, person As XElement) As SpielerInfoTurnier
                                   Dim s = New SpielerInfoTurnier(New SpielerInfo(id) With {
                                        .Geschlecht = Integer.Parse(person.@sex),
                                        .Lizenznummer = person.Attribute("licence-nr").Value,
                                        .Nachname = person.@lastname,
                                        .Vorname = person.@firstname,
                                        .Vereinsname = person.Attribute("club-name").Value
                                    }, comp)
                                   Integer.TryParse(person.@birthyear, s.Geburtsjahr)
                                   Integer.TryParse(person.@ttr, s.TTRating)
                                   Integer.TryParse(person.Attribute("ttr-match-count")?.Value, s.TTRMatchCount)
                                   Return s
                               End Function

            For Each xmlSpieler In klassement.<players>.<player>
                Dim id = xmlSpieler.@id
                Dim person = xmlSpieler.<person>.Single
                l.Add(neuerSpieler(id, False, person))
            Next


        Next
        Return l
    End Function

    Private Sub ListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim filter = Turnierfilter.SelectedItems
        If filter.Count = 0 Then
            CType(DataContext, MainWindowContext).Spielerliste = LeseSpieler()
            linkeListe.ItemsSource = CType(DataContext, MainWindowContext).Spielerliste
            Return
        End If
        Dim filteredSpieler = LeseSpieler().Where(Function(x) filter.Contains(x.Klassement)).ToList
        CType(DataContext, MainWindowContext).Spielerliste = filteredSpieler
        linkeListe.ItemsSource = filteredSpieler
    End Sub

End Class
