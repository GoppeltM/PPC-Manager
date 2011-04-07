Imports <xmlns="http://PPCManager/SpeicherStand">

Public Class Optionen


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        My.Settings.Vereine.Clear()
        For Each verein As Verein In CType(Resources.Item("Vereine"), VereinsDaten)
            My.Settings.Vereine.Add(verein.Vereinsname)
        Next
        My.Settings.Save()
        Me.Close()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button4.Click
        Dim dialog = New OpenFileDialog
        dialog.DefaultExt = "xml"
        dialog.Filter = "XML Dateien|*.xml"

        If dialog.ShowDialog Then
            Dim xml = XDocument.Load(dialog.FileName).Root

            Dim AktiveSpieler = CType(FindResource("AktiveSpieler"), SpielerListe)
            AktiveSpieler.FromXML(xml.<AktiveSpieler>)
            Dim AusgeschiedeneSpieler = CType(FindResource("AusgeschiedeneSpieler"), SpielerListe)
            AusgeschiedeneSpieler.FromXML(xml.<AusgeschiedeneSpieler>)
            Dim SpielRunden = CType(FindResource("MySpielRunden"), SpielRunden)
            SpielRunden.fromXML(xml.<SpielRunden>)

        End If


    End Sub
End Class

Friend Class VereinsDaten
    Inherits ObjectModel.ObservableCollection(Of Verein)

    Public Sub New()
        For Each verein As String In My.Settings.Vereine
            Me.Add(New Verein With {.Vereinsname = verein})
        Next
    End Sub
End Class

Friend Class Verein
    Public Property Vereinsname As String
End Class

Public Class SatzZahlConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If CType(value, Integer) = 1 Then
            Return "Ein Gewinnsatz"
        End If
        Return String.Format("{0} Gewinnsätze", value)
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class