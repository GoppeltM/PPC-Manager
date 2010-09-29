Imports System.Collections.ObjectModel

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

End Class

Public Class SpielPartien
    Inherits ObservableCollection(Of SpielPartie)

    Public Sub New()        
        ' TODO: für Testzwecke
        Dim x As New SpielPartie(New Spieler, New Spieler, 0)
        x.SpielerLinks.Nachname = "Zylka"
        x.Sätze.Add(New Satz With {.PunkteLinks = 3, .PunkteRechts = 21})
        x.Sätze.Add(New Satz With {.PunkteLinks = 21, .PunkteRechts = 4})
        Add(x)
        x = New SpielPartie(New Spieler, New Spieler, 0)
        x.Sätze.Add(New Satz With {.PunkteLinks = 3, .PunkteRechts = 21})
        x.Sätze.Add(New Satz With {.PunkteLinks = 5, .PunkteRechts = 4})
        Add(x)
    End Sub

    Private Property AktuelleRunde = 0

    Public Sub PaarungenBerechnen(ByVal spielerListe As SpielerListe)
        Me.Clear()
        For Each SpielPartie In Berechnen(spielerListe)
            Add(SpielPartie)
        Next
    End Sub

    Public Sub ErgebnisseEintragen()
        For Each begegnung In Me
            begegnung.SpielerLinks.GespieltePartien.Add(begegnung)
            begegnung.SpielerRechts.GespieltePartien.Add(begegnung)
        Next
    End Sub

    Private Function Berechnen(ByVal spielerListe As SpielerListe) As IList(Of SpielPartie)
        Dim list As New List(Of SpielPartie)
        For i = 0 To spielerListe.Count - 2 Step 2
            Dim partie As New SpielPartie(spielerListe(i), spielerListe(i + 1), AktuelleRunde)
            list.Add(partie)
        Next
        Return list
    End Function

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
