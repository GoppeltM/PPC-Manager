Public Class MainWindowController
    Implements IController

    Public Sub New(competition As Competition)
        _Competition = competition
        If _Competition Is Nothing Then Throw New ArgumentNullException("competition")
    End Sub

    Private ReadOnly _Competition As Competition

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
            Dim begegnungen = New PaketBildung(RundenName, .SpielRegeln.Gewinnsätze).organisierePakete(AktiveListe, .SpielRunden.Count)
            Dim Zeitstempel = Date.Now
            For Each partie In begegnungen
                partie.ZeitStempel = Zeitstempel
            Next

            Dim spielRunde As New SpielRunde

            For Each begegnung In begegnungen
                spielRunde.Add(begegnung)
            Next
            .SpielRunden.Push(spielRunde)

        End With
    End Sub

    Public Sub NächstesPlayoff_Execute() Implements IController.NächstesPlayoff_Execute

        If CBool(My.Settings.AutoSaveAn) Then
            AktiveCompetition.SaveXML()
        End If

        With AktiveCompetition
            Dim spielRunde As New SpielRunde
            .SpielRunden.Push(spielRunde)
        End With

        If CBool(My.Settings.AutoSaveAn) Then
            AktiveCompetition.SaveExcel()
        End If
    End Sub

    Public Sub RundenbeginnDrucken(p As IPrinter) Implements IController.RundenbeginnDrucken

        Dim size = New Size(p.PrintableAreaWidth, p.PrintableAreaHeight)

        Dim neuePaarungenFactory = Function() New NeuePaarungen(AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)

        Dim PaarungenPaginator As New UserControlPaginator(Of NeuePaarungen) _
            (From x In AktiveCompetition.SpielRunden.Peek
             Where Not TypeOf x Is FreiLosSpiel, size, neuePaarungenFactory)

        Dim SchiriFactory = Function() New SchiedsrichterZettel(AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)
        Dim SchiriPaginator As New UserControlPaginator(Of SchiedsrichterZettel)(AktiveCompetition.SpielRunden.Peek, size, SchiriFactory)
        p.PrintDocument(New PaginatingPaginator({PaarungenPaginator, SchiriPaginator}), "Neue Begegnungen - Aushang und Schiedsrichterzettel")
    End Sub

    Public Sub RundenendeDrucken(p As IPrinter) Implements IController.RundenendeDrucken
        Dim size = New Size(p.PrintableAreaWidth, p.PrintableAreaHeight)

        Dim Spielpartien As IEnumerable(Of SpielPartie) = New List(Of SpielPartie)
        With AktiveCompetition.SpielRunden
            If .Any Then
                Spielpartien = From x In AktiveCompetition.SpielRunden.Peek Where Not TypeOf x Is FreiLosSpiel
            End If
        End With

        Dim SpielErgebnisseFactory = Function() New SpielErgebnisse(AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)
        Dim ErgebnissePaginator As New UserControlPaginator(Of SpielErgebnisse)(Spielpartien, size, SpielErgebnisseFactory)
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

        Dim ranglisteFactory = Function() New RanglisteSeite(AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)
        Dim RanglistePaginator As New UserControlPaginator(Of RanglisteSeite)(l, size, ranglisteFactory)
        p.PrintDocument(New PaginatingPaginator({ErgebnissePaginator, RanglistePaginator}), "Rundenende - Aushang und Rangliste")
    End Sub

    Public Sub ExcelExportieren(dateiName As String) Implements IController.ExcelExportieren
        Dim spieler = AktiveCompetition.SpielerListe.ToList
        spieler.Sort()
        ExcelInterface.CreateFile(dateiName, spieler, AktiveCompetition)
    End Sub

    Public Sub SaveXML() Implements IController.SaveXML
        AktiveCompetition.SaveXML()
    End Sub

    Public Sub SaveExcel() Implements IController.SaveExcel
        
        AktiveCompetition.SaveExcel()
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

    Public Sub SatzEintragen(value As Integer, inverted As Boolean, partie As SpielPartie) Implements IController.SatzEintragen
        Dim oValue = OtherValue(value)
        If inverted Then
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
