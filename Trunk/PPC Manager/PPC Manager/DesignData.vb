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
        SpielRunde.Add(New FreiLosSpiel("Runde 1", New SpielerInfo("999") With {.Nachname = "MisterX", .Vorname = "Max"}, 3))
        SpielRunde.AusgeschiedeneSpielerIDs.Add("999")
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

    Public Sub RundenbeginnDrucken(printdialog As IPrinter) Implements IController.RundenbeginnDrucken

    End Sub

    Public Sub RundenendeDrucken(p As IPrinter) Implements IController.RundenendeDrucken

    End Sub

    Public Sub Save() Implements IController.SaveXML

    End Sub

    Public Sub SaveExcel() Implements IController.SaveExcel

    End Sub

    Public Function NächsteRunde(rundenName As String) As SpielRunde Implements IController.NächsteRunde
        Throw New NotImplementedException()
    End Function
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
        Me.Add(New FreiLosSpiel("Runde 2", New SpielerInfo("PLAYER255") With {.Vorname = "Günther", .Nachname = "Netzer"}, 3))
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
    Inherits List(Of SpielerInfo)

    Public Sub New()
        Add(New SpielerInfo("PLAYER01") With {.Vorname = "Marius", .Nachname = "Goppelt", .TTRating = 1, .Geschlecht = 1})
        Add(New SpielerInfo("PLAYER02") With {.Vorname = "Flo", .Nachname = "Ewald", .TTRating = 2})
        Add(New SpielerInfo("PLAYER03") With {.Vorname = "Hartmut", .Nachname = "Seiter", .TTRating = 3})
        Add(New SpielerInfo("PLAYER04") With {.Vorname = "Max", .Nachname = "Mustermann", .Vereinsname = "TTC Langensteinbach", .TTRating = 4})
        Add(New SpielerInfo("PLAYER05") With {.Vorname = "Leonardo", .Nachname = "Da Vinci", .TTRating = 5, .Geschlecht = 1})
        Add(New SpielerInfo("PLAYER06") With {.Vorname = "Sarah", .Nachname = "Palin", .TTRating = 6, .Geschlecht = 0})
        Add(New SpielerInfo("PLAYER07") With {.Vorname = "Manuel José", .Nachname = "Barroso", .TTRating = 7})
        Add(New SpielerInfo("PLAYER08") With {.Vorname = "Catharina", .Nachname = "Sforza", .TTRating = 8})
        Add(New SpielerInfo("PLAYER09") With {.Vorname = "Adam", .Nachname = "Jensen", .TTRating = 9})
        Add(New SpielerInfo("PLAYER10") With {.Vorname = "Miroslav", .Nachname = "Klose", .TTRating = 10})
        Add(New SpielerInfo("PLAYER11") With {.Vorname = "Che", .Nachname = "Guevara", .TTRating = 11})
        Add(New SpielerInfo("PLAYER12") With {.Vorname = "Friedrich", .Nachname = "Nietzsche", .TTRating = 12})
        Add(New SpielerInfo("PLAYER13") With {.Vorname = "Johann Wolfgang von", .Nachname = "Goethe", .TTRating = 13})
        Add(New SpielerInfo("PLAYER14") With {.Vorname = "Margaret", .Nachname = "Thatcher", .TTRating = 14})
        Add(New SpielerInfo("PLAYER15") With {.Vorname = "Marie", .Nachname = "Curie", .TTRating = 15})
        Add(New SpielerInfo("PLAYER16") With {.Vorname = "Albert", .Nachname = "Einstein", .TTRating = 16})
        Add(New SpielerInfo("PLAYER17") With {.Vorname = "Julius", .Nachname = "Cäsar", .TTRating = 17})
        Add(New SpielerInfo("PLAYER18") With {.Vorname = "Lucius", .Nachname = "Verenus", .TTRating = 18})
        Add(New SpielerInfo("PLAYER19") With {.Vorname = "Titus", .Nachname = "Pullo", .TTRating = 19})
        Add(New SpielerInfo("PLAYER20") With {.Vorname = "Mick", .Nachname = "Jagger", .TTRating = 20})
        Add(New SpielerInfo("PLAYER21") With {.Vorname = "Paul", .Nachname = "Ryan", .TTRating = 21})
        Add(New SpielerInfo("PLAYER22") With {.Vorname = "Barack", .Nachname = "Obama", .TTRating = 22})
        Add(New SpielerInfo("PLAYER23") With {.Vorname = "Francois", .Nachname = "Mitterrand", .Vereinsname = "TTC Langensteinbach", .TTRating = 23})
        Add(New SpielerInfo("PLAYER24") With {.Vorname = "Napoleon", .Nachname = "Bonaparte", .TTRating = 24, .Geburtsjahr = 1790})
        Add(New SpielerInfo("PLAYER25") With {.Vorname = "Marty", .Nachname = "McFly", .TTRating = 25})
        Add(New SpielerInfo("PLAYER26") With {.Vorname = "Marilyn", .Nachname = "Monroe", .TTRating = 26})
        Add(New SpielerInfo("PLAYER27") With {.Vorname = "Freddy", .Nachname = "Mercury", .TTRating = 27})
        Add(New SpielerInfo("PLAYER28") With {.Vorname = "Stephen", .Nachname = "Hawking", .TTRating = 28})
    End Sub
End Class
