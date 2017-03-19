Public Interface IReportFactory
    Sub AutoSave()
    Sub SchreibeReport(filePath As String)
    Sub IstBereit()
    ReadOnly Property ExcelPfad As String
End Interface
