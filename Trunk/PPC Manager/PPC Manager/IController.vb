Public Interface IController

    ReadOnly Property AktiveCompetition As Competition

    Sub RundeVerwerfen()

    Function NächsteRunde_CanExecute() As Boolean

    Sub NächsteRunde_Execute()

    Sub NächstesPlayoff_Execute()

    Sub RundenbeginnDrucken(printdialog As IPrinter)

    Sub RundenendeDrucken(p As IPrinter)

    Sub ExcelExportieren(p1 As String)

    Sub SaveXML()

    Sub SaveExcel()

    Sub SatzEintragen(value As Integer, inverted As Boolean, partie As SpielPartie)

    Function NeuerSatz_CanExecute(s As SpielPartie) As Boolean

    Function FilterSpieler(s As Spieler) As Boolean

    Sub NeuePartie(rundenName As String, spielerA As Spieler, SpielerB As Spieler)

End Interface
