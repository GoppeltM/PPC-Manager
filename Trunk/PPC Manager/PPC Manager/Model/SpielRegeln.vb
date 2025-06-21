Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class SpielRegeln


    Private ReadOnly _Gewinnsätze As Integer
    Private ReadOnly _SatzDifferenz As Boolean
    Private ReadOnly _SonneBornBerger As Boolean

    Public Sub New(gewinnSätze As Double?, satzDifferenz As Boolean?, sonneBornBerger As Boolean?)
        _Gewinnsätze = Convert.ToInt32(gewinnSätze)
        _SatzDifferenz = Convert.ToBoolean(satzDifferenz)
        _SonneBornBerger = Convert.ToBoolean(sonneBornBerger)
    End Sub

    ReadOnly Property Gewinnsätze As Integer
        Get
            Return _Gewinnsätze
        End Get
    End Property
    ReadOnly Property SatzDifferenz As Boolean
        Get
            Return _SatzDifferenz
        End Get
    End Property
    ReadOnly Property SonneBornBerger As Boolean
        Get
            Return _SonneBornBerger
        End Get
    End Property

    Public Shared Function Parse(doc As XDocument, klassement As String) As SpielRegeln
        Dim competition = (From x In doc.Root.<competition>
                           Where x.Attribute("ttr-remarks").Value = klassement Select x).Single

        Dim GewinnsätzeAnzahl As Double = 3
        Dim SatzDiffCheck As Boolean = True
        Dim SonneBorn As Boolean = True

        If competition.@ppc:gewinnsätze IsNot Nothing Then
            GewinnsätzeAnzahl = Double.Parse(competition.@ppc:gewinnsätze)
        End If

        If competition.@ppc:satzdifferenz IsNot Nothing Then
            SatzDiffCheck = Boolean.Parse(competition.@ppc:satzdifferenz)
        End If

        If competition.@ppc:sonnebornberger IsNot Nothing Then
            SonneBorn = Boolean.Parse(competition.@ppc:sonnebornberger)
        End If

        Return New SpielRegeln(GewinnsätzeAnzahl, SatzDiffCheck, SonneBorn)
    End Function

End Class
