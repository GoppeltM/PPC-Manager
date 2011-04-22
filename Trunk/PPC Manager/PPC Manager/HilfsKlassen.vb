Imports System.Collections.ObjectModel
Imports <xmlns="http://PPCManager/SpeicherStand">

Friend Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

    Sub FromXML(ByVal spielRunden As SpielRunden, ByVal xSpielerListe As IEnumerable(Of XElement))
        Clear()
        For Each xSpieler In xSpielerListe
            Add(Spieler.FromXML(spielRunden, xSpieler))
        Next
    End Sub

End Class

Friend Class SpielRunden
    Inherits Stack(Of SpielRunde)
    
    Friend Sub FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal runden As IEnumerable(Of XElement))
        Clear()
        For Each xRunde In From x In runden Order By Integer.Parse(x.@Nummer)
            Push(SpielRunde.FromXML(spielerListe, xRunde))
        Next
    End Sub

    Friend Function ToXML() As XElement

        Dim xSpielRunden As New List(Of XElement)
        For i = 1 To Me.Count
            xSpielRunden.Add(<SpielRunde Nummer=<%= i %>>
                                 <%= From x In Me(i) Let y = x.ToXML() Select y %>
                             </SpielRunde>)
        Next

        Return <SpielRunden>
                   <%= xSpielRunden %>
               </SpielRunden>
    End Function

End Class


Friend Class SpielRunde
    Inherits ObservableCollection(Of SpielPartie)

    
    Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xRunde As XElement) As SpielRunde
        Dim runde As New SpielRunde
        For Each xSpielPartie In xRunde.<SpielRunde>
            runde.Add(SpielPartie.FromXML(spielerListe, xSpielPartie))
        Next
        Dim xFreilos = xRunde.<FreiLosSpiel>.SingleOrDefault
        If xFreilos IsNot Nothing Then
            runde.Add(FreiLosSpiel.FromXML(spielerListe, xFreilos))
        End If        
        Return runde
    End Function

End Class


Public Class SpielPartien
    Inherits ObservableCollection(Of SpielPartie)

    Public Sub New()        
        ' TODO: für Testzwecke
        Dim x As New SpielPartie(New Spieler, New Spieler)
        x.SpielerLinks.Nachname = "Zylka"

        With x(0)
            .PunkteLinks = 3
            .PunkteRechts = 21
        End With

        With x(1)
            .PunkteLinks = 17
            .PunkteRechts = 4
        End With
        Add(x)
        x = New SpielPartie(New Spieler, New Spieler)
        Add(x)
    End Sub

    Private Property AktuelleRunde As Integer = 0

    Friend Sub PaarungenBerechnen(ByVal spielerListe As SpielerListe)
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
            Dim partie As New SpielPartie(spielerListe(i), spielerListe(i + 1))
            list.Add(partie)
        Next
        Return list
    End Function

End Class

Public Class Vereine
    Inherits Collections.ObjectModel.Collection(Of String)

End Class


Public Class SatzFarbenPainter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert        

        If Not targetType Is GetType(Brush) Then
            Throw New Exception("Must be a brush!")
        End If

        Dim x As Integer = CInt(value)
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
