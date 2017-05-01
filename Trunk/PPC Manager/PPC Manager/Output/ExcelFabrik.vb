Imports System.IO
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Spreadsheet
Imports PPC_Manager

Public Class ExcelFabrik
    Private ReadOnly _Spielstand As ISpielstand

    Public Sub New(spielstand As ISpielstand)
        _Spielstand = spielstand
    End Sub

    Public Function HoleDokument(pfad As String) As ITurnierReport
        Dim doc As IExcelDokument
        If File.Exists(pfad) Then
            doc = OpenExistingExcelDocument(pfad)
        Else
            doc = CreateEmptyExcelDocument(pfad)
        End If
        Return New TurnierReport(doc, _Spielstand)
    End Function

    Private Shared Function OpenExistingExcelDocument(path As String) As IExcelDokument
        Dim MissCount = 0
        Dim spreadsheetDocument As SpreadsheetDocument = Nothing
        While spreadsheetDocument Is Nothing
            Try
                spreadsheetDocument = SpreadsheetDocument.Open(path, True)
                Exit While
            Catch ex As Exception When Not MissCount >= 20
                MissCount += 1
                System.Threading.Thread.Sleep(200)
            End Try
        End While
        Return New ExcelDocument(spreadsheetDocument)
    End Function

    Private Shared Function CreateEmptyExcelDocument(path As String) As IExcelDokument
        ' Create a spreadsheet document by supplying the filepath.
        ' By default, AutoSave = true, Editable = true, and Type = xlsx.
        Dim spreadsheetDocument As SpreadsheetDocument = Nothing
        Dim MissCount = 0
        Dim folder = IO.Path.GetDirectoryName(path)
        If Not IO.Directory.Exists(folder) Then
            IO.Directory.CreateDirectory(folder)
        End If
        While spreadsheetDocument Is Nothing
            Try
                spreadsheetDocument = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook, True)
                Exit While
            Catch ex As Exception When Not MissCount >= 20
                MissCount += 1
                System.Threading.Thread.Sleep(200)
            End Try
        End While


        ' Add a WorkbookPart to the document.
        Dim workbookpart As WorkbookPart = spreadsheetDocument.AddWorkbookPart
        workbookpart.Workbook = New Workbook
        workbookpart.Workbook.BookViews = New BookViews
        workbookpart.Workbook.BookViews.AppendChild(New WorkbookView)

        Dim stringTablePart = workbookpart.AddNewPart(Of SharedStringTablePart)()
        stringTablePart.SharedStringTable = New SharedStringTable

        ' Add Sheets to the Workbook.
        Dim sheets As Sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(Of Sheets)(New Sheets())
        Return New ExcelDocument(spreadsheetDocument)
    End Function

End Class
