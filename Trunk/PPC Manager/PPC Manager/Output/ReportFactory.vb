Imports System.IO
Imports PPC_Manager

Public Class ReportFactory
    Implements IReportFactory
    Private ReadOnly _DateiPfad As String
    Private ReadOnly _Altersgruppe As String
    Private ReadOnly _Spieler As IEnumerable(Of SpielerInfo)
    Private ReadOnly _SpielRunden As IEnumerable(Of SpielRunde)
    Private ReadOnly _HoleDokument As Func(Of String, ITurnierReport)
    Private ReadOnly _Vergleicher As IComparer(Of SpielerInfo)

    Public Sub New(dateipfad As String,
                   altersgruppe As String,
                   spieler As IEnumerable(Of SpielerInfo),
                   vergleicher As IComparer(Of SpielerInfo),
                   SpielRunden As IEnumerable(Of SpielRunde),
                   holeDokument As Func(Of String, ITurnierReport))
        _DateiPfad = dateipfad
        _Altersgruppe = altersgruppe
        _Spieler = spieler
        _Vergleicher = vergleicher
        _SpielRunden = SpielRunden
        _HoleDokument = holeDokument
    End Sub

    Public Sub AutoSave() Implements IReportFactory.AutoSave
        SchreibeReport(ExcelPfad)
    End Sub

    Public Sub SchreibeReport(ByVal filePath As String) Implements IReportFactory.SchreibeReport
        Dim exportSpieler = _Spieler.ToList
        exportSpieler.Sort(_Vergleicher)
        exportSpieler.Reverse()
        Try

            Using ex = _HoleDokument(filePath)
                With ex
                    Dim RundeNr = _SpielRunden.Count - 1
                    .SchreibeRangliste(exportSpieler, RundeNr)

                    Dim current = 1
                    For Each runde In _SpielRunden.Reverse.Skip(1)
                        .SchreibeNeuePartien(runde, current)
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


    Public ReadOnly Property ExcelPfad As String Implements IReportFactory.ExcelPfad
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
