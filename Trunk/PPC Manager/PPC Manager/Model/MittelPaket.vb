
Class MittelPaket
    Inherits Paket

    Sub New(ByVal aktuelleRunde As Integer)
        ' TODO: Complete member initialization 
        MyBase.New(aktuelleRunde)
    End Sub

    Sub New(ByVal mittelPaket As MittelPaket)
        MyBase.New(mittelPaket)
    End Sub

    Property aktuellerSchwimmer As Object

    Sub ÜbernimmPaarungen(ByVal vorgänger As Paket)
        Throw New NotImplementedException
    End Sub



End Class
