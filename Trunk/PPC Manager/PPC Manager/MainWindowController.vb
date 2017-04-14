Imports PPC_Manager

Public Delegate Function OrganisierePakete(spielerliste As IEnumerable(Of SpielerInfo), runde As Integer) As PaarungsContainer(Of SpielerInfo)

Public Class MainWindowController
    Implements IController

    Public Sub New(spielrunden As SpielRunden,
                  competition As Competition,
                   speichern As Action,
                   reportFactory As IReportFactory,
                   organisierePakete As OrganisierePakete
                   )
        _Spielrunden = spielrunden
        _Competition = competition
        _Speichern = speichern
        _ReportFactory = reportFactory
        _OrganisierePakete = organisierePakete
        If _Competition Is Nothing Then Throw New ArgumentNullException("competition")
    End Sub

    Private ReadOnly _Competition As Competition
    Private ReadOnly _Speichern As Action
    Private ReadOnly _ReportFactory As IReportFactory
    Private ReadOnly _OrganisierePakete As OrganisierePakete
    Private ReadOnly _Spielrunden As SpielRunden

    Public ReadOnly Property AktiveCompetition As Competition
        Get
            Return _Competition
        End Get
    End Property

    Public ReadOnly Property ExcelPfad As String Implements IController.ExcelPfad
        Get
            Return _ReportFactory.ExcelPfad
        End Get
    End Property

    Public Sub RundeVerwerfen() Implements IController.RundeVerwerfen
        With _Spielrunden
            .Pop()
            Dim überzählig = (From x In .AusgeschiedeneSpieler Where x.Runde > .Count).ToList

            For Each ausgeschieden In überzählig
                .AusgeschiedeneSpieler.Remove(ausgeschieden)
            Next
        End With
    End Sub

    Public Sub NächsteRunde() Implements IController.NächsteRunde
        _ReportFactory.IstBereit()
        With AktiveCompetition
            Dim AktiveListe = .SpielerListe.OfType(Of SpielerInfo).ToList
            For Each Ausgeschieden In .SpielRunden.AusgeschiedeneSpieler
                AktiveListe.Remove(Ausgeschieden.Spieler)
            Next
            Dim RundenName = "Runde " & .SpielRunden.Count + 1

            Dim begegnungen = _OrganisierePakete(AktiveListe, .SpielRunden.Count)

            Dim Zeitstempel = Date.Now
            Dim spielRunde As New SpielRunde

            For Each begegnung In begegnungen.Partien
                spielRunde.Add(
                    New SpielPartie(RundenName, begegnung.Item1, begegnung.Item2, .SpielRegeln.Gewinnsätze) _
                    With {.ZeitStempel = Zeitstempel})
            Next
            If begegnungen.Übrig IsNot Nothing Then
                spielRunde.Add(New FreiLosSpiel(RundenName, begegnungen.Übrig, .SpielRegeln.Gewinnsätze))
            End If
            .SpielRunden.Push(spielRunde)

        End With
    End Sub

    Public Sub NächstesPlayoff() Implements IController.NächstesPlayoff

        If My.Settings.AutoSaveAn Then
            _Speichern()
        End If
        _Spielrunden.Push(New SpielRunde)


        If My.Settings.AutoSaveAn Then
            _ReportFactory.AutoSave()
        End If
    End Sub

    Public Sub RundenbeginnDrucken(p As IPrinter) Implements IController.RundenbeginnDrucken
        Dim seiteneinstellung = p.Konfigurieren()
        Dim format = New Size(seiteneinstellung.Breite, seiteneinstellung.Höhe)
        Dim doc = New FixedDocument
        Dim schiriSeiten = From x In (New FixedPageFabrik).ErzeugeSchiedsrichterZettelSeiten(AktiveCompetition.SpielRunden.Peek,
                               format, AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)
                           Select New PageContent() With {.Child = x}

        For Each page In schiriSeiten
            doc.Pages.Add(page)
        Next

        Dim neuePaarungenSeiten = From x In (New FixedPageFabrik).ErzeugeSpielErgebnisse(AktiveCompetition.SpielRunden.Peek,
                               format, AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)
                                  Select New PageContent() With {.Child = x}

        For Each page In neuePaarungenSeiten
            doc.Pages.Add(page)
        Next

        p.Drucken(doc, "Neue Begegnungen - Aushang und Schiedsrichterzettel")
    End Sub

    Public Sub RundenendeDrucken(p As IPrinter) Implements IController.RundenendeDrucken
        Dim seiteneinstellung = p.Konfigurieren()
        Dim format = New Size(seiteneinstellung.Breite, seiteneinstellung.Höhe)

        Dim Spielpartien As IEnumerable(Of SpielPartie) = New List(Of SpielPartie)
        With _Spielrunden
            If .Any Then
                Spielpartien = From x In .Peek Where Not TypeOf x Is FreiLosSpiel
            End If
        End With

        Dim AusgeschiedenInRunde0 = Function(s As SpielerInfo) As Boolean
                                        Return Aggregate x In _Spielrunden.AusgeschiedeneSpieler
                                               Where x.Spieler = s AndAlso x.Runde = 0
                                               Into Any()
                                    End Function
        Dim l = (From x In AktiveCompetition.SpielerListe
                 Where Not AusgeschiedenInRunde0(x)
                 Select x).ToList
        l.Sort()
        l.Reverse()

        Dim doc = New FixedDocument
        Dim allePartien = _Spielrunden.SelectMany(Function(m) m)
        Dim ranglistenSeiten = From x In (New FixedPageFabrik).ErzeugeRanglisteSeiten(l.OfType(Of Spieler),
                                   seiteneinstellung,
                                   AktiveCompetition.Altersgruppe,
                                   _Spielrunden.Count,
                                   allePartien
)
                               Select New PageContent() With {.Child = x}

        Dim spielErgebnisSeiten = From x In (New FixedPageFabrik).ErzeugeSpielErgebnisse(
                                      Spielpartien,
                                      format,
                                      AktiveCompetition.Altersgruppe,
                                      _Spielrunden.Count)
                                  Select New PageContent() With {.Child = x}

        For Each page In spielErgebnisSeiten
            doc.Pages.Add(page)
        Next

        For Each page In ranglistenSeiten
            doc.Pages.Add(page)
        Next
        p.Drucken(doc, "Rundenende - Aushang und Rangliste")
    End Sub

    Public Sub ExcelExportieren(dateiName As String) Implements IController.ExcelExportieren
        _ReportFactory.SchreibeReport(dateiName)
    End Sub

    Public Sub SaveXML() Implements IController.SaveXML
        _Speichern()
    End Sub

    Public Sub SaveExcel() Implements IController.SaveExcel
        _ReportFactory.AutoSave()
    End Sub



    Public Sub NeuePartie(rundenName As String, spielerA As SpielerInfo, SpielerB As SpielerInfo) Implements IController.NeuePartie
        Dim AktuelleRunde = _Spielrunden.Peek()
        Dim neueSpielPartie = New SpielPartie(rundenName,
                                              spielerA,
                                              SpielerB,
                                              AktiveCompetition.SpielRegeln.Gewinnsätze)
        neueSpielPartie.ZeitStempel = Date.Now
        AktuelleRunde.Add(neueSpielPartie)
    End Sub

    Public Sub SpielerAusscheiden(spieler As SpielerInfo) Implements IController.SpielerAusscheiden
        _Spielrunden.AusgeschiedeneSpieler.Add(
            New Ausgeschieden(Of SpielerInfo) With {.Spieler = spieler, .Runde = AktiveCompetition.SpielRunden.Count})
    End Sub
End Class
