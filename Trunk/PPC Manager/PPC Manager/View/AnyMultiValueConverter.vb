Imports System.Globalization

Public Class AnyMultiValueConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        Return values.OfType(Of Boolean).Any(Function(x) x = True)
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class