Imports System.Globalization

Public Class GeschlechtKonverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim geschlecht = DirectCast(value, Integer)
        Select Case geschlecht
            Case 0 : Return "w"
            Case 1 : Return "m"
        End Select
        Throw New ArgumentException("Unbekanntes geschlecht: " & geschlecht)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class

Public Class SpielKlassenkonverter
    Implements IValueConverter

    Private _AktuellesJahr As Integer

    Public Sub New()
        _AktuellesJahr = Date.Now.Year
    End Sub

    Public Sub New(aktuellesJahr As Integer)
        _AktuellesJahr = aktuellesJahr
    End Sub

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim Geburtsjahr = DirectCast(value, Integer)

        Select Case _AktuellesJahr - Geburtsjahr
            Case Is < 13 : Return "u13"
            Case Is < 15 : Return "u15"
            Case Is < 18 : Return "u18"
            Case Else : Return "Ü18"
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class


Public Class AusgeschiedenPainter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If Not targetType Is GetType(Brush) Then
            Throw New Exception("Must be a brush!")
        End If

        Dim val = CType(value, Boolean)
        If val Then
            Return Brushes.Bisque
        Else
            Return Brushes.Transparent
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class
Class HintergrundLinksKonverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        With DirectCast(value, Satz)
            If .PunkteLinks >= 11 AndAlso .PunkteLinks > .PunkteRechts Then
                Return Brushes.Yellow
            Else
                Return Brushes.Transparent
            End If
        End With
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class

Class HintergrundRechtsKonverter
    Implements IValueConverter


    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        With DirectCast(value, Satz)
            If .PunkteRechts >= 11 AndAlso .PunkteRechts > .PunkteLinks Then
                Return Brushes.Yellow
            Else
                Return Brushes.Transparent
            End If
        End With
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class

Public Class MeineGewonnenenSätze
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim points = CType(value, Satz)
        Select Case parameter.ToString
            Case "L"
                If points.PunkteLinks >= My.Settings.GewinnPunkte AndAlso points.PunkteLinks > points.PunkteRechts Then
                    Return Visibility.Visible
                End If
            Case "R"
                If points.PunkteRechts >= My.Settings.GewinnPunkte AndAlso points.PunkteRechts > points.PunkteLinks Then
                    Return Visibility.Visible
                End If
        End Select
        Return Visibility.Hidden
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class

Public Class OpacityConverter
    Implements IValueConverter

    Public Property IstAusgeschieden As Predicate(Of SpielPartie)

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If DirectCast(value, Boolean) Then
            Return 0.2
        Else
            Return 1.0
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class

Public Class GewonneneSätzeConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim partie = TryCast(value, SpielPartie)
        If partie Is Nothing Then Return Nothing

        Dim gewonnenLinks = partie.MeineGewonnenenSätze(partie.SpielerLinks)
        Dim gewonnenRechts = partie.MeineGewonnenenSätze(partie.SpielerRechts)

        Return String.Format("{0}:{1}", gewonnenLinks.Count, gewonnenRechts.Count)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class
