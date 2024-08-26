Imports System.Collections.ObjectModel
Imports System.IO
Imports DocumentFormat.OpenXml.EMMA
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class TabHeader
    Public Property Icon As String
    Public Property HeaderText As String
    Public Property SubText As String
    Public Property FontWeight As String
    Public Property PlayerCount As Integer
End Class

Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.

    Private Sub Application_Exit(ByVal sender As Object, ByVal e As System.Windows.ExitEventArgs) Handles Me.Exit
        My.Settings.Save()
    End Sub


    Public doc As XDocument
    Public xmlPfad As String
    Public competition As String
    Public AktiveCompetition As Competition

    Public Sub CrashBehandeln(sender As Object, args As UnhandledExceptionEventArgs)
        Dim nachricht = ""
        If TypeOf args.ExceptionObject Is FileNotFoundException Then
            Dim x = CType(args.ExceptionObject, FileNotFoundException)
            nachricht += x.FusionLog
        End If
        nachricht += args.ExceptionObject.ToString
        Dim f As New Fehler
        f.ExceptionText.Text = nachricht
        f.ShowDialog()
    End Sub

    Public Sub LadeCompetition(sender As Object, klassement As String)
        AktiveCompetition = Nothing
        competition = klassement
        doc = XDocument.Load(xmlPfad)
        Dim TabHeaders As Collection(Of TabHeader) = New Collection(Of TabHeader)(MakeTabHeaders(klassement))
        Dim Regeln = SpielRegeln.Parse(doc, klassement)
        Dim spielRunden = New SpielRunden
        Dim spielpartien = spielRunden.SelectMany(Function(m) m)
        Dim ausgeschiedeneIds = spielRunden.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs)
        Dim spielstand = New Spielstand(Regeln.Gewinnsätze)
        Dim spielverlauf = New Spielverlauf(spielpartien, ausgeschiedeneIds, spielstand)

        Try
            'befüllt spielRunden
            AktiveCompetition = AusXML.CompetitionFromXML(xmlPfad, doc, klassement, Regeln, spielRunden)

        Catch ex As SpielDatenUnvollständigException
            'Fehler wird in MainWindow behandelt, um UI Fehlermeldungen anzuzeigen, statt abzustürzen
            Dim competitionXML = (From x In doc.Root.<competition> Where x.Attribute("age-group").Value = competition).Single
            Dim spielerInfos = AusXML.SpielerListeFromXML(competitionXML.<players>)
            If AktiveCompetition Is Nothing Then AktiveCompetition = New Competition(spielerInfos, competition)
        End Try

        Resources("KlassementName") = AktiveCompetition.Altersgruppe
        Dim speichern = Sub() ZuXML.SaveXML(xmlPfad, Regeln, klassement, AktiveCompetition.SpielRunden)
        Dim excelVerlauf = New Spielverlauf(spielRunden.Reverse.SelectMany(Function(m) m),
                                        spielRunden.Reverse.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs),
                                        spielstand)
        Dim excelFabrik = New ExcelFabrik(spielstand, excelVerlauf)
        Dim vergleicher = New SpielerInfoComparer(spielverlauf, Regeln.SatzDifferenz, Regeln.SonneBornBerger)
        Dim r = New ReportFactory(xmlPfad,
                                  klassement,
                                  AktiveCompetition.SpielerListe,
                                  vergleicher,
                                  AktiveCompetition.SpielRunden,
                                  AddressOf excelFabrik.HoleDokument)
        Dim ausgeschiedeneSpielerIds = spielRunden.SelectMany(Function(m) m.AusgeschiedeneSpielerIDs)

        Dim AktiveListe = From x In AktiveCompetition.SpielerListe
                          Where Not ausgeschiedeneSpielerIds.Contains(x.Id)
                          Select x
        Dim OrganisierePakete = Function()
                                    Dim spielverlaufCache = New SpielverlaufCache(spielverlauf)
                                    Dim comparer = New SpielerInfoComparer(spielverlaufCache, Regeln.SatzDifferenz, Regeln.SonneBornBerger)
                                    Dim paarungsSuche = New PaarungsSuche(Of SpielerInfo)(AddressOf comparer.Compare, AddressOf spielverlaufCache.Habengegeneinandergespielt)
                                    Dim begegnungen = New PaketBildung(Of SpielerInfo)(spielverlaufCache, AddressOf paarungsSuche.SuchePaarungen)
                                    Dim l = AktiveListe.ToList()
                                    l.Sort(comparer)
                                    l.Reverse()
                                    Return begegnungen.organisierePakete(l, spielRunden.Count - 1)
                                End Function

        Dim spielerWrapped = From x In AktiveCompetition.SpielerListe
                             Select New Spieler(x, spielverlauf, vergleicher)
        Dim druckFabrik = New FixedPageFabrik(
            spielerWrapped,
            spielRunden,
            klassement,
            spielstand)
        Dim controller = New MainWindowController(speichern,
                                                  r,
                                                  OrganisierePakete,
                                                  druckFabrik)
        Dim oldWindow = MainWindow

        Dim window = New MainWindow(controller,
                                    spielerWrapped,
                                    spielRunden,
                                    klassement,
                                    spielstand,
                                    vergleicher,
                                    New DruckerFabrik,
                                    TabHeaders,
                                    doc)

        MainWindow = window

        'copy position and size from old window to new window
        If oldWindow IsNot Nothing Then
            window.Left = oldWindow.Left
            window.Top = oldWindow.Top
            window.Width = oldWindow.Width
            window.Height = oldWindow.Height

            'Handle Fullscreen
            window.WindowState = oldWindow.WindowState
            window.WindowStyle = oldWindow.WindowStyle
            window.ResizeMode = oldWindow.ResizeMode

            oldWindow.Close()
        End If

        window.Show()

    End Sub

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CrashBehandeln
        ShutdownMode = ShutdownMode.OnExplicitShutdown
        With New LadenNeu

            If Not .ShowDialog() Then
                Shutdown()
                Return
            End If

            xmlPfad = .XMLPathText.Text

            LadeCompetition(sender, .CompetitionCombo.SelectedItem.ToString)

            ShutdownMode = ShutdownMode.OnMainWindowClose
        End With
    End Sub

    Friend Function MakeTabHeaders(klassement As String) As List(Of TabHeader)
        Dim tabs As New List(Of TabHeader)()

        ' Iterate over each competition node
        For Each competition As XElement In doc.Root.<competition>
            ' Retrieve the necessary attributes and elements
            Dim ageGroup As String = competition.Attribute("age-group").Value
            Dim icon As String = "defaultIcon.png" ' Set a default icon or choose one based on your logic
            Dim fontWeight As String = ""

            ' Determine the SubText based on the conditions
            Dim subText As String
            Dim hasPlayers As Boolean = competition.<players>.<player>.Any()
            Dim playerCount As Integer = competition.<players>.<player>.Count()
            Dim hasFinalsMode As Boolean = competition.@ppc:finalsmode IsNot Nothing

            If Not hasPlayers Then
                subText = "Playoff Config"
                icon = "sliders"
            ElseIf hasFinalsMode Then
                subText = "Playoff"
                icon = "Trophy"
            Else
                subText = "Vorrunde"
                icon = "Group"
            End If

            If ageGroup = klassement Then
                fontWeight = "DemiBold"
            End If

            ' Create a new TabHeader object and add it to the tabs list
            Dim tabHeader As New TabHeader With {
                .HeaderText = ageGroup,
                .SubText = subText,
                .Icon = icon,
                .FontWeight = fontWeight,
                .PlayerCount = playerCount
            }

            tabs.Add(tabHeader)
        Next

        tabs.Sort(Function(a, b) String.Compare(a.Icon, b.Icon))

        Return tabs
    End Function
End Class
