Imports PPC_Manager

Friend Class DruckerFabrik
    Implements IDruckerFabrik

    Public Function Neu(einstellungen As PrintDialog) As IPrinter Implements IDruckerFabrik.Neu
        Return New Printer(einstellungen)
    End Function
End Class
