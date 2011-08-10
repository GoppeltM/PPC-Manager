Public Class LadenNeu

    Public Property SpeicherPfad As String

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click        
        Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        With SpeichernDialog()
            If .ShowDialog Then
                SpeicherPfad = .FileName
                DialogResult = True
                Close()
            End If
        End With
        
    End Sub

    Public Property Canceled As Boolean

    Public Shared Function SpeichernDialog() As SaveFileDialog
        Dim dialog As New SaveFileDialog
        With dialog
            .Filter = "XML Dateien |*.xml"
            If My.Settings.LetztesVerzeichnis = String.Empty Then
                .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            Else
                .InitialDirectory = My.Settings.LetztesVerzeichnis
            End If

        End With
        Return dialog
    End Function

    Public Shared Function LadenDialog() As OpenFileDialog
        Dim dialog = New OpenFileDialog
        With dialog
            .Filter = "XML Dateien |*.xml"
            If My.Settings.LetztesVerzeichnis = String.Empty Then
                .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            Else
                .InitialDirectory = My.Settings.LetztesVerzeichnis
            End If

        End With
        Return dialog
    End Function

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button3.Click
        Canceled = True
    End Sub
End Class
