
Public Class PaarungsContainer(Of T)

    Public Property Übrig As T

    Public Property Partien As List(Of Tuple(Of T, T)) = New List(Of Tuple(Of T, T))()
End Class
