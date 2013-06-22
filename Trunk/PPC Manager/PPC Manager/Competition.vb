Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class Competition

    Property StartDatum As String
    Property Altersgruppe As String
    Property SpielerListe As New SpielerListe
    Property DateiPfad As String
    Property SatzDifferenz As Boolean
    Property AutoSaveAn As Boolean
    Property SpielRunden As New SpielRunden
    Property Gewinnsätze As Integer

    Property SonneBornBerger As Boolean

    Public Shared Function FromXML(dateipfad As String, node As XElement, satzzahl As Double, satzDifferenz As Boolean?, SonneBornBerger As Boolean?) As Competition
        Dim c = New Competition With
               {
                   .Gewinnsätze = Convert.ToInt32(satzzahl),
                   .SatzDifferenz = Convert.ToBoolean(satzDifferenz),
                   .SonneBornBerger = Convert.ToBoolean(SonneBornBerger),
                   .DateiPfad = dateipfad,
                   .StartDatum = node.Attribute("start-date").Value,
                   .Altersgruppe = node.Attribute("age-group").Value,
                   .SpielerListe = SpielerListe.FromXML(node.<players>)
                }
        c.SpielRunden = SpielRunden.FromXML(c.SpielerListe, node.<matches>.SingleOrDefault)

        Return c
    End Function

    Public Shared Function FromXML(dateiPfad As String, gruppe As String, satzzahl As Double, satzDifferenz As Boolean?, SonneBornBerger As Boolean?) As Competition
        Dim doc = XDocument.Load(dateiPfad)
        Dim competitionXML = (From x In doc.Root.<competition> Where x.Attribute("age-group").Value = gruppe).Single
        ' Syntax Checks

        Dim TryBool = Function(x As String) As Boolean
                          Dim val As Boolean = False
                          Boolean.TryParse(x, val)
                          Return val
                      End Function

        Dim UnbekannterZustand = From x In competitionXML...<person> Where Not TryBool(x.@ppc:anwesend) AndAlso Not TryBool(x.@ppc:abwesend)

        If UnbekannterZustand.Any Then
            MessageBox.Show(String.Format("Es gibt noch {0} Spieler dessen Anwesenheitsstatus unbekannt ist. Bitte korrigieren bevor das Turnier beginnt.", UnbekannterZustand.Count), _
                            "Spieldaten unvollständig", MessageBoxButton.OK, MessageBoxImage.Error)
            Application.Current.Shutdown()
        End If
        Return FromXML(dateiPfad, competitionXML, satzzahl, satzDifferenz, SonneBornBerger)
    End Function


    Public Sub SaveXML()
        Dim doc = XDocument.Load(DateiPfad)
        Dim CompetitionNode = (From x In doc.Root.<competition> Where x.Attribute("age-group").Value = Altersgruppe).Single
        CompetitionNode.@ppc:satzdifferenz = SatzDifferenz.ToString
        CompetitionNode.@ppc:gewinnsätze = Gewinnsätze.ToString
        CompetitionNode.@ppc:sonnebornberger = SonneBornBerger.ToString
        Dim runden = SpielRunden.ToXML
        CompetitionNode.<matches>.Remove()
        CompetitionNode.Add(<matches>
                                <%= runden %>
                            </matches>)

        doc.Save(DateiPfad)
    End Sub

    Public Sub SaveExcel()
        Dim spieler = MainWindow.AktiveCompetition.SpielerListe.ToList
        spieler.Sort(New ExportComparer)
        spieler.Reverse()

        ExcelInterface.CreateFile(ExcelPfad, spieler, SpielRunden)
    End Sub

    Private Class ExportComparer
        Implements IComparer(Of Spieler)

        Public Function Compare(myself As Spieler, other As Spieler) As Integer Implements IComparer(Of Spieler).Compare
            Dim diff = myself.ExportPunkte - other.ExportPunkte
            If diff <> 0 Then Return diff
            diff = myself.ExportBHZ - other.ExportBHZ
            If diff <> 0 Then Return diff

            If MainWindow.AktiveCompetition.SonneBornBerger Then
                diff = myself.ExportSonneborn - other.ExportSonneborn
                If diff <> 0 Then Return diff
            End If

            If MainWindow.AktiveCompetition.SatzDifferenz Then
                diff = (myself.ExportSätzeGewonnen - myself.ExportSätzeVerloren) - (other.ExportSätzeGewonnen - other.ExportSätzeVerloren)
                If diff <> 0 Then Return diff
            End If
            diff = myself.TTRating - other.TTRating
            If diff <> 0 Then Return diff
            diff = myself.TTRMatchCount - other.TTRMatchCount
            If diff <> 0 Then Return diff
            diff = other.Nachname.CompareTo(myself.Nachname)
            If diff <> 0 Then Return diff
            diff = other.Vorname.CompareTo(myself.Vorname)
            If diff <> 0 Then Return diff
            Return myself.Lizenznummer - other.Lizenznummer
        End Function
    End Class

    Public ReadOnly Property ExcelPfad As String
        Get
            Dim DateiName = IO.Path.GetFileNameWithoutExtension(DateiPfad)
            DateiName &= " " & Altersgruppe
            For Each c In IO.Path.GetInvalidFileNameChars
                DateiName = DateiName.Replace(c, " "c)
            Next
            DateiName &= ".xlsx"
            Dim Unterpfad = IO.Path.Combine(IO.Path.GetDirectoryName(DateiPfad), "Protokolle")
            DateiName = IO.Path.Combine(Unterpfad, DateiName)
            Return DateiName
        End Get
    End Property


End Class
