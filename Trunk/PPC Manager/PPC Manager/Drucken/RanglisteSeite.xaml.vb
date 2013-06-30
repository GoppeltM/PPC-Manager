Public Class RanglisteSeite
    Implements IPaginatibleUserControl


    Public Function GetMaxItemCount() As Integer Implements IPaginatibleUserControl.GetMaxItemCount
        Dim width = PageContent.ActualWidth
        Dim height = PageContent.ActualHeight

        Dim ItemHeight = 20

        Dim Rows = CInt(height - 25) \ CInt(ItemHeight)

        If Rows = 0 Then
            Throw New Exception("Seitenformat zu klein! Kann kein Element darauf vollständig drucken!")
        End If
        Return Rows
    End Function

    Public Sub SetSource(ByVal elements As IEnumerable(Of Object)) Implements IPaginatibleUserControl.SetSource
        Dim res = CType(FindResource("Spieler"), SpielerListe)
        For Each s In elements.OfType(Of Spieler)()
            res.Add(s)
        Next

    End Sub
End Class


Public Class IndexConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If Not TypeOf value Is Spieler Then Return ""
        Dim spieler = CType(value, Spieler)
        Dim liste = CType(parameter, IEnumerable(Of Spieler))

        Return liste.ToList.IndexOf(spieler) + 1
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class
