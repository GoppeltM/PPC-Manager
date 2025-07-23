

Public Class UrkundeManuellDialog

    Public Sub New()
        InitializeComponent()

        Vorschau.Datum1.Text = Date.Today.ToShortDateString()
        Vorschau.Datum2.Text = Date.Today.ToLongDateString()

        Try
            Dim app = CType(Application.Current, Application)
            Dim liste = app.AktiveCompetition.SpielerListe.ToList
            liste.Sort(CType(CType(app.MainWindow, MainWindow)._Comparer, IComparer(Of SpielerInfo)))
            liste.Reverse()
            Spielerliste.ItemsSource = liste

        Catch ex As SpielDatenUnvollständigException
        End Try

    End Sub

    Private Sub txtName_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtName.TextChanged
        Vorschau.SpielerName.Text = txtName.Text
    End Sub

    Private Sub txtVereinsname_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtVereinsname.TextChanged
        Vorschau.Vereinsname.Text = txtVereinsname.Text
    End Sub

    Private Sub txtPlatz_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtPlatz.TextChanged
        Vorschau.Platzierung.Text = "zum " & txtPlatz.Text & ". Platz"
    End Sub

    Private Sub txtWettbewerbname_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtWettbewerbname.TextChanged
        Vorschau.WettbName.Text = "im Wettbewerb " & txtWettbewerbname.Text
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim Urkunde = New Urkunde
        Urkunde.SpielerName.Text = txtName.Text
        Urkunde.Vereinsname.Text = txtVereinsname.Text
        Urkunde.Platzierung.Text = "zum " & txtPlatz.Text & ". Platz"
        Urkunde.WettbName.Text = "im Wettbewerb " & txtWettbewerbname.Text

        Urkunde.Datum1.Text = Date.Today.ToShortDateString()
        Urkunde.Datum2.Text = Date.Today.ToLongDateString()

        Dim p = New PrintDialog
        If p.ShowDialog() Then
            p.PrintVisual(Urkunde, "Urkunde")
        End If
    End Sub

    Private Sub Spielerliste_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Spielerliste.SelectionChanged
        Dim spieler = CType(Spielerliste.SelectedValue, SpielerInfo)

        With spieler
            txtName.Text = .Vorname & " " & .Nachname

            txtVereinsname.Text = .Vereinsname

            txtPlatz.Text = (Spielerliste.SelectedIndex + 1).ToString

            txtWettbewerbname.Text = CType(Application.Current, Application).competition

        End With

    End Sub
End Class
