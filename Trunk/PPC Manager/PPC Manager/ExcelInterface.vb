Imports Microsoft.Office.Interop.Excel

Public Class ExcelInterface

    Public Shared Sub CreateFile(ByVal filePath As String, ByVal spieler As IEnumerable(Of Spieler))
        Dim excel As Microsoft.Office.Interop.Excel.Application = Nothing
        Try
            excel = New Microsoft.Office.Interop.Excel.Application
            excel.Visible = False
            Dim wb = excel.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet)
            Dim sheet = CType(wb.Worksheets.Add, Worksheet)
            sheet.Name = "Spieler"
            Dim current = 0
            For Each s In spieler
                Dim range = CType(sheet.Cells(current + 1, 1), Range)
                range.Value2 = s.Nachname
                current += 1
            Next

            wb.SaveAs(filePath, XlFileFormat.xlWorkbookDefault)
            wb.Close()
        Finally
            If excel IsNot Nothing Then
                excel.Quit()
            End If

        End Try

        
    End Sub


End Class
