Imports StartlistenEditor

Public Class SpeicherCache
    Implements ISpeicher

    Private ReadOnly _Speicher As ISpeicher
    Private ReadOnly _KlassementNamen As Lazy(Of IEnumerable(Of String))
    Private ReadOnly _Speicheraufrufe As New List(Of Veränderung)
    Public Sub New(speicher As ISpeicher)
        _Speicher = speicher
        _KlassementNamen = New Lazy(Of IEnumerable(Of String))(Function() _Speicher.KlassementNamen)
    End Sub

    Public ReadOnly Property KlassementNamen As IEnumerable(Of String) Implements ISpeicher.KlassementNamen
        Get
            Return _KlassementNamen.Value
        End Get
    End Property

    Public Sub Speichere(veränderung As Veränderung) Implements ISpeicher.Speichere
        _Speicheraufrufe.Add(veränderung)
    End Sub

    Public Sub SpeichereAlles()
        Dim action = Sub(x As IEnumerable(Of XElement))
                         For Each v In _Speicheraufrufe
                             v(x)
                         Next
                     End Sub
        _Speicher.Speichere(action)
    End Sub
End Class
