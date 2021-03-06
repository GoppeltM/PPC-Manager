﻿Imports System.Collections.ObjectModel

Public Class StartlisteSeite

    Public Sub New(spielerListe As IEnumerable(Of SpielerInfo))

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        Dim res = CType(FindResource("Spieler"), SpielerListe)
        For Each s In spielerListe
            res.Add(s)
        Next
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub

    Public Function GetMaxItemCount() As Integer
        Dim width = SpielerRangListe.ActualWidth
        Dim height = SpielerRangListe.ActualHeight - 70 ' Row Headers ausgenommen

        Dim ItemHeight = 21

        Dim Rows = CInt(height) \ CInt(ItemHeight)

        If Rows = 0 Then
            Throw New Exception("Seitenformat zu klein! Kann kein Element darauf vollständig drucken!")
        End If
        Return Rows
    End Function

End Class

Public Class SpielerListe
    Inherits ObservableCollection(Of SpielerInfo)

    Public Sub New()

    End Sub

End Class

Class DesignSpielerListe
    Inherits SpielerListe

    Sub New()
        Add(New SpielerInfo)
    End Sub
End Class


Public Class IndexConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If Not TypeOf value Is SpielerInfo Then Return ""
        Dim spieler = CType(value, SpielerInfo)
        Dim liste = CType(parameter, IEnumerable(Of SpielerInfo))

        Return liste.ToList.IndexOf(spieler) + 1
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class
