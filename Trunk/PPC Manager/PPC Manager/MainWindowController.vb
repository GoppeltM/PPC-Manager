Imports PPC_Manager

Public Class MainWindowController
    Implements IController

    Public Sub New(competition As Competition, speichern As Action, reportFactory As IReportFactory)
        _Competition = competition
        _Speichern = speichern
        _ReportFactory = reportFactory
        If _Competition Is Nothing Then Throw New ArgumentNullException("competition")
    End Sub

    Private ReadOnly _Competition As Competition
    Private ReadOnly _Speichern As Action
    Private ReadOnly _ReportFactory As IReportFactory

    Public ReadOnly Property AktiveCompetition As Competition Implements IController.AktiveCompetition
        Get
            Return _Competition
        End Get
    End Property

    Public Sub RundeVerwerfen() Implements IController.RundeVerwerfen
        With AktiveCompetition.SpielRunden
            .Pop()
            Dim überzählig = (From x In .AusgeschiedeneSpieler Where x.Runde > .Count).ToList

            For Each ausgeschieden In überzählig
                .AusgeschiedeneSpieler.Remove(ausgeschieden)
            Next
        End With
    End Sub

    Public Function NächsteRunde_CanExecute() As Boolean Implements IController.NächsteRunde_CanExecute
        With AktiveCompetition.SpielRunden
            If Not .Any Then
                Return True
            End If

            Dim AktuellePartien = .Peek.ToList

            Dim AlleAbgeschlossen = Aggregate x In AktuellePartien Into All(x.Abgeschlossen)

            Return AlleAbgeschlossen
        End With
    End Function

    Public Sub NächsteRunde_Execute() Implements IController.NächsteRunde_Execute
        ExcelBeschreibbar()
        With AktiveCompetition
            Dim AktiveListe = .SpielerListe.ToList
            For Each Ausgeschieden In .SpielRunden.AusgeschiedeneSpieler
                AktiveListe.Remove(Ausgeschieden.Spieler)
            Next
            Dim RundenName = "Runde " & .SpielRunden.Count + 1
            Dim spielpartien = .SpielRunden.SelectMany(Function(m) m.AsEnumerable)
            Dim habenGegeinanderGespielt = Function(a As Spieler, b As Spieler) New Spielverlauf(spielpartien).Habengegeneinandergespielt(a, b)
            Dim suchePaarungenFunc = Function(istAltschwimmer As Predicate(Of Spieler)) As SuchePaarungen(Of Spieler)
                                         Return Function(spielerliste, absteigend) _
                                         New PaarungsSuche(Of Spieler)(habenGegeinanderGespielt, istAltschwimmer).SuchePaarungen(spielerliste, absteigend)
                                     End Function
            Dim begegnungen = New PaketBildung(suchePaarungenFunc, RundenName, .SpielRegeln.Gewinnsätze).organisierePakete(AktiveListe, .SpielRunden.Count)
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

    Public Sub NächstesPlayoff_Execute() Implements IController.NächstesPlayoff_Execute

        If My.Settings.AutoSaveAn Then
            _Speichern()
        End If

        With AktiveCompetition
            Dim spielRunde As New SpielRunde
            .SpielRunden.Push(spielRunde)
        End With

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
        With AktiveCompetition.SpielRunden
            If .Any Then
                Spielpartien = From x In AktiveCompetition.SpielRunden.Peek Where Not TypeOf x Is FreiLosSpiel
            End If
        End With

        Dim AusgeschiedenInRunde0 = Function(s As Spieler) As Boolean
                                        Return Aggregate x In AktiveCompetition.SpielRunden.AusgeschiedeneSpieler
                                               Where x.Spieler = s AndAlso x.Runde = 0
                                               Into Any()
                                    End Function
        Dim l = (From x In AktiveCompetition.SpielerListe
                 Where Not AusgeschiedenInRunde0(x)
                 Select x).ToList
        l.Sort()
        l.Reverse()

        Dim doc = New FixedDocument
        Dim ranglistenSeiten = From x In (New FixedPageFabrik).ErzeugeRanglisteSeiten(l, seiteneinstellung, AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)
                               Select New PageContent() With {.Child = x}

        Dim spielErgebnisSeiten = From x In (New FixedPageFabrik).ErzeugeSpielErgebnisse(Spielpartien, format, AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)
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
        Dim spieler = AktiveCompetition.SpielerListe.ToList
        spieler.Sort()
        _ReportFactory.SchreibeReport(dateiName)
    End Sub

    Public Sub SaveXML() Implements IController.SaveXML
        _Speichern()
    End Sub

    Public Sub SaveExcel() Implements IController.SaveExcel
        _ReportFactory.AutoSave()
    End Sub


    Public Sub ExcelBeschreibbar()
        Try
            If IO.File.Exists(AktiveCompetition.ExcelPfad) Then
                Using file = IO.File.OpenRead(AktiveCompetition.ExcelPfad)

                End Using
            End If
        Catch ex As IO.IOException
            Throw New ExcelNichtBeschreibbarException
        End Try
    End Sub

    Public Sub SatzEintragen(value As Integer, linksGewonnen As Boolean, partie As SpielPartie) Implements IController.SatzEintragen
        Dim oValue = OtherValue(value)
        If linksGewonnen Then
            Dim temp = value
            value = oValue
            oValue = temp
        End If
        Dim s = New Satz With {.PunkteLinks = value, .PunkteRechts = oValue}
        partie.Add(s)
    End Sub

    Private Function OtherValue(value As Integer) As Integer
        Dim oValue = 11
        If value > 9 Then oValue = value + 2
        Return oValue
    End Function

    Public Function NeuerSatz_CanExecute(s As SpielPartie) As Boolean Implements IController.NeuerSatz_CanExecute
        If s Is Nothing Then
            Return False
        End If


        Dim GewinnLinks = Aggregate x In s Where x.PunkteLinks > x.PunkteRechts Into Count()
        Dim GewinnRechts = Aggregate x In s Where x.PunkteLinks < x.PunkteRechts Into Count()

        Return Math.Max(GewinnLinks, GewinnRechts) < 3
    End Function

    Public Function FilterSpieler(s As Spieler) As Boolean Implements IController.FilterSpieler

        Dim ausgeschiedeneSpieler = AktiveCompetition.SpielRunden.AusgeschiedeneSpieler

        Dim AusgeschiedenVorBeginn = Aggregate x In ausgeschiedeneSpieler Where x.Runde = 0 And
                            x.Spieler = s Into Any()

        Return Not AusgeschiedenVorBeginn
    End Function

    Public Sub NeuePartie(rundenName As String, spielerA As Spieler, SpielerB As Spieler) Implements IController.NeuePartie
        Dim AktuelleRunde = AktiveCompetition.SpielRunden.Peek()
        Dim neueSpielPartie = New SpielPartie(rundenName, spielerA, SpielerB, AktiveCompetition.SpielRegeln.Gewinnsätze)
        neueSpielPartie.ZeitStempel = Date.Now
        AktuelleRunde.Add(neueSpielPartie)
    End Sub


End Class
