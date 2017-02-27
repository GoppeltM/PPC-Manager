''' <summary>
''' Enthält alle Operationen die auf die Datenschicht einwirken
''' </summary>
''' <remarks></remarks>
Public Class StartlistenController
    Implements ICollection(Of SpielerInfo)

    Private ReadOnly _SpielerListe As ICollection(Of SpielerInfo)

    Public Sub New(spielerListe As ICollection(Of SpielerInfo))
        _SpielerListe = spielerListe
    End Sub

    Public ReadOnly Property Count As Integer Implements ICollection(Of SpielerInfo).Count
        Get
            Return _SpielerListe.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of SpielerInfo).IsReadOnly
        Get
            Return _SpielerListe.IsReadOnly
        End Get
    End Property

    Public Sub Add(item As SpielerInfo) Implements ICollection(Of SpielerInfo).Add
        If item.LizenzNr = 0 Then
            Dim Lizenznummern = (From x In _SpielerListe Select x.LizenzNr).ToList
            Dim NeueLizenzNummer = -1
            While Lizenznummern.Contains(NeueLizenzNummer)
                NeueLizenzNummer -= 1
            End While
            item.LizenzNr = NeueLizenzNummer
            item.ID = "PLAYER" & NeueLizenzNummer
        End If
        _SpielerListe.Add(item)
    End Sub

    Public Sub Clear() Implements ICollection(Of SpielerInfo).Clear
        _SpielerListe.Clear()
    End Sub

    Public Function Contains(item As SpielerInfo) As Boolean Implements ICollection(Of SpielerInfo).Contains
        Return _SpielerListe.Contains(item)
    End Function

    Public Sub CopyTo(array() As SpielerInfo, arrayIndex As Integer) Implements ICollection(Of SpielerInfo).CopyTo
        _SpielerListe.CopyTo(array, arrayIndex)
    End Sub

    Public Function Remove(item As SpielerInfo) As Boolean Implements ICollection(Of SpielerInfo).Remove
        Return _SpielerListe.Remove(item)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of SpielerInfo) Implements IEnumerable(Of SpielerInfo).GetEnumerator
        Return _SpielerListe.GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return _SpielerListe.GetEnumerator()
    End Function
End Class
