Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports Microsoft.Win32
Imports StartlistenEditor

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private repository As SpielerRepository


    Private Sub Application_Exit(sender As Object, e As ExitEventArgs) Handles Me.[Exit]
        repository?.Deregister()
    End Sub

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        AddHandler AppDomain.CurrentDomain.UnhandledException, Sub(sender2 As Object, args As UnhandledExceptionEventArgs)
                                                                   Dim f As New Fehler
                                                                   f.ExceptionText.Text = args.ExceptionObject.ToString
                                                                   f.ShowDialog()
                                                               End Sub
        Me.ShutdownMode = ShutdownMode.OnExplicitShutdown
        Dim d = New ProgrammStart
        If Not d.ShowDialog Then
            Shutdown()
            Return
        End If
        Dim pfad As String
        If d.NeuesTurnier Then
            With New NeuesTurnierDialog
                If Not .ShowDialog() Then
                    Shutdown()
                    Return
                End If
                pfad = .NeuesTurnierKontext.Dateiname
                SpeichereTurnier(.NeuesTurnierKontext)
            End With
        Else
            pfad = ÖffneDialog()
        End If

        Dim dateiSystem = New Dateisystem(pfad)
        Dim speicher As New Speicher(dateiSystem)
        Dim cache As New SpeicherCache(speicher)
        Dim observable = New StartlistenController(speicher.LeseSpieler())
        repository = New SpielerRepository(cache, observable, observable)
        Dim mainWindow As New MainWindow(observable,
                                      cache.KlassementNamen, AddressOf cache.SpeichereAlles)
        mainWindow.Show()
        Me.ShutdownMode = ShutdownMode.OnLastWindowClose
    End Sub

    Private Sub SpeichereTurnier(neuesTurnierKontext As NeuesTurnierKontext)
        With neuesTurnierKontext
            Dim doc = New XDocument(<tournament
                                        end-date=<%= .Turnierende.ToString("yyyy-MM-dd") %>
                                        start-date=<%= .Turnierbeginn.ToString("yyyy-MM-dd") %>
                                        name=<%= .Turniername %>
                                        tournament-id=<%= Guid.NewGuid %>>
                                        <%= From x In .Klassements Select <competition start-date=<%= x.KlassementStart.ToString("yyyy-MM-dd H:mm") %>
                                                                              ttr-remarks=<%= x.TTRHinweis %>
                                                                              age-group=<%= x.KlassementName %>
                                                                              type=<%= x.Typ %>>
                                                                              <players/>
                                                                          </competition> %>
                                    </tournament>)
            doc.Save(neuesTurnierKontext.Dateiname)
        End With

    End Sub

    Private Function ÖffneDialog() As String
        With New OpenFileDialog
            .Filter = "Click-TT Turnierdaten|*.xml"
            If Not .ShowDialog Then
                Shutdown()
                Return ""
            End If
            Return .FileName
        End With
    End Function
End Class
