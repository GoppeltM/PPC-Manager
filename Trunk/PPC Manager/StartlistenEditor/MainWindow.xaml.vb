Imports Microsoft.Win32

Class MainWindow

    Private Doc As XDocument

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        With New OpenFileDialog
            .Filter = "Click-TT Turnierdaten|*.xml"
            If Not .ShowDialog Then
                Application.Current.Shutdown()
                Return
            End If

            Doc = XDocument.Load(.FileName)
        End With

        Dim SpielerListe = From competition In Doc.Root.<competition>
                           From Spieler In competition...<person>
                           Group Spieler, competition By Spieler.Attribute("internal-nr").Value Into Group
                           Select Group

        Dim AlleSpieler As New List(Of Spieler)

        For Each s In SpielerListe

            Dim SpielerKnoten = From x In s Select x.Spieler

            Dim Competitions = From x In s Select x.competition.Attribute("age-group").Value
            AlleSpieler.Add(Spieler.FromXML(SpielerKnoten, Competitions))
        Next



    End Sub
End Class
