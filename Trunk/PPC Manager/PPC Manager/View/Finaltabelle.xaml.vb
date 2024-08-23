Imports System.Collections.ObjectModel
Imports System.Drawing.Printing
Imports System.Globalization
Imports System.Printing
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class Finaltabelle
    Dim initialisiert As Boolean = False
    Dim competition As Competition
    Dim mode As Integer
    Dim players As IEnumerable(Of SpielerInfo)

    Public Function GetPlayers(CompetitionXML As XElement, mode As Integer) As IEnumerable(Of SpielerInfo)
        Dim playersxml = CompetitionXML.<players>.<player>

        If mode = FinalMode.Viertelfinale Then
            playersxml = playersxml.OrderBy(Function(x) x.@ppc:vpos)
        End If

        If mode = FinalMode.Halbfinale Then
            playersxml = playersxml.OrderBy(Function(x) x.@ppc:hpos)
        End If

        Return playersxml.Select(Function(x) AusXML.SpielerFromXML(x))

    End Function

    Private Sub ResetUI(_mode As Integer)
        V1.DataContext = Nothing
        V2.DataContext = Nothing
        V3.DataContext = Nothing
        V4.DataContext = Nothing
        H1.DataContext = Nothing
        H2.DataContext = Nothing
        F1.DataContext = Nothing

        DetailGrid.DataContext = Nothing


        CType(V1.Parent, UIElement).IsEnabled = True
        CType(V2.Parent, UIElement).IsEnabled = True
        CType(V3.Parent, UIElement).IsEnabled = True
        CType(V4.Parent, UIElement).IsEnabled = True

        CType(H1.Parent, UIElement).IsEnabled = False
        CType(H2.Parent, UIElement).IsEnabled = False

        CType(F1.Parent, UIElement).IsEnabled = False

        UrkundenPlatz5.IsEnabled = False
        UrkundenPlatz3.IsEnabled = False
        UrkundenFinalisten.IsEnabled = False

        If (_mode = FinalMode.Halbfinale) Then
            GlassBrush.Visibility = Visibility.Visible
            HatchBrush.Visibility = Visibility.Visible
        End If

    End Sub

    Public Sub Init(CompetitionXML As XElement, _mode As Integer, runden As SpielRunden)
        ResetUI(_mode)

        competition = CType(Application.Current, Application).AktiveCompetition
        mode = _mode
        players = GetPlayers(CompetitionXML, mode)

        If mode = FinalMode.Viertelfinale Then
            Dim runde = If(competition.SpielRunden.Count >= 2, SortiereFreiloseInViertelFinale(competition.SpielRunden.Reverse(1)), NächsteRunde())

            SetViertelFinalDataContext(runde)

            runde = If(competition.SpielRunden.Count >= 3, competition.SpielRunden.Reverse(2), Nothing)
            SetHalbFinalDataContext(runde)

            runde = If(competition.SpielRunden.Count = 4, competition.SpielRunden.Reverse(3), Nothing)
            SetFinalDataContext(runde)
        End If

        If mode = FinalMode.Halbfinale Then
            Dim runde = If(competition.SpielRunden.Count >= 2, SortiereFreiloseInHalbFinale(competition.SpielRunden.Reverse(1)), NächsteRunde())

            SetHalbFinalDataContext(runde)

            runde = If(competition.SpielRunden.Count = 3, competition.SpielRunden.Reverse(2), Nothing)
            SetFinalDataContext(runde)
        End If

        initialisiert = True
    End Sub

    Private Sub SetViertelFinalDataContext(runde As SpielRunde)
        Selectable.DeselectAll()
        V1.DataContext = runde(0)
        V2.DataContext = runde(1)
        V3.DataContext = runde(2)
        V4.DataContext = runde(3)
        CType(V1.Parent, Selectable).IsSelected = True
    End Sub

    Private Function SortiereFreiloseInViertelFinale(_runde As SpielRunde) As SpielRunde
        'Freilose werden als Spielpartie am Ende der Rundenliste angehängt.
        'Das führt zu Sortierungsfehlern beim laden der Runden aus dem XML, wenn das Playoff gestartet wurde
        'Beim starten im UI findet die Sortierung implizit in der Funktion NächsteRunde() statt, diese wird aber
        'beim Laden der Applikation nicht mehr gerufen, wenn Spielergebnisse vorhanden sind, wodurch die einzelnen Runden so sortiert sind wie im XML

        'Bestimmt gibt es ein Möglichkeit (speichern der Matches bei Initialisierung, da aktuell nur die Player ins XML geschrieben werden), das generischer oder schöner zu lösen, aber wir kennen die exakte Reihenfolge der Runden:
        'Freilose am Ende, "Echte" Partien oben, beide jeweils korrekt sortiert. Da auch die Positionen der Gegner in Runde 1 immer konstant sind: 1-(8), 4-5, 3-(6), 2-(7),
        'Können wir ohne Sortierung anhand der Spieleranzahl bzw. der Freilosanzahl die Runde sortieren

        Dim matches = _runde.Where(Function(partie) TypeOf partie IsNot FreiLosSpiel) _
                            .Concat(_runde.Where(Function(partie) TypeOf partie Is FreiLosSpiel))

        Dim runde As New SpielRunde(matches)

        If players.Count = 8 Then Return runde

        ' 1 Freilos
        If players.Count = 7 Then Return New SpielRunde From {
            runde(3),
            runde(0),
            runde(1),
            runde(2)
        }

        ' 2 Freilose
        If players.Count = 6 Then Return New SpielRunde From {
            runde(2),
            runde(0),
            runde(1),
            runde(3)
        }

        ' 3 Freilose
        Return New SpielRunde From {
           runde(1),
           runde(0),
           runde(2),
           runde(3)
        }

    End Function

    Private Function SortiereFreiloseInHalbFinale(_runde As SpielRunde) As SpielRunde
        Dim matches = _runde.Where(Function(partie) TypeOf partie Is FreiLosSpiel) _
                            .Concat(_runde.Where(Function(partie) TypeOf partie IsNot FreiLosSpiel))

        Return New SpielRunde(matches)
    End Function

    Private Sub SetHalbFinalDataContext(runde As SpielRunde)
        If runde Is Nothing Then Return
        CType(V1.Parent, Selectable).IsSelected = False
        Selectable.DeselectAll()
        H1.DataContext = runde(0)
        CType(H1.Parent, UIElement).IsEnabled = True
        CType(H1.Parent, Selectable).IsSelected = True
        H2.DataContext = runde(1)
        CType(H2.Parent, UIElement).IsEnabled = True


        CType(V1.Parent, UIElement).IsEnabled = False
        CType(V2.Parent, UIElement).IsEnabled = False
        CType(V3.Parent, UIElement).IsEnabled = False
        CType(V4.Parent, UIElement).IsEnabled = False

        UrkundenPlatz5.IsEnabled = True
    End Sub

    Private Sub SetFinalDataContext(runde As SpielRunde)
        If runde Is Nothing Then Return
        CType(H1.Parent, Selectable).IsSelected = False
        Selectable.DeselectAll()
        F1.DataContext = runde(0)
        CType(F1.Parent, UIElement).IsEnabled = True
        CType(F1.Parent, Selectable).IsSelected = True

        CType(H1.Parent, UIElement).IsEnabled = False
        CType(H2.Parent, UIElement).IsEnabled = False

        UrkundenPlatz3.IsEnabled = True

        If runde(0).IstAbgeschlossen Then
            UrkundenFinalisten.IsEnabled = True
        End If
    End Sub

    Friend Sub Update()
        If Not initialisiert Then Return

        Dim mainWindow = CType(Application.Current.MainWindow, MainWindow)

        mainWindow._Controller.SaveXML()

        If mainWindow.RundeAbgeschlossen() Then mainWindow.NächsteRunde()
    End Sub

    Private Sub Selectable_Selected(sender As Object, e As RoutedEventArgs)
        Dim partieKarte = CType(CType(sender, Selectable).Child, PartieKarte)
        Dim partie = CType(partieKarte.DataContext, SpielPartie)

        DetailGrid.DataContext = partie
    End Sub

    Friend Function NächsteRunde() As SpielRunde

        If mode = FinalMode.Viertelfinale Then
            If competition.SpielRunden.Count = 1 Then
                If players.Count > 8 OrElse players.Count < 5 Then
                    Throw New Exception("Falsche Anzahl an Spielern, 5 - 8 Spieler für Viertelfinale benötigt")
                End If

                Dim rundenName = "Viertelfinale 1"

                Dim runde = New SpielRunde From {
                    If(players.Count > 7, New SpielPartie(rundenName, players(0), players(7)), New FreiLosSpiel(rundenName, players(0))),
                    New SpielPartie(rundenName, players(3), players(4)),
                    If(players.Count > 5, New SpielPartie(rundenName, players(2), players(5)), New FreiLosSpiel(rundenName, players(2))),
                    If(players.Count > 6, New SpielPartie(rundenName, players(1), players(6)), New FreiLosSpiel(rundenName, players(1)))
                }

                SetViertelFinalDataContext(runde)

                competition.SpielRunden.Push(runde)
                Return runde
            End If

            If competition.SpielRunden.Count = 2 Then
                Dim viertelfinals = competition.SpielRunden.Peek()
                Dim runde = New SpielRunde From {
                    New SpielPartie("Halbfinale 1", viertelfinals(0).Gewinner, viertelfinals(1).Gewinner),
                    New SpielPartie("Halbfinale 1", viertelfinals(2).Gewinner, viertelfinals(3).Gewinner)
                }
                SetHalbFinalDataContext(runde)

                competition.SpielRunden.Push(runde)
                Return runde
            End If

            If competition.SpielRunden.Count = 3 Then
                Dim halbfinals = competition.SpielRunden.Peek()
                Dim runde = New SpielRunde From {
                    New SpielPartie("Finale 1", halbfinals(0).Gewinner, halbfinals(1).Gewinner)
                }
                SetFinalDataContext(runde)

                competition.SpielRunden.Push(runde)
                Return runde
            End If

            If competition.SpielRunden.Count = 4 Then
                UrkundenFinalisten.IsEnabled = True
            End If
        End If

        If mode = FinalMode.Halbfinale Then
            If competition.SpielRunden.Count = 1 Then
                If players.Count > 4 OrElse players.Count < 3 Then
                    Throw New Exception("Falsche Anzahl an Spielern, 3 oder 4 Spieler für Halbfinale benötigt.")
                End If

                Dim runde = New SpielRunde From {
                    If(players.Count > 3, New SpielPartie("Halbfinale 1", players(0), players(3)), New FreiLosSpiel("Halbfinale 1", players(0))),
                    New SpielPartie("Halbfinale 1", players(1), players(2))
                }

                SetHalbFinalDataContext(runde)

                competition.SpielRunden.Push(runde)
                Return runde
            End If

            If competition.SpielRunden.Count = 2 Then
                Dim halbfinals = competition.SpielRunden.Peek()
                Dim runde = New SpielRunde From {
                    New SpielPartie("Finale 1", halbfinals(0).Gewinner, halbfinals(1).Gewinner)
                }
                SetFinalDataContext(runde)

                competition.SpielRunden.Push(runde)
                Return runde
            End If

            If competition.SpielRunden.Count = 3 Then
                UrkundenFinalisten.IsEnabled = True
            End If

        End If

        Return Nothing
    End Function

    Private Sub UrkundenPlatz5_Click(sender As Object, e As RoutedEventArgs) Handles UrkundenPlatz5.Click
        Dim p = New PrintDialog

        If p.ShowDialog Then

            Dim viertelfinals = competition.SpielRunden.Reverse(1)

            Dim playersP5 = New List(Of SpielerInfo) From {
                If(TypeOf viertelfinals(0) Is FreiLosSpiel, Nothing, viertelfinals(0).Verlierer),
                If(TypeOf viertelfinals(1) Is FreiLosSpiel, Nothing, viertelfinals(1).Verlierer),
                If(TypeOf viertelfinals(2) Is FreiLosSpiel, Nothing, viertelfinals(2).Verlierer),
                If(TypeOf viertelfinals(3) Is FreiLosSpiel, Nothing, viertelfinals(3).Verlierer)
            }

            PrintUrkunde(playersP5, 5, p)

        End If
    End Sub

    Private Sub UrkundenPlatz3_Click(sender As Object, e As RoutedEventArgs) Handles UrkundenPlatz3.Click
        Dim p = New PrintDialog

        If p.ShowDialog Then

            Dim index = If(mode = FinalMode.Viertelfinale, 2, 1)
            Dim halbfinals = competition.SpielRunden.Reverse(index)

            Dim playersP3 = New List(Of SpielerInfo) From {
            If(TypeOf halbfinals(0) Is FreiLosSpiel, Nothing, halbfinals(0).Verlierer),
            If(TypeOf halbfinals(1) Is FreiLosSpiel, Nothing, halbfinals(1).Verlierer)
        }

            PrintUrkunde(playersP3, 3, p)

        End If
    End Sub

    Private Sub UrkundenFinalisten_Click(sender As Object, e As RoutedEventArgs) Handles UrkundenFinalisten.Click
        Dim p = New PrintDialog

        If p.ShowDialog Then

            Dim index = If(mode = FinalMode.Viertelfinale, 3, 2)
            Dim finale = competition.SpielRunden.Reverse(index)(0)

            Dim playersP2 = New List(Of SpielerInfo) From {
                finale.Verlierer
            }

            PrintUrkunde(playersP2, 2, p)

            Dim playersP1 = New List(Of SpielerInfo) From {
                finale.Gewinner
            }

            PrintUrkunde(playersP1, 1, p)

        End If
    End Sub

    Private Sub PrintUrkunde(playerList As IEnumerable(Of SpielerInfo), platzierung As Integer, p As PrintDialog)
        For Each player In playerList
            If player Is Nothing Then Return

            Dim Urkunde = New Urkunde
            With Urkunde
                .Datum1.Text = Date.Today.ToShortDateString() & " in"
                .Datum2.Text = Date.Today.ToLongDateString()
                .SpielerName.Text = player.Vorname & " " & player.Nachname
                .Vereinsname.Text = player.Vereinsname
                .WettbName.Text = "Im Wettbewerb " & CType(Application.Current, Application).competition
                .Platzierung.Text = "zum " & platzierung & ". Platz"
            End With
            p.PrintVisual(Urkunde, "Urkunde Platz " & platzierung)
        Next
    End Sub

    Private Sub Screenshot_Click(sender As Object, e As RoutedEventArgs) Handles Screenshot.Click

        Dim visualToPrint As Visual = Tree
        Dim desiredWidth = 900
        Dim desiredHeight = 630


        ' Calculate the bounds of the visual
        Dim bounds As Rect = VisualTreeHelper.GetDescendantBounds(visualToPrint)


        ' Create a DrawingVisual to contain the visual
        Dim drawingVisual As New DrawingVisual()
        Using context As DrawingContext = drawingVisual.RenderOpen()
            ' Draw the visual without any offset
            context.DrawRectangle(New VisualBrush(visualToPrint), Nothing, New Rect(New Point(), bounds.Size))

            context.DrawRectangle(Brushes.White, Nothing, New Rect(0, bounds.Height - 50, bounds.Width, bounds.Height))


            ' Create the formatted text to draw
            Dim formattedText As New FormattedText(
                Date.Today.ToShortDateString & "        " & CType(Application.Current, Application).competition,
                System.Globalization.CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                New Typeface("Arial"),
                14,
                Brushes.Black
            )

            ' Position the text (e.g., below the visual)
            Dim textPosition As New Point(0, -100) ' Adjust the position as needed

            ' Draw the text
            context.DrawText(formattedText, textPosition)
        End Using

        ' Calculate the scale factors for desired size
        Dim scaleX As Double = desiredWidth / bounds.Width
        Dim scaleY As Double = desiredHeight / bounds.Height
        Dim scale = Math.Min(scaleY, scaleX)

        ' Create a TransformGroup to combine translate and scale transforms
        Dim transformGroup As New TransformGroup()

        ' Remove the offset by translating the visual to (20, 20)
        Dim translateTransform As New TranslateTransform(-bounds.X + 100, -bounds.Y + 150)
        transformGroup.Children.Add(translateTransform)

        ' Scale the visual to the desired size
        Dim scaleTransform As New ScaleTransform(scale, scale)
        transformGroup.Children.Add(scaleTransform)

        ' Apply the transformations
        drawingVisual.Transform = transformGroup

        Dim Druckeinstellung = New DruckEinstellungen
        Dim p = Druckeinstellung.EinstellungenSchiedsrichterzettel

        p.PrintTicket.PageOrientation = PageOrientation.Landscape
        p.PrintVisual(drawingVisual, "Screenshot")

    End Sub

    Private Sub Schiedsrichter_Click(sender As Object, e As RoutedEventArgs) Handles Schiedsrichter.Click
        Dim w = CType(Application.Current.MainWindow, MainWindow)
        Dim Einstellung = New DruckEinstellungen
        Dim p = w._DruckerFabrik.Neu(Einstellung.EinstellungenSchiedsrichterzettel)
        Dim doc = w._Controller.DruckeSchiedsrichterzettel(p.LeseKonfiguration)
        p.Drucken(doc, "Schiedsrichterzettel")
    End Sub
End Class

