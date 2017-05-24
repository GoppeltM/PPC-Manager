Imports PPC_Manager

Public Interface IController

    ReadOnly Property ExcelPfad As String
    Function NächsteRunde(rundenName As String) As SpielRunde
    Sub DruckeSchiedsrichterzettel(drucker As IPrinter)
    Sub DruckeNeuePaarungen(drucker As IPrinter)
    Sub DruckeRangliste(drucker As IPrinter)
    Sub DruckeSpielergebnisse(drucker As IPrinter)
    Sub ExcelExportieren(p1 As String)
    Sub SaveXML()
    Sub SaveExcel()
End Interface
