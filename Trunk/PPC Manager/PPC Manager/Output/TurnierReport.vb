Imports DocumentFormat.OpenXml.Spreadsheet
Imports System.IO


Public Class TurnierReport
    Implements IDisposable

    Public Sub New(filePath As String)
        If File.Exists(filePath) Then
            _SpreadSheet = ExcelDocument.OpenExistingExcelDocument(filePath)
        Else
            _SpreadSheet = ExcelDocument.CreateEmptyExcelDocument(filePath)
        End If
    End Sub

    Private ReadOnly _SpreadSheet As ExcelDocument

    Public Sub WriteSpielerSheet(ByVal spieler As IEnumerable(Of ExportSpieler), rundeNr As Integer)
        Dim rundeString = rundeNr.ToString.PadLeft(2, "0"c)
        Dim sheet = _SpreadSheet.GetSheet("sp_rd" & rundeString)
        Dim Titles = {"Rang", "Vorname", "Nachname", "ID", "Geschlecht", "Geburtsjahr", "Verein", "TTRating", "Punkte", "Buchholzpunkte", "SonnebornBergerpunkte",
                          "Gewonnene Sätze", "Verlorene Sätze", "Ausgeschieden", "Gegnerprofil"}


        Dim SheetData = sheet.GetFirstChild(Of SheetData)()
        SheetData.RemoveAllChildren(Of Row)()

        _SpreadSheet.CreateRow(SheetData, 1, Titles)

        Dim current = 2UI

        For Each s In spieler

            Dim Geschlecht = Function() As String
                                 Select Case s.Geschlecht
                                     Case 0 : Return "w"
                                     Case 1 : Return "m"
                                     Case Else : Throw New ArgumentException("Unbekanntes Geschlecht: " & s.Geschlecht)
                                 End Select
                             End Function

            Dim Werte = {(current - 1).ToString, s.Vorname, s.Nachname, s.Id.ToString, Geschlecht(), s.Geburtsjahr.ToString, s.Vereinsname, s.TTRating.ToString,
                         s.Punkte.ToString, s.BuchholzPunkte.ToString, s.SonneBornBergerPunkte.ToString, s.SätzeGewonnen.ToString,
                         s.SätzeVerloren.ToString, s.Ausgeschieden.ToString}.Concat(s.GegnerProfil)
            _SpreadSheet.CreateRow(SheetData, current, Werte)
            current += 1UI
        Next
        _SpreadSheet.Save()
    End Sub

    Public Sub WriteRunde(ByVal spielrunde As SpielRunde, runde As Integer)
        Dim currentName = runde.ToString.PadLeft(2, "0"c)
        Dim worksheet = _SpreadSheet.GetSheet("erg_rd" & currentName)
        Dim SheetData = Worksheet.GetFirstChild(Of SheetData)()
        SheetData.RemoveAllChildren(Of Row)()

        Dim Titles = {"Linker Spieler", "Rechter Spieler", "Sätze Links", "Sätze Rechts"}
        _SpreadSheet.CreateRow(SheetData, 1, Titles)

        Dim current = 2UI

        For Each ergebnis In spielrunde
            Dim Werte = {ergebnis.SpielerLinks.Id, ergebnis.SpielerRechts.Id, ergebnis.MeineGewonnenenSätze(ergebnis.SpielerLinks).Count.ToString,
                         ergebnis.MeineGewonnenenSätze(ergebnis.SpielerRechts).Count.ToString}
            _SpreadSheet.CreateRow(SheetData, current, Werte)
            current += 1UI
        Next
        _SpreadSheet.SetActiveSheet(CUInt(_SpreadSheet.SheetCount - 1))
        _SpreadSheet.Save()
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                _SpreadSheet.Dispose()
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
