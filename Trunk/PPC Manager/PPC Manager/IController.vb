Imports PPC_Manager

Public Interface IController

    ReadOnly Property ExcelPfad As String
    Function NächsteRunde(rundenName As String) As SpielRunde
    Sub RundenbeginnDrucken(printdialog As IPrinter)
    Sub RundenendeDrucken(p As IPrinter)
    Sub ExcelExportieren(p1 As String)
    Sub SaveXML()
    Sub SaveExcel()
End Interface
