Imports PPC_Manager

Public Class DesignSpielRunden
    Inherits SpielRunden

    Public Sub New()

        Dim SpielRunde As New SpielRunde
        Dim spielerListe = New DesignSpielerListe()

        For Each partie In New DesignSpielPartien
            SpielRunde.Add(partie)
        Next
        Dim verlauf = New DesignSpielverlauf
        SpielRunde.Add(New FreiLosSpiel("Runde 1", New Spieler(verlauf) With {.Id = "999", .Nachname = "MisterX", .Vorname = "Max"}, 3))

        AusgeschiedeneSpieler.Add(New Ausgeschieden(Of SpielerInfo) With {.Spieler = spielerListe.Last, .Runde = 1})
        Me.Push(SpielRunde)
    End Sub


End Class

Public Class DesignController
    Implements IController

    Public ReadOnly Property ExcelPfad As String Implements IController.ExcelPfad
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public Sub ExcelExportieren(p1 As String) Implements IController.ExcelExportieren

    End Sub

    Public Sub NächsteRunde() Implements IController.NächsteRunde

    End Sub

    Public Sub NächstesPlayoff() Implements IController.NächstesPlayoff

    End Sub

    Public Sub RundenbeginnDrucken(printdialog As IPrinter) Implements IController.RundenbeginnDrucken

    End Sub

    Public Sub RundenendeDrucken(p As IPrinter) Implements IController.RundenendeDrucken

    End Sub

    Public Sub Save() Implements IController.SaveXML

    End Sub

    Public Sub NeuePartie(rundenName As String, spielerA As SpielerInfo, SpielerB As SpielerInfo) Implements IController.NeuePartie

    End Sub

    Public Sub SaveExcel() Implements IController.SaveExcel

    End Sub

    Public Sub SpielerAusscheiden(spieler As SpielerInfo) Implements IController.SpielerAusscheiden
        Throw New NotImplementedException()
    End Sub
End Class

Public Class DesignSpielPartienOhneFreilos
    Inherits SpielPartien

    Public Sub New()
        Dim spielerListe = New DesignSpielerListe()
        For i = 1 To spielerListe.Count - 1 Step 2
            Dim partie = New SpielPartie("Runde 2", spielerListe(i - 1), spielerListe(i), 3)
            partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz With {.PunkteLinks = 0, .PunkteRechts = 11})
            partie.Add(New Satz With {.PunkteLinks = 0, .PunkteRechts = 11})
            partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 5})
            Me.Add(partie)
        Next
    End Sub

    Public Shared ReadOnly Property DesignSpielPartie As SpielPartie
        Get
            Dim partien = New DesignSpielPartien
            Return partien.First
        End Get
    End Property

End Class

Public Class DesignSpielPartien
    Inherits DesignSpielPartienOhneFreilos

    Public Sub New()
        Dim spielverlauf = New DesignSpielverlauf
        Me.Add(New FreiLosSpiel("Runde 2", New Spieler(spielverlauf) With {.Vorname = "Günther", .Nachname = "Netzer", .Id = "PLAYER255"}, 3))
    End Sub

End Class

Public Class DesignSpielverlauf
    Implements ISpielverlauf(Of SpielerInfo)

    Public Function BerechneBuchholzPunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneBuchholzPunkte
        Return 42
    End Function

    Public Function BerechneGewonneneSätze(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneGewonneneSätze
        Return 42
    End Function

    Public Function BerechnePunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechnePunkte
        Return 42
    End Function

    Public Function BerechneSatzDifferenz(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneSatzDifferenz
        Return 42
    End Function

    Public Function BerechneSonnebornBergerPunkte(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneSonnebornBergerPunkte
        Return 42
    End Function

    Public Function BerechneVerloreneSätze(t As SpielerInfo) As Integer Implements ISpielverlauf(Of SpielerInfo).BerechneVerloreneSätze
        Return 42
    End Function

    Public Function Habengegeneinandergespielt(a As SpielerInfo, b As SpielerInfo) As Boolean Implements ISpielverlauf(Of SpielerInfo).Habengegeneinandergespielt
        Return False
    End Function

    Public Function HatFreilos(t As SpielerInfo) As Boolean Implements ISpielverlauf(Of SpielerInfo).HatFreilos
        Return False
    End Function

    Public Function IstAusgeschieden(t As SpielerInfo) As Boolean Implements ISpielverlauf(Of SpielerInfo).IstAusgeschieden
        Return False
    End Function
End Class

Friend Class DesignSpielerListe
    Inherits SpielerListe

    Public Sub New()
        Dim spielverlauf = New DesignSpielverlauf
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER1", .Vorname = "Marius", .Nachname = "Goppelt", .TTRating = 1, .Geschlecht = 1})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER2", .Vorname = "Flo", .Nachname = "Ewald", .TTRating = 2})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER3", .Vorname = "Hartmut", .Nachname = "Seiter", .TTRating = 3})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER4", .Vorname = "Max", .Nachname = "Mustermann", .Vereinsname = "TTC Langensteinbach", .TTRating = 4})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER5", .Vorname = "Leonardo", .Nachname = "Da Vinci", .TTRating = 5, .Geschlecht = 1})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER6", .Vorname = "Sarah", .Nachname = "Palin", .TTRating = 6, .Geschlecht = 0})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER7", .Vorname = "Manuel José", .Nachname = "Barroso", .TTRating = 7})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER8", .Vorname = "Catharina", .Nachname = "Sforza", .TTRating = 8})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER9", .Vorname = "Adam", .Nachname = "Jensen", .TTRating = 9})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER10", .Vorname = "Miroslav", .Nachname = "Klose", .TTRating = 10})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER11", .Vorname = "Che", .Nachname = "Guevara", .TTRating = 11})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER12", .Vorname = "Friedrich", .Nachname = "Nietzsche", .TTRating = 12})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER13", .Vorname = "Johann Wolfgang von", .Nachname = "Goethe", .TTRating = 13})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER14", .Vorname = "Margaret", .Nachname = "Thatcher", .TTRating = 14})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER15", .Vorname = "Marie", .Nachname = "Curie", .TTRating = 15})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER16", .Vorname = "Albert", .Nachname = "Einstein", .TTRating = 16})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER17", .Vorname = "Julius", .Nachname = "Cäsar", .TTRating = 17})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER18", .Vorname = "Lucius", .Nachname = "Verenus", .TTRating = 18})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER19", .Vorname = "Titus", .Nachname = "Pullo", .TTRating = 19})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER20", .Vorname = "Mick", .Nachname = "Jagger", .TTRating = 20})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER21", .Vorname = "Paul", .Nachname = "Ryan", .TTRating = 21})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER22", .Vorname = "Barack", .Nachname = "Obama", .TTRating = 22})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER23", .Vorname = "Francois", .Nachname = "Mitterrand", .Vereinsname = "TTC Langensteinbach", .TTRating = 23})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER24", .Vorname = "Napoleon", .Nachname = "Bonaparte", .TTRating = 24, .Geburtsjahr = 1790})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER25", .Vorname = "Marty", .Nachname = "McFly", .TTRating = 25})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER26", .Vorname = "Marilyn", .Nachname = "Monroe", .TTRating = 26})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER27", .Vorname = "Freddy", .Nachname = "Mercury", .TTRating = 27})
        Add(New Spieler(spielverlauf) With {.Id = "PLAYER28", .Vorname = "Stephen", .Nachname = "Hawking", .TTRating = 28})
    End Sub
End Class
