Public Class FremdSpielerDialog

    Private ReadOnly _Spieler As SpielerInfo

    Public Sub New(klassementListe As IEnumerable(Of String),
                   spieler As SpielerInfo)
        InitializeComponent()
        _Spieler = spieler
        Me.Vorname.Text = spieler.Vorname
        Me.Nachname.Text = spieler.Nachname
        Me.Verein.Text = spieler.Verein
        Me.TTR.Text = spieler.TTR.ToString
        Me.TTRMatchCount.Text = spieler.TTRMatchCount.ToString
        Me.Geschlecht.IsChecked = CType(spieler.Geschlecht, Boolean?)
        Me.Geburtsjahr.Text = spieler.Geburtsjahr.ToString
        For Each cat In klassementListe
            Klassements.Items.Add(cat)
        Next
        Klassements.SelectedItem = spieler.Klassement
    End Sub


    Public Function EditierterSpieler() As SpielerInfo

        _Spieler.Fremd = True
        _Spieler.Vorname = Vorname.Text
        _Spieler.Nachname = Nachname.Text
        _Spieler.Verein = Verein.Text
        _Spieler.TTR = CInt(TTR.Text)
        _Spieler.TTRMatchCount = CInt(TTRMatchCount.Text)
        _Spieler.Geschlecht = If(Geschlecht.IsChecked, 1, 0)
        _Spieler.Geburtsjahr = CInt(Geburtsjahr.Text)
        _Spieler.Klassement = Klassements.SelectedValue.ToString

        Return _Spieler
    End Function

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = True
        Me.Close()
    End Sub

End Class

Public Class IsSelectedConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return DirectCast(value, Integer) <> -1
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class
