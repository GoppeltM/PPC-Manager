Public Interface IController

    ReadOnly Property AktiveCompetition As Competition

    Sub RundeVerwerfen()

    Function NächsteRunde_CanExecute() As Boolean

    Sub NächsteRunde_Execute()

    Sub NächstesPlayoff_Execute()

    Sub RundenbeginnDrucken(printdialog As PrintDialog)

    Sub RundenendeDrucken(p As PrintDialog)

    Sub ExcelExportieren(p1 As String)

    Sub Save()

End Interface
