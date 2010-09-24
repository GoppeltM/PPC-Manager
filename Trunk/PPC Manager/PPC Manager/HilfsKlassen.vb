Imports System.Collections.ObjectModel

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

End Class

Public Class SpielPartien
    Inherits ObservableCollection(Of SpielPartie)

    Public Sub New()
        ' TODO: für Testzwecke
        Dim x As New SpielPartie
        x.Sätze.Add(New Satz With {.PunkteLinks = 3, .PunkteRechts = 21})
        x.Sätze.Add(New Satz With {.PunkteLinks = 21, .PunkteRechts = 4})
        Add(x)
        x = New SpielPartie
        x.Sätze.Add(New Satz With {.PunkteLinks = 3, .PunkteRechts = 21})
        x.Sätze.Add(New Satz With {.PunkteLinks = 5, .PunkteRechts = 4})
        Add(x)
    End Sub

End Class


Public Class SatzFarbenPainter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert        

        If Not targetType Is GetType(Brush) Then
            Throw New Exception("Must be a brush!")
        End If

        Dim x As Integer = value
        If x >= My.Settings.GewinnPunkte Then
            Return Brushes.GreenYellow
        Else
            Return Brushes.Transparent
        End If

    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class