Imports System.IO

Public Class ApplicationTests
    <Test, Explicit, Apartment(Threading.ApartmentState.STA)>
    Public Sub FileNotFoundException_schreibt_FusionLog()
        Dim a = New Application
        a.CrashBehandeln(Me, New UnhandledExceptionEventArgs(New FileNotFoundException("Hallo Welt", "D:\Dummy.dll"), True))

    End Sub
End Class
