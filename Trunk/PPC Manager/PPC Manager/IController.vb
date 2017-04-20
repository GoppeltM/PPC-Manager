Imports PPC_Manager

Public Interface IController

    ReadOnly Property ExcelPfad As String
    Sub NächsteRunde()
    Sub NächstesPlayoff()
    Sub RundenbeginnDrucken(printdialog As IPrinter)
    Sub RundenendeDrucken(p As IPrinter)
    Sub ExcelExportieren(p1 As String)
    Sub SaveXML()
    Sub SaveExcel()
    Sub NeuePartie(rundenName As String, spielerA As SpielerInfo, SpielerB As SpielerInfo)
    Sub SpielerAusscheiden(spieler As SpielerInfo)
End Interface
