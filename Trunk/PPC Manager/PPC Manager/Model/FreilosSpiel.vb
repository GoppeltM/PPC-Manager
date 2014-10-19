﻿Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class FreiLosSpiel
    Inherits SpielPartie

    Public Sub New(rundenName As String, ByVal freilosSpieler As Spieler, gewinnsätze As Integer)
        MyBase.New(rundenName, freilosSpieler, freilosSpieler, gewinnsätze)
    End Sub


    Public Overrides Function ToXML(matchNr As Integer) As System.Xml.Linq.XElement
        Dim node = <ppc:freematch player=<%= SpielerLinks.Id %> group=<%= RundenName %>
                       scheduled=<%= ZeitStempel.ToString("yyyy-MM-dd HH:mm") %>/>
        Return node
    End Function

    Public Overrides ReadOnly Property MeineGewonnenenSätze(ByVal ich As Spieler) As System.Collections.Generic.IList(Of Satz)
        Get
            Dim l As New List(Of Satz)
            For i = 0 To _Gewinnsätze - 1
                l.Add(New Satz() With {.PunkteLinks = My.Settings.GewinnPunkte})
            Next
            Return l
        End Get
    End Property

    Overloads Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xFreilosSpiel As XElement, gewinnsätze As Integer) As FreiLosSpiel
        Dim spieler = (From x In spielerListe Where x.Id = xFreilosSpiel.@player Select x).First
        Return New FreiLosSpiel(xFreilosSpiel.@group, spieler, gewinnsätze)
    End Function

    Public Overrides Function ToString() As String
        Return "Freilos für " & Me.SpielerLinks.Nachname
    End Function

End Class
