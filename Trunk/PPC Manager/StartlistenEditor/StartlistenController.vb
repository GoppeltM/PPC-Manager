Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports StartlistenEditor
''' <summary>
''' Enthält alle Operationen die auf die Datenschicht einwirken
''' </summary>
''' <remarks></remarks>
Public Class StartlistenController
    Inherits ObservableCollection(Of SpielerInfo)

    Public Sub New(init As IEnumerable(Of SpielerInfo))
        For Each el In init
            Add(el)
        Next
    End Sub

    Protected Overrides Sub InsertItem(index As Integer, item As SpielerInfo)
        If item.LizenzNr = "" Then
            Dim Lizenznummern = (From x In Me Select x.LizenzNr).ToList
            Dim NeueLizenzNummer = -1
            While Lizenznummern.Contains(NeueLizenzNummer & "")
                NeueLizenzNummer -= 1
            End While
            item.LizenzNr = NeueLizenzNummer & ""
            item.ID = "PLAYER" & NeueLizenzNummer
        End If
        MyBase.InsertItem(index, item)
    End Sub

End Class
