Imports System.Runtime.InteropServices.Marshal
Imports System.Text
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Spreadsheet
Imports System.IO


Public Class TurnierReport
    Implements IDisposable


    Private Sub New(filePath As String)
        If IO.File.Exists(filePath) Then
            _SpreadSheet = ExcelDocument.OpenExistingExcelDocument(filePath)
        Else
            _SpreadSheet = ExcelDocument.CreateEmptyExcelDocument(filePath)
        End If
    End Sub

    Private ReadOnly _SpreadSheet As ExcelDocument

    Public Shared Sub CreateFile(ByVal filePath As String, ByVal spieler As IEnumerable(Of Spieler), ByVal competition As Competition)
        Dim s = spieler.ToList
        s.Sort(New ExportComparer(competition.SpielRegeln.SonneBornBerger, competition.SpielRegeln.SatzDifferenz))
        s.Reverse()
        Try

            Using ex = New TurnierReport(filePath)
                With ex
                    Dim RundeNr = competition.SpielRunden.Count.ToString.PadLeft(2, "0"c)
                    Dim sheet = ex._SpreadSheet.GetSheet("sp_rd" & RundeNr)
                    .WriteSpielerSheet(s, sheet)

                    Dim current = 1
                    For Each runde In competition.SpielRunden.Reverse
                        Dim currentName = current.ToString.PadLeft(2, "0"c)
                        sheet = ex._SpreadSheet.GetSheet("erg_rd" & currentName)
                        .WriteRunde(runde, sheet)
                        current += 1
                    Next
                    'Dim ss = ._SpreadSheet.GetSheetFromWorksheet(sheet)
                    '._SpreadSheet.SetActiveSheet(ss.SheetId)
                End With
            End Using

        Catch ex As IOException
            Throw New Exception("Excel Datei konnte nicht geschrieben werden.", ex)
        End Try

    End Sub

    Private Sub WriteSpielerSheet(ByVal spieler As IEnumerable(Of Spieler), ByVal sheet As Worksheet)
        Dim Titles = {"Rang", "Vorname", "Nachname", "ID", "Geschlecht", "Geburtsjahr", "Verein", "TTRating", "Punkte", "Buchholzpunkte", "SonnebornBergerpunkte",
                          "Gewonnene Sätze", "Verlorene Sätze", "Ausgeschieden", "Gegnerprofil"}


        Dim SheetData = sheet.GetFirstChild(Of SheetData)()
        SheetData.RemoveAllChildren(Of Row)()

        _SpreadSheet.CreateRow(SheetData, 1, Titles)

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

            Dim Werte = {(current - 1).ToString, s.Vorname, s.Nachname, s.Id.ToString, Geschlecht(), s.Geburtsjahr.ToString, s.Vereinsname, s.TTRating.ToString,
                         s.ExportPunkte.ToString, s.ExportBHZ.ToString, s.ExportSonneborn.ToString, s.ExportSätzeGewonnen.ToString,
                         s.ExportSätzeVerloren.ToString, s.Ausgeschieden.ToString}.Concat(gegnerProfil)
            _SpreadSheet.CreateRow(SheetData, current, Werte)
            current += 1UI
        Next

    End Sub

    Private Sub WriteRunde(ByVal spielrunde As SpielRunde, ByVal worksheet As Worksheet)
        Dim SheetData = worksheet.GetFirstChild(Of SheetData)()
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

    End Sub


    Private Class ExportComparer
        Implements IComparer(Of Spieler)

        Private ReadOnly _Sonneborn As Boolean
        Private ReadOnly _satzDifferenz As Boolean

        Public Sub New(sonneBornBerger As Boolean, satzDifferenz As Boolean)
            _Sonneborn = sonneBornBerger
            _satzDifferenz = satzDifferenz
        End Sub

        Public Function Compare(myself As Spieler, other As Spieler) As Integer Implements IComparer(Of Spieler).Compare
            Dim diff = 0
            diff = other.Ausgeschieden.CompareTo(myself.Ausgeschieden)
            If diff <> 0 Then Return diff
            diff = myself.ExportPunkte - other.ExportPunkte
            If diff <> 0 Then Return diff
            diff = myself.ExportBHZ - other.ExportBHZ
            If diff <> 0 Then Return diff

            If _Sonneborn Then
                diff = myself.ExportSonneborn - other.ExportSonneborn
                If diff <> 0 Then Return diff
            End If

            If _satzDifferenz Then
                diff = (myself.ExportSätzeGewonnen - myself.ExportSätzeVerloren) - (other.ExportSätzeGewonnen - other.ExportSätzeVerloren)
                If diff <> 0 Then Return diff
            End If
            diff = myself.TTRating - other.TTRating
            If diff <> 0 Then Return diff
            diff = myself.TTRMatchCount - other.TTRMatchCount
            If diff <> 0 Then Return diff
            diff = other.Nachname.CompareTo(myself.Nachname)
            If diff <> 0 Then Return diff
            diff = other.Vorname.CompareTo(myself.Vorname)
            If diff <> 0 Then Return diff
            Return myself.Lizenznummer - other.Lizenznummer
        End Function
    End Class


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
