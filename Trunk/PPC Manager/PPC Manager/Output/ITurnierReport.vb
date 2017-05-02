Imports PPC_Manager

Public Interface ITurnierReport
    Inherits IDisposable
    Sub SchreibeNeuePartien(spielPartien As IEnumerable(Of SpielPartie), runde As Integer)
    Sub SchreibeRangliste(spieler As IEnumerable(Of SpielerInfo), rundeNr As Integer)
End Interface
