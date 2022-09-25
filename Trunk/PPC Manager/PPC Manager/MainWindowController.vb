Imports PPC_Manager

Public Delegate Function OrganisierePakete() As PaarungsContainer(Of SpielerInfo)

Public Class MainWindowController
    Implements IController

    Public Sub New(speichern As Action,
                   reportFactory As IReportFactory,
                   organisierePakete As OrganisierePakete,
                   pageFabrik As IFixedPageFabrik
                   )
        _Speichern = speichern
        _ReportFactory = reportFactory
        _OrganisierePakete = organisierePakete
        _PageFabrik = pageFabrik
    End Sub

    Private ReadOnly _Speichern As Action
    Private ReadOnly _ReportFactory As IReportFactory
    Private ReadOnly _OrganisierePakete As OrganisierePakete
    Private ReadOnly _PageFabrik As IFixedPageFabrik
    Private ReadOnly _GewinnSätze As Integer

    Public ReadOnly Property ExcelPfad As String Implements IController.ExcelPfad
        Get
            Return _ReportFactory.ExcelPfad
        End Get
    End Property

    Public Function NächsteRunde(rundenName As String) As SpielRunde Implements IController.NächsteRunde
        _ReportFactory.IstBereit()

        Dim begegnungen = _OrganisierePakete()

        Dim Zeitstempel = Date.Now
        Dim spielRunde As New SpielRunde

        For Each begegnung In begegnungen.Partien
            spielRunde.Add(
                New SpielPartie(rundenName, begegnung.Item1, begegnung.Item2) _
                With {.ZeitStempel = Zeitstempel})
        Next
        If begegnungen.Übrig IsNot Nothing Then
            spielRunde.Add(New FreiLosSpiel(rundenName, begegnungen.Übrig))
        End If
        Return spielRunde

    End Function

    Public Sub ExcelExportieren(dateiName As String) Implements IController.ExcelExportieren
        _ReportFactory.SchreibeReport(dateiName)
    End Sub

    Public Sub SaveXML() Implements IController.SaveXML
        _Speichern()
    End Sub

    Public Sub SaveExcel() Implements IController.SaveExcel
        _ReportFactory.AutoSave()
    End Sub

    Public Function DruckeSchiedsrichterzettel(seiteneinstellung As SeitenEinstellung) As FixedDocument Implements IController.DruckeSchiedsrichterzettel
        Dim doc = New FixedDocument
        Dim schiriSeiten = From x In _PageFabrik.ErzeugeSchiedsrichterZettelSeiten(seiteneinstellung)
                           Select New PageContent() With {.Child = x}

        For Each page In schiriSeiten
            doc.Pages.Add(page)
        Next
        Return doc
    End Function

    Public Function DruckeNeuePaarungen(seiteneinstellung As SeitenEinstellung) As FixedDocument Implements IController.DruckeNeuePaarungen
        Dim neuePaarungenSeiten = From x In _PageFabrik.ErzeugePaarungen(seiteneinstellung)
                                  Select New PageContent() With {.Child = x}

        Dim doc = New FixedDocument

        For Each page In neuePaarungenSeiten
            doc.Pages.Add(page)
        Next
        Return doc
    End Function

    Public Function DruckeRangliste(seiteneinstellung As SeitenEinstellung) As FixedDocument Implements IController.DruckeRangliste
        Dim doc = New FixedDocument
        Dim ranglistenSeiten = From x In _PageFabrik.ErzeugeRanglisteSeiten(
                                   seiteneinstellung)
                               Select New PageContent() With {.Child = x}

        For Each page In ranglistenSeiten
            doc.Pages.Add(page)
        Next
        Return doc
    End Function

    Public Function DruckeSpielergebnisse(seiteneinstellung As SeitenEinstellung) As FixedDocument Implements IController.DruckeSpielergebnisse
        Dim spielErgebnisSeiten = From x In _PageFabrik.ErzeugeSpielErgebnisse(
                                      seiteneinstellung)
                                  Select New PageContent() With {.Child = x}

        Dim doc = New FixedDocument
        For Each page In spielErgebnisSeiten
            doc.Pages.Add(page)
        Next
        Return doc
    End Function
End Class
