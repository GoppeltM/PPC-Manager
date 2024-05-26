
Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class MainWindowContext
    Public Property Spielerliste As ICollection(Of SpielerInfoTurnier)
    Public Property KlassementListe As IEnumerable(Of String)
End Class

Public Class SpielerInfoTurnier
    Inherits SpielerInfo
    Public Property Klassement As String
    Public Property Punkte As Integer = 0
    Public Property Platz As Integer = 99

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
            .KlassementListe = New Collection(Of String)(doc.Root.<competition>.Where(Function(x) x.<players>.<player>.Any).Select(Function(x) x.Attribute("age-group").Value).ToList),
            .Spielerliste = LeseSpieler()
        }

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
            Spieler.Add(New SpielerInfoTurnier(x, comp))
        Next

        For Each s In Spieler
            s.Punkte = spielverlauf.BerechnePunkte(s)
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
