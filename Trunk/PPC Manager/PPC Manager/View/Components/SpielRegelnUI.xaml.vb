
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class SpielRegelnUI

    Private Sub SaveRules()
        Dim app = CType(Application.Current, Application)
        Dim doc = app.doc
        Dim comp = app.competition
        Dim XMLPath = app.xmlPfad

        If GewinnsätzeAnzahl Is Nothing OrElse
                SatzDiffCheck Is Nothing OrElse
                SonneBorn Is Nothing Then Return

        If comp <> "" Then
            Dim competition = (From x In doc.Root.<competition>
                               Where x.Attribute("ttr-remarks").Value = comp Select x).Single
            competition.@ppc:gewinnsätze = GewinnsätzeAnzahl.Value.ToString.ToLower
            competition.@ppc:satzdifferenz = SatzDiffCheck.IsChecked.ToString.ToLower
            competition.@ppc:sonnebornberger = SonneBorn.IsChecked.ToString.ToLower

            doc.Save(XMLPath)
        End If

    End Sub

    Private Sub SonneBorn_Checked(sender As Object, e As RoutedEventArgs) Handles SonneBorn.Click
        SaveRules()
    End Sub

    Private Sub SatzDiffCheck_Checked(sender As Object, e As RoutedEventArgs) Handles SatzDiffCheck.Click
        SaveRules()
    End Sub

    Private Sub GewinnsätzeAnzahl_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        SaveRules()
    End Sub
End Class
