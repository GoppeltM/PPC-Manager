Imports PPC_Manager

Public Delegate Function OrganisierePakete(spielerliste As IEnumerable(Of SpielerInfo), runde As Integer) As PaarungsContainer(Of SpielerInfo)

Public Class MainWindowController
    Implements IController

    Public Sub New(spielerListe As IEnumerable(Of SpielerInfo),
                  spielrunden As SpielRunden,
                   speichern As Action,
                   reportFactory As IReportFactory,
                   organisierePakete As OrganisierePakete,
                   druckFabrik As IFixedPageFabrik,
                   gewinnSätze As Integer
                   )
        _SpielerListe = spielerListe
        _Spielrunden = spielrunden
        _Speichern = speichern
        _ReportFactory = reportFactory
        _OrganisierePakete = organisierePakete
        _DruckFabrik = druckFabrik
        _GewinnSätze = gewinnSätze
    End Sub

    Private ReadOnly _Speichern As Action
    Private ReadOnly _ReportFactory As IReportFactory
    Private ReadOnly _OrganisierePakete As OrganisierePakete
    Private ReadOnly _Spielrunden As SpielRunden
    Private ReadOnly _DruckFabrik As IFixedPageFabrik
    Private ReadOnly _GewinnSätze As Integer
    Private ReadOnly _SpielerListe As IEnumerable(Of SpielerInfo)

    Public ReadOnly Property ExcelPfad As String Implements IController.ExcelPfad
        Get
            Return _ReportFactory.ExcelPfad
        End Get
    End Property

    Public Sub NächsteRunde() Implements IController.NächsteRunde
        _ReportFactory.IstBereit()

        Dim AktiveListe = _SpielerListe.ToList
        For Each Ausgeschieden In _Spielrunden.AusgeschiedeneSpieler
            AktiveListe.Remove(Ausgeschieden.Spieler)
        Next
        Dim RundenName = "Runde " & _Spielrunden.Count + 1

        Dim begegnungen = _OrganisierePakete(AktiveListe, _Spielrunden.Count)

        Dim Zeitstempel = Date.Now
            Dim spielRunde As New SpielRunde

            For Each begegnung In begegnungen.Partien
                spielRunde.Add(
                    New SpielPartie(RundenName, begegnung.Item1, begegnung.Item2, _GewinnSätze) _
                    With {.ZeitStempel = Zeitstempel})
            Next
            If begegnungen.Übrig IsNot Nothing Then
                spielRunde.Add(New FreiLosSpiel(RundenName, begegnungen.Übrig, _GewinnSätze))
            End If
        _Spielrunden.Push(spielRunde)

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
        Dim doc = New FixedDocument
        Dim schiriSeiten = From x In _DruckFabrik.ErzeugeSchiedsrichterZettelSeiten(seiteneinstellung)
                           Select New PageContent() With {.Child = x}

        For Each page In schiriSeiten
            doc.Pages.Add(page)
        Next

        Dim neuePaarungenSeiten = From x In _DruckFabrik.ErzeugeSpielErgebnisse(seiteneinstellung)
                                  Select New PageContent() With {.Child = x}

        For Each page In neuePaarungenSeiten
            doc.Pages.Add(page)
        Next

        p.Drucken(doc, "Neue Begegnungen - Aushang und Schiedsrichterzettel")
    End Sub

    Public Sub RundenendeDrucken(p As IPrinter) Implements IController.RundenendeDrucken
        Dim seiteneinstellung = p.Konfigurieren()

        Dim doc = New FixedDocument
        Dim ranglistenSeiten = From x In _DruckFabrik.ErzeugeRanglisteSeiten(
                                   seiteneinstellung)
                               Select New PageContent() With {.Child = x}

        Dim spielErgebnisSeiten = From x In _DruckFabrik.ErzeugeSpielErgebnisse(
                                      seiteneinstellung)
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
                                              _GewinnSätze)
        neueSpielPartie.ZeitStempel = Date.Now
        AktuelleRunde.Add(neueSpielPartie)
    End Sub

    Public Sub SpielerAusscheiden(spieler As SpielerInfo) Implements IController.SpielerAusscheiden
        _Spielrunden.AusgeschiedeneSpieler.Add(
            New Ausgeschieden(Of SpielerInfo) With {.Spieler = spieler, .Runde = _Spielrunden.Count})
    End Sub
End Class
