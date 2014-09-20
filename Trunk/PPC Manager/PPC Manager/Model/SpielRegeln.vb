Public Class SpielRegeln


    Private ReadOnly _Gewinnsätze As Integer
    Private ReadOnly _SatzDifferenz As Boolean
    Private ReadOnly _SonneBornBerger As Boolean
    
    Public Sub New(gewinnSätze As Double?, satzDifferenz As Boolean?, sonneBornBerger As Boolean?)
        _Gewinnsätze = Convert.ToInt32(gewinnSätze)
        _SatzDifferenz = Convert.ToBoolean(satzDifferenz)
        _SonneBornBerger = Convert.ToBoolean(sonneBornBerger)
    End Sub

    ReadOnly Property Gewinnsätze As Integer
        Get
            Return _Gewinnsätze
        End Get
    End Property
    ReadOnly Property SatzDifferenz As Boolean
        Get
            Return _SatzDifferenz
        End Get
    End Property
    ReadOnly Property SonneBornBerger As Boolean
        Get
            Return _SonneBornBerger
        End Get
    End Property

End Class
