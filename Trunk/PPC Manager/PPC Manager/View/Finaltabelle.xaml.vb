Imports System.Collections.ObjectModel
Imports System.Globalization
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class Finaltabelle

    Public Sub Init(Competition As XElement, mode As Integer)
        Dim playersxml = Competition.<players>.<player>

        playersxml = playersxml.OrderBy(Function(x) x.@ppc:vpos)

        Dim players = playersxml.Select(Function(x) AusXML.SpielerFromXML(x))

        If mode = FinalMode.Viertelfinale Then
            'spielpartie
            V1.DataContext = New SpielPartie("Viertelfinale 1", players(0), players(7))
            V2.DataContext = New SpielPartie("Viertelfinale 2", players(1), players(6))
            V3.DataContext = New SpielPartie("Viertelfinale 3", players(2), players(5))
            V4.DataContext = New SpielPartie("Viertelfinale 4", players(3), players(4))
        End If
    End Sub

End Class

