﻿Public Interface IExcelDokument
    Inherits IDisposable

    Sub LeereBlatt(sheetName As String)
    Sub NeueZeile(sheetName As String, Zellen As IEnumerable(Of String))
    Sub Speichern()
End Interface
