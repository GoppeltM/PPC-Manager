Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Spreadsheet

Public Class ExcelDocument
    Implements IDisposable
    Implements IExcelDokument

    Private Property doc As SpreadsheetDocument

    Public Sub New(doc As SpreadsheetDocument)
        Me.doc = doc
    End Sub

    Public Sub Speichern() Implements IExcelDokument.Speichern
        SetActiveSheet(CUInt(SheetCount() - 1))
        doc.WorkbookPart.Workbook.Save()
    End Sub

    Public Sub NeueZeile(sheetName As String, Zellen As IEnumerable(Of String)) Implements IExcelDokument.NeueZeile
        Dim sheet = GetSheet(sheetName)
        Dim SheetData = sheet.GetFirstChild(Of SheetData)()
        Dim row = New Row
        row.RowIndex = CType(SheetData.ChildElements.Count + 1, UInt32Value)
        SheetData.Append(row)
        Dim AValue = Convert.ToInt32("A"c)
        Dim ValueCells = Zellen.Zip(Enumerable.Range(AValue, Integer.MaxValue - AValue), Function(x, y) New With {x, .y = Convert.ToChar(y)})
        For Each Value In ValueCells
            Dim CachedCellValue = InsertSharedStringItem(Value.x, doc.WorkbookPart.SharedStringTablePart)
            Dim CellName = Value.y & row.RowIndex.Value
            row.AppendChild(New Cell With {.CellValue = New CellValue(CachedCellValue.ToString),
                            .CellReference = CellName, .DataType = New EnumValue(Of CellValues)(CellValues.SharedString)})
        Next
    End Sub

    Public Sub LeereBlatt(sheetName As String) Implements IExcelDokument.LeereBlatt
        Dim sheet = GetSheet(sheetName)
        Dim SheetData = sheet.GetFirstChild(Of SheetData)()
        SheetData.RemoveAllChildren(Of Row)()
    End Sub

    Private Function SheetCount() As Integer
        Return doc.WorkbookPart.Workbook.Sheets.Count
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
        Return WorkSheetPart.Worksheet
    End Function

    Private Sub SetActiveSheet(id As UInteger)
        Dim bookViews = doc.WorkbookPart.Workbook.BookViews
        Dim view = bookViews.ChildElements.First(Of WorkbookView)()
        view.ActiveTab = id
    End Sub

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
