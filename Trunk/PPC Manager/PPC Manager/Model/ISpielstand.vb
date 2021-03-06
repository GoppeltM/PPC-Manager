﻿Imports PPC_Manager

Public Interface ISpielstand
    Function IstAbgeschlossen(spielPartie As SpielPartie) As Boolean
    Function MeineGewonnenenSätze(partie As SpielPartie, ich As SpielerInfo) As Integer
    Function MeineVerlorenenSätze(partie As SpielPartie, ich As SpielerInfo) As Integer
    Function HatPartieGewonnen(x As SpielPartie, t As SpielerInfo) As Boolean
End Interface
