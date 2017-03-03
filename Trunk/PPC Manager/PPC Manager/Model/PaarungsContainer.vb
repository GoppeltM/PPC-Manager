
Public Class PaarungsContainer(Of T)

    Public Property aktuellerSchwimmer As T

    Public Property Partien As List(Of Tuple(Of T, T))

    Public Property SpielerListe As List(Of T)
End Class
