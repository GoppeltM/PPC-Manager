Imports System.IO
Imports PPC_Manager

Public Class TurnierReport
    Implements IDisposable
    Implements ITurnierReport

    Public Sub New(dokument As IExcelDokument, spielstand As ISpielstand)
        _SpreadSheet = dokument
        _Spielstand = spielstand
    End Sub

    Private ReadOnly _SpreadSheet As IExcelDokument

    Public Sub SchreibeRangliste(ByVal spieler As IEnumerable(Of ExportSpieler), rundeNr As Integer) Implements ITurnierReport.SchreibeRangliste
        Dim rundeString = rundeNr.ToString.PadLeft(2, "0"c)
        Dim sheetName = "sp_rd" & rundeString
        Dim Titles = {"Rang", "Vorname", "Nachname", "ID", "Geschlecht", "Geburtsjahr", "Verein", "TTRating", "Punkte", "Buchholzpunkte", "SonnebornBergerpunkte",
                          "Gewonnene Sätze", "Verlorene Sätze", "Ausgeschieden", "Gegnerprofil"}

        _SpreadSheet.LeereBlatt(sheetName)
        _SpreadSheet.NeueZeile(sheetName, 1, Titles)

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
            _SpreadSheet.NeueZeile(sheetName, current, Werte)
            current += 1UI
        Next
        _SpreadSheet.Speichern()
    End Sub

    Public Sub SchreibeNeuePartien(spielPartien As IEnumerable(Of SpielPartie), runde As Integer) Implements ITurnierReport.SchreibeNeuePartien
        Dim currentName = runde.ToString.PadLeft(2, "0"c)
        _SpreadSheet.LeereBlatt(currentName)
        Dim Titles = {"Linker Spieler", "Rechter Spieler", "Sätze Links", "Sätze Rechts"}
        _SpreadSheet.NeueZeile(currentName, 1, Titles)

        Dim current = 2UI

        For Each ergebnis In spielPartien
            Dim Werte = {ergebnis.SpielerLinks.Id, ergebnis.SpielerRechts.Id,
                _Spielstand.MeineGewonnenenSätze(ergebnis, ergebnis.SpielerLinks).ToString,
                         _Spielstand.MeineGewonnenenSätze(ergebnis, ergebnis.SpielerRechts).ToString}
            _SpreadSheet.NeueZeile(currentName, current, Werte)
            current += 1UI
        Next
        _SpreadSheet.Speichern()
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls
    Private ReadOnly _Spielstand As ISpielstand

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
