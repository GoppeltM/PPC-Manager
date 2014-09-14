Imports System.Globalization

Public Class SpielPartienListe
    Private Sub SpielPartie_Löschen_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        Dim PlayOffAktiv = DirectCast(FindResource("PlayoffAktiv"), Boolean)
        e.CanExecute = PlayOffAktiv AndAlso SpielPartienView.SelectedItem IsNot Nothing
    End Sub

    Private Sub SpielPartie_Löschen_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim AktuelleRunde = MainWindow.AktiveCompetition.SpielRunden.Peek()
        Dim SelektierteSpielpartie = DirectCast(SpielPartienView.SelectedItem, SpielPartie)
        AktuelleRunde.Remove(SelektierteSpielpartie)
    End Sub
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