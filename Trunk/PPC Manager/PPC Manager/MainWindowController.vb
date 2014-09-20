Public Class MainWindowController
    Implements IController

    Public Sub New(competition As Competition)
        _Competition = competition
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
        Try
            If IO.File.Exists(AktiveCompetition.ExcelPfad) Then
                Using file = IO.File.OpenRead(AktiveCompetition.ExcelPfad)

                End Using
            End If
        Catch ex As IO.IOException
            MessageBox.Show(String.Format("Kein Schreibzugriff auf Excel Datei {0} möglich. Bitte Excel vor Beginn der nächsten Runde schließen!", AktiveCompetition.ExcelPfad),
                            "Excel offen", MessageBoxButton.OK)
            Return
        End Try
        RundeBerechnen()
    End Sub


    Private Sub RundeBerechnen()

        If CBool(My.Settings.AutoSaveAn) Then
            AktiveCompetition.SaveXML()
        End If

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


        If CBool(My.Settings.AutoSaveAn) Then
            AktiveCompetition.SaveExcel()
        End If

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

    Public Sub RundenbeginnDrucken(p As PrintDialog) Implements IController.RundenbeginnDrucken

        Dim size = New Size(p.PrintableAreaWidth, p.PrintableAreaHeight)

        Dim neuePaarungenFactory = Function() New NeuePaarungen(AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)

        Dim PaarungenPaginator As New UserControlPaginator(Of NeuePaarungen) _
            (From x In AktiveCompetition.SpielRunden.Peek
             Where Not TypeOf x Is FreiLosSpiel, size, neuePaarungenFactory)

        Dim SchiriFactory = Function() New SchiedsrichterZettel(AktiveCompetition.Altersgruppe, AktiveCompetition.SpielRunden.Count)
        Dim SchiriPaginator As New UserControlPaginator(Of SchiedsrichterZettel)(AktiveCompetition.SpielRunden.Peek, size, SchiriFactory)
        p.PrintDocument(New PaginatingPaginator({PaarungenPaginator, SchiriPaginator}), "Neue Begegnungen - Aushang und Schiedsrichterzettel")
    End Sub

    Public Sub RundenendeDrucken(p As PrintDialog) Implements IController.RundenendeDrucken
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

    Public Sub Save() Implements IController.Save
        AktiveCompetition.SaveXML()
    End Sub
End Class
