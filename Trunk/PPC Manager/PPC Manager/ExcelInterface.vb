Imports System.Runtime.InteropServices.Marshal
Imports System.Text
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Spreadsheet


Public Class ExcelInterface
    Implements IDisposable


    Private Sub New(filePath As String)
        If IO.File.Exists(filePath) Then
            doc = OpenExistingExcelDocument(filePath)
        Else
            doc = CreateEmptyExcelDocument(filePath)
        End If
    End Sub

    Private Property doc As SpreadsheetDocument

    Public Shared Sub CreateFile(ByVal filePath As String, ByVal spieler As IEnumerable(Of Spieler), ByVal spielrunden As SpielRunden)


        Using ex = New ExcelInterface(filePath)
            With ex
                Dim RundeNr = spielrunden.Count.ToString.PadLeft(2, "0"c)
                Dim sheet = .GetSheet("sp_rd" & RundeNr)
                .WriteSpielerSheet(spieler, sheet)

                Dim current = 1
                For Each runde In spielrunden.Reverse
                    Dim currentName = current.ToString.PadLeft(2, "0"c)
                    sheet = .GetSheet("erg_rd" & currentName)
                    .WriteRunde(runde, sheet)
                    current += 1
                Next
            End With
        End Using

    End Sub

    Private Sub WriteSpielerSheet(ByVal spieler As IEnumerable(Of Spieler), ByVal sheet As Worksheet)
        Dim Titles = {"Vorname", "Nachname", "Geschlecht", "Geburtsjahr", "Verein", "ID", "TTRating", "Punkte", "Buchholzpunkte", "SonnebornBergerpunkte",
                          "Gewonnene Sätze", "Verlorene Sätze", "Gegnerprofil"}


        Dim SheetData = sheet.GetFirstChild(Of SheetData)()
        SheetData.RemoveAllChildren(Of Row)()
        
        CreateRow(SheetData, 1, Titles)

        Dim current = 2UI

        For Each s In spieler
            Dim gegnerProfil = From x In s.GespieltePartien Select x.MeinGegner(s).Id

            Dim Geschlecht = Function() As String
                                 Select Case s.Geschlecht
                                     Case 0 : Return "w"
                                     Case 1 : Return "m"
                                     Case Else : Throw New ArgumentException("Unbekanntes Geschlecht: " & s.Geschlecht)
                                 End Select
                             End Function

            Dim Werte = {s.Vorname, s.Nachname, Geschlecht(), s.Geburtsjahr.ToString, s.Vereinsname, s.Id.ToString, s.TTRating.ToString,
                         s.Punkte.ToString, s.ExportBHZ.ToString, s.ExportSonneborn.ToString, s.ExportSätzeGewonnen.ToString,
                         s.ExportSätzeVerloren.ToString}.Concat(gegnerProfil)
            CreateRow(SheetData, current, Werte)
            current += 1UI
        Next

    End Sub

    Private Sub WriteRunde(ByVal spielrunde As SpielRunde, ByVal worksheet As Worksheet)
        Dim SheetData = worksheet.GetFirstChild(Of SheetData)()
        SheetData.RemoveAllChildren(Of Row)()

        Dim Titles = {"Linker Spieler", "Rechter Spieler", "Sätze Links", "Sätze Rechts"}
        CreateRow(SheetData, 1, Titles)

        Dim current = 2UI

        For Each ergebnis In spielrunde
            Dim Werte = {ergebnis.SpielerLinks.Id, ergebnis.SpielerRechts.Id, ergebnis.MeineGewonnenenSätze(ergebnis.SpielerLinks).Count.ToString,
                         ergebnis.MeineGewonnenenSätze(ergebnis.SpielerRechts).Count.ToString}
            CreateRow(SheetData, current, Werte)
            current += 1UI
        Next

    End Sub

    Private Sub CreateRow(sheetdata As SheetData, rowIndex As UInteger, entries As IEnumerable(Of String))
        Dim row = New Row
        row.RowIndex = rowIndex
        sheetdata.Append(row)
        Dim AValue = Convert.ToInt32("A"c)
        Dim ValueCells = entries.Zip(Enumerable.Range(AValue, Integer.MaxValue - AValue), Function(x, y) New With {x, .y = Convert.ToChar(y)})
        For Each Value In ValueCells
            Dim CachedCellValue = InsertSharedStringItem(Value.x, doc.WorkbookPart.SharedStringTablePart)
            Dim CellName = Value.y & row.RowIndex.Value
            row.AppendChild(Of Cell)(New Cell With {.CellValue = New CellValue(CachedCellValue.ToString),
                            .CellReference = CellName, .DataType = New EnumValue(Of CellValues)(CellValues.SharedString)})
        Next
    End Sub



    Private Function OpenExistingExcelDocument(path As String) As SpreadsheetDocument
        Dim MissCount = 0
        Dim spreadsheetDocument As SpreadsheetDocument = Nothing
        While spreadsheetDocument Is Nothing
            Try
                spreadsheetDocument = spreadsheetDocument.Open(path, True)
                Exit While
            Catch ex As Exception When Not MissCount >= 20
                MissCount += 1
                System.Threading.Thread.Sleep(200)
            End Try
        End While
        Return spreadsheetDocument
    End Function

    Private Function CreateEmptyExcelDocument(path As String) As SpreadsheetDocument
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
                spreadsheetDocument = spreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook, True)
                Exit While
            Catch ex As Exception When Not MissCount >= 20
                MissCount += 1
                System.Threading.Thread.Sleep(200)
            End Try
        End While


        ' Add a WorkbookPart to the document.
        Dim workbookpart As WorkbookPart = spreadsheetDocument.AddWorkbookPart
        workbookpart.Workbook = New Workbook

        Dim stringTablePart = workbookpart.AddNewPart(Of SharedStringTablePart)()
        stringTablePart.SharedStringTable = New SharedStringTable

        ' Add Sheets to the Workbook.
        Dim sheets As Sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(Of Sheets)(New Sheets())
        Return spreadsheetDocument
    End Function

    Private Function GetSheet(sheetName As String) As Worksheet
        Dim Sheets = doc.WorkbookPart.Workbook.Sheets
        Dim MySheet = (From x In Sheets.Elements(Of Sheet)() Where x.Name = sheetName).FirstOrDefault
        If MySheet IsNot Nothing Then
            Dim Worksheet = doc.WorkbookPart.GetPartById(MySheet.Id)
            Return DirectCast(Worksheet, WorksheetPart).Worksheet
        End If
        ' Add a WorksheetPart to the WorkbookPart.
        Dim WorkSheetPart As WorksheetPart
        WorkSheetPart = doc.WorkbookPart.AddNewPart(Of WorksheetPart)()
        WorkSheetPart.Worksheet = New Worksheet(New SheetData())

        Dim SheetIDs = (From x In Sheets.Elements(Of Sheet)() Select x.SheetId.Value).ToList

        Dim CurrentId = 1UI
        While SheetIDs.Contains(CurrentId)
            CurrentId += 1UI
        End While

        ' Append a new worksheet and associate it with the workbook.
        Dim sheet As Sheet = New Sheet
        sheet.Id = doc.WorkbookPart.GetIdOfPart(WorkSheetPart)
        sheet.SheetId = CurrentId
        sheet.Name = sheetName

        Sheets.Append(sheet)
        Return worksheetPart.Worksheet
    End Function

    Private Function GetWorkSheet(sheetName As String) As Worksheet        
        For Each Sheet In doc.WorkbookPart.Workbook.Sheets.Elements(Of Sheet)()            
            If Sheet.Name = sheetName Then
                Dim ID = doc.WorkbookPart.GetPartById(Sheet.Id)
                Return DirectCast(ID, WorksheetPart).Worksheet
            End If
        Next
        Throw New ArgumentException("No sheet found!", sheetName)
    End Function

    ' Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
    ' and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
    Private Function InsertSharedStringItem(ByVal text As String, ByVal shareStringPart As SharedStringTablePart) As Integer
        Dim i As Integer = 0

        ' Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
        For Each item As SharedStringItem In shareStringPart.SharedStringTable.Elements(Of SharedStringItem)()
            If (item.InnerText = text) Then
                Return i
            End If
            i = (i + 1)
        Next

        ' The text does not exist in the part. Create the SharedStringItem and return its index.
        shareStringPart.SharedStringTable.AppendChild(New SharedStringItem(New DocumentFormat.OpenXml.Spreadsheet.Text(text)))
        shareStringPart.SharedStringTable.Save()

        Return i
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                doc.Dispose()
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
