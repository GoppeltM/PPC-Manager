Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class Spieler
    Inherits SpielerInfo
    Implements IComparable(Of Spieler), INotifyPropertyChanged

#Region "Public Properties"

    Protected ReadOnly _Spielverlauf As ISpielverlauf(Of SpielerInfo)

    Public Sub New(spielverlauf As ISpielverlauf(Of SpielerInfo))
        _Spielverlauf = spielverlauf
    End Sub

    Public Sub New(spieler As SpielerInfo, spielverlauf As ISpielverlauf(Of SpielerInfo))
        MyBase.New(spieler)
        _Spielverlauf = spielverlauf
    End Sub

    Public Sub New(spieler As Spieler)
        MyBase.New(spieler)
        _Spielverlauf = spieler._Spielverlauf
    End Sub

    Public ReadOnly Property Punkte As Integer
        Get
            Return _Spielverlauf.BerechnePunkte(Me)
        End Get
    End Property

    Public ReadOnly Property BuchholzPunkte As Integer
        Get
            Return _Spielverlauf.BerechneBuchholzPunkte(Me)
        End Get
    End Property

    Public ReadOnly Property SonneBornBergerPunkte As Integer
        Get
            Return _Spielverlauf.BerechneSonnebornBergerPunkte(Me)
        End Get
    End Property

    Public Sub PunkteGeändert()
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Punkte"))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("BuchholzPunkte"))
    End Sub

    Public ReadOnly Property SätzeGewonnen As Integer
        Get
            Return _Spielverlauf.BerechneGewonneneSätze(Me)
        End Get
    End Property

    Public ReadOnly Property SätzeVerloren As Integer
        Get
            Return _Spielverlauf.BerechneVerloreneSätze(Me)
        End Get
    End Property

    Public ReadOnly Property SatzDifferenz As Integer
        Get
            Return SätzeGewonnen - SätzeVerloren
        End Get
    End Property

    Public ReadOnly Property Ausgeschieden As Boolean
        Get
            Return _Spielverlauf.IstAusgeschieden(Me)
        End Get
    End Property

    Public Sub AusscheidenLassen()
        ' SpielRunden.AusgeschiedeneSpieler.Add(New Ausgeschieden With {.Spieler = Me, .Runde = SpielRunden.Count})
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Ausgeschieden"))
    End Sub

#End Region

    Public Function CompareTo(ByVal other As Spieler) As Integer Implements IComparable(Of Spieler).CompareTo
        Dim diff As Integer = 0
        diff = other.Ausgeschieden.CompareTo(Me.Ausgeschieden)
        If diff <> 0 Then Return diff
        diff = Me.Punkte - other.Punkte
        If diff <> 0 Then Return diff
        diff = Me.BuchholzPunkte - other.BuchholzPunkte
        If diff <> 0 Then Return diff

        diff = Me.SonneBornBergerPunkte - other.SonneBornBergerPunkte
        If diff <> 0 Then Return diff
        diff = Me.SatzDifferenz - other.SatzDifferenz
        If diff <> 0 Then Return diff
        diff = Me.TTRating - other.TTRating
        If diff <> 0 Then Return diff
        diff = Me.TTRMatchCount - other.TTRMatchCount
        If diff <> 0 Then Return diff
        diff = other.Nachname.CompareTo(Me.Nachname)
        If diff <> 0 Then Return diff
        diff = other.Vorname.CompareTo(Me.Vorname)
        If diff <> 0 Then Return diff
        Return Me.Lizenznummer - other.Lizenznummer
    End Function



    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged


End Class
