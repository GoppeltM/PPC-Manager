Public Class ExcelNichtBeschreibbarException
    Inherits Exception

End Class

Public Class SpielDatenUnvollständigException
    Inherits Exception

    Public Sub New(unvollständigCount As Integer)
        Me.UnvollständigCount = unvollständigCount
    End Sub

    Public ReadOnly UnvollständigCount As Integer

End Class