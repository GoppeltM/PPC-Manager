Imports System.IO

Public Class ReportFactory
    Implements IReportFactory
    Private ReadOnly _DateiPfad As String
    Private ReadOnly _Altersgruppe As String
    Private ReadOnly _Spieler As IEnumerable(Of Spieler)
    Private ReadOnly _SpielRunden As IEnumerable(Of SpielRunde)

    Public Sub New(dateipfad As String, altersgruppe As String, spieler As IEnumerable(Of Spieler), spielrunden As IEnumerable(Of SpielRunde))
        _DateiPfad = dateipfad
        _Altersgruppe = altersgruppe
        _Spieler = spieler
        _SpielRunden = spielrunden
    End Sub

    Public Sub AutoSave() Implements IReportFactory.AutoSave
        SchreibeReport(ExcelPfad)
    End Sub

    Public Sub SchreibeReport(ByVal filePath As String) Implements IReportFactory.SchreibeReport
        Dim exportSpieler = (From x In _Spieler Select New ExportSpieler(x)).ToList
        exportSpieler.Sort()
        exportSpieler.Reverse()
        Try

            Using ex = New TurnierReport(filePath)
                With ex
                    Dim RundeNr = _SpielRunden.Count
                    .WriteSpielerSheet(exportSpieler, RundeNr)

                    Dim current = 1
                    For Each runde In _SpielRunden.Reverse
                        Dim currentName = current.ToString.PadLeft(2, "0"c)
                        .WriteRunde(runde, current)
                        current += 1
                    Next
                End With
            End Using

        Catch ex As IOException
            Throw New Exception("Excel Datei konnte nicht geschrieben werden.", ex)
        End Try
    End Sub

    Public Sub ExcelBeschreibbar() Implements IReportFactory.IstBereit
        Try
            If File.Exists(ExcelPfad) Then
                Using file = IO.File.OpenRead(ExcelPfad)

                End Using
            End If
        Catch ex As IOException
            Throw New ExcelNichtBeschreibbarException
        End Try
    End Sub


    Private ReadOnly Property ExcelPfad As String
        Get
            Dim DateiName = IO.Path.GetFileNameWithoutExtension(_DateiPfad)
            DateiName &= "_" & _Altersgruppe
            For Each c In IO.Path.GetInvalidFileNameChars
                DateiName = DateiName.Replace(c, "_"c)
            Next
            DateiName = DateiName.Replace(" "c, "_"c)
            DateiName &= ".xlsx"
            Dim Unterpfad = IO.Path.Combine(IO.Path.GetDirectoryName(_DateiPfad), "Protokolle")
            DateiName = IO.Path.Combine(Unterpfad, DateiName)
            Return DateiName
        End Get
    End Property
End Class
