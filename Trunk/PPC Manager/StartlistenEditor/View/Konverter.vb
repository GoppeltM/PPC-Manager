Imports System.Globalization

Public Class OKConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim e = CType(values(0), IEnumerable)
        If Not e.GetEnumerator.MoveNext Then
            Return False
        End If
        If String.IsNullOrWhiteSpace(CType(values(1), String)) Then
            Return False
        End If
        Return True
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class