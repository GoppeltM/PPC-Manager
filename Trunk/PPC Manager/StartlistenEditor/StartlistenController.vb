﻿Imports Microsoft.Win32
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

''' <summary>
''' Enthält alle Operationen die auf die Datenschicht einwirken
''' </summary>
''' <remarks></remarks>
Public Class StartlistenController
    Implements IStartlistenController

    Private _SpielerListe As SpielerListe

    Public ReadOnly Property SpielerListe As SpielerListe
        Get
            Return _SpielerListe
        End Get
    End Property

    Private _KlassementListe As KlassementListe

    Public ReadOnly Property KlassementListe As KlassementListe
        Get
            Return _KlassementListe
        End Get
    End Property

    Sub Initialize(spielerListe As SpielerListe, klassementListe As KlassementListe) Implements IStartlistenController.Initialize
        _SpielerListe = spielerListe
        _KlassementListe = klassementListe
    End Sub

    Public Shared Doc As XDocument
    Public Pfad As String

    Public Sub Öffnend(doc As XDocument, pfad As String) Implements IStartlistenController.Öffnend
        StartlistenController.Doc = doc
        Me.Pfad = pfad
        Dim AlleSpieler = XmlZuSpielerListe(doc)

        Dim AlleKlassements = (From x In doc.Root.<competition> Select x.Attribute("age-group").Value).Distinct
        With KlassementListe
            For Each Klassement In AlleKlassements
                .Add(New KlassementName With {.Name = Klassement})
            Next
        End With

        For Each s In AlleSpieler
            SpielerListe.Add(s)
        Next
    End Sub


    Public Sub Speichern() Implements IStartlistenController.Speichern
        If Pfad Is Nothing Then Return
        Dim AusgeschiedenKlassements = From x In SpielerListe Group x By x.KlassementNode Into Group

        For Each klassement In AusgeschiedenKlassements

            If Not klassement.KlassementNode.<matches>.Any Then
                klassement.KlassementNode.Add(<matches/>)
            End If

            For Each ausgeschiedeneSpieler In klassement.KlassementNode.<matches>.<ppc:inactiveplayer>.ToList
                If ausgeschiedeneSpieler.@group = "0" Then
                    ausgeschiedeneSpieler.Remove()
                End If
            Next

            Dim xSpieler = From x In klassement.Group Where x.Abwesend Select <ppc:inactiveplayer player=<%= x.ID %> group="0"/>

            klassement.KlassementNode.<matches>.Single.Add(xSpieler)
        Next

        Doc.Save(Pfad)
    End Sub

    Public Sub NeuerFremdSpieler(neuerTTR As Integer) Implements IStartlistenController.NeuerFremdSpieler
        Dim Lizenznummern = (From x In SpielerListe Select x.LizenzNr).ToList
        Dim NeueLizenzNummer = -1
        While Lizenznummern.Contains(NeueLizenzNummer)
            NeueLizenzNummer -= 1
        End While
        Dim dialog = FremdSpielerDialog.NeuerFremdSpieler(Doc, neuerTTR, NeueLizenzNummer)
        If dialog.ShowDialog() Then
            SpielerListe.Add(dialog.Spieler)
        End If
    End Sub

    Public Sub LöscheFremdSpieler(aktuellerSpieler As Spieler) Implements IStartlistenController.LöscheFremdSpieler
        SpielerListe.Remove(aktuellerSpieler)
        aktuellerSpieler.XmlKnoten.Remove()
    End Sub

    Public Sub EditiereFremdSpieler(aktuellerSpieler As Spieler) Implements IStartlistenController.EditiereFremdSpieler
        Dim dialog = FremdSpielerDialog.EditiereFremdSpieler(Doc, aktuellerSpieler)
        If Not dialog.ShowDialog() Then
            aktuellerSpieler.CancelEdit()
        End If
    End Sub

    Public Shared Function XmlZuSpielerListe(doc As XDocument) As IList(Of Spieler)

        Dim SpielerListe = From competition In doc.Root.<competition>
                           From Spieler In competition...<player>.Concat(competition...<ppc:player>)
                           Select Spieler

        Dim AlleSpieler As New List(Of Spieler)

        For Each s In SpielerListe
            AlleSpieler.Add(Spieler.FromXML(s))
        Next

        AlleSpieler.Sort()
        Return AlleSpieler
    End Function

End Class
