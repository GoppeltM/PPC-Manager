Imports PPC_Manager

Public Interface IController

    ReadOnly Property AktiveCompetition As Competition
    ReadOnly Property HatRunden As Boolean
    ReadOnly Property ExcelPfad As String
    Sub RundeVerwerfen()

    Function NächsteRunde_CanExecute() As Boolean

    Sub NächsteRunde_Execute()

    Sub NächstesPlayoff_Execute()

    Sub RundenbeginnDrucken(printdialog As IPrinter)

    Sub RundenendeDrucken(p As IPrinter)

    Sub ExcelExportieren(p1 As String)

    Sub SaveXML()

    Sub SaveExcel()

    Sub NeuePartie(rundenName As String, spielerA As SpielerInfo, SpielerB As SpielerInfo)
    Sub SpielerAusscheiden(spieler As SpielerInfo)
End Interface
