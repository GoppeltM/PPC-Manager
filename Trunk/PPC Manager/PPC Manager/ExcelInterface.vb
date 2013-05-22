Imports Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices.Marshal

Public Class ExcelInterface

    Public Shared Sub CreateFile(ByVal filePath As String, ByVal spieler As IEnumerable(Of Spieler), ByVal spielrunden As SpielRunden)
        Dim excel As Microsoft.Office.Interop.Excel.Application = Nothing
        Try
            excel = New Microsoft.Office.Interop.Excel.Application
            excel.Visible = False
            Dim wb = excel.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet)
            Dim sheet = CType(wb.Worksheets.Add, Worksheet)
            sheet.Name = "Spieler"

            WriteSpielerSheet(spieler, sheet)
            WriteRunden(spielrunden, wb)
           
            If IO.File.Exists(filePath) Then
                IO.File.Delete(filePath)
            End If
            wb.SaveAs(filePath, XlFileFormat.xlWorkbookDefault)
            wb.Close()
        Finally
            If excel IsNot Nothing Then
                excel.Quit()
                ReleaseComObject(excel)
            End If

        End Try


    End Sub

    Private Shared Sub WriteSpielerSheet(ByVal spieler As IEnumerable(Of Spieler), ByVal sheet As Worksheet)
        Dim Titles = {"Vorname", "Nachname", "Verein", "Spielklasse",
                          "Position", "Turnierklasse", "Startnummer",
                          "TTRang", "TTRating", "Punkte", "Buchholzpunkte",
                          "Gewonnene Sätze", "Verlorene Sätze", "Gegnerprofil"}
        Dim column = 1
        For Each title In Titles
            CType(sheet.Cells(1, column), Range).Value2 = title
            column += 1
        Next

        Dim current = 2
        For Each s In spieler
            CType(sheet.Cells(current, 1), Range).Value2 = s.Vorname
            CType(sheet.Cells(current, 2), Range).Value2 = s.Nachname
            CType(sheet.Cells(current, 3), Range).Value2 = s.Vereinsname            
            CType(sheet.Cells(current, 7), Range).Value2 = s.Id
            CType(sheet.Cells(current, 9), Range).Value2 = s.TTRating
            CType(sheet.Cells(current, 10), Range).Value2 = s.Punkte
            CType(sheet.Cells(current, 11), Range).Value2 = s.BuchholzPunkte

            Dim AlleSätze = Aggregate x In s.GespieltePartien Into Sum(x.Count)
            Dim ss = s
            Dim meineSätze = Aggregate x In s.GespieltePartien Into Sum(x.MeineGewonnenenSätze(ss).Count)

            CType(sheet.Cells(current, 12), Range).Value2 = meineSätze
            CType(sheet.Cells(current, 13), Range).Value2 = AlleSätze - meineSätze

            column = 14
            Dim gegnerProfil = From x In ss.GespieltePartien Select x.MeinGegner(ss).Id

            For Each gegner In gegnerProfil
                CType(sheet.Cells(current, column), Range).Value2 = gegner
                column += 1
            Next
            current += 1
        Next
    End Sub

    Private Shared Sub WriteRunden(ByVal spielrunden As SpielRunden, ByVal workbook As Workbook)

        Dim currentRunde = 1
        For Each runde In spielrunden
            Dim sheet = CType(workbook.Worksheets.Add, Worksheet)
            sheet.Name = "Runde_" & currentRunde
            currentRunde += 1

            Dim Titles = {"Linker Spieler", "Rechter Spieler", "Sätze Links", "Sätze Rechts"}
            Dim currentTitle = 1
            For Each title In Titles
                CType(sheet.Cells(1, currentTitle), Range).Value2 = title
                currentTitle += 1
            Next
            Dim current = 2
            For Each ergebnis In runde
                CType(sheet.Cells(current, 1), Range).Value2 = ergebnis.SpielerLinks.Id
                CType(sheet.Cells(current, 2), Range).Value2 = ergebnis.SpielerRechts.Id
                CType(sheet.Cells(current, 3), Range).Value2 = ergebnis.MeineGewonnenenSätze(ergebnis.SpielerLinks).Count
                CType(sheet.Cells(current, 4), Range).Value2 = ergebnis.MeineGewonnenenSätze(ergebnis.SpielerRechts).Count
                current += 1
            Next
        Next

    End Sub


End Class
