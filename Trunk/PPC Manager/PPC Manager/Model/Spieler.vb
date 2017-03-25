Imports System.ComponentModel

Public Class Spieler
    Inherits SpielerInfo
    Implements IComparable(Of Spieler), INotifyPropertyChanged

#Region "Public Properties"

    Protected ReadOnly _Spielverlauf As ISpielverlauf(Of SpielerInfo)
    Private ReadOnly _Comparer As SpielerInfoComparer

    Public Sub New(spielverlauf As ISpielverlauf(Of SpielerInfo))
        _Spielverlauf = spielverlauf
        _Comparer = New SpielerInfoComparer(_Spielverlauf)
    End Sub

    Public Sub New(spieler As SpielerInfo, spielverlauf As ISpielverlauf(Of SpielerInfo))
        MyBase.New(spieler)
        _Spielverlauf = spielverlauf
        _Comparer = New SpielerInfoComparer(_Spielverlauf)
    End Sub

    Public Sub New(spieler As Spieler)
        MyBase.New(spieler)
        _Spielverlauf = spieler._Spielverlauf
        _Comparer = New SpielerInfoComparer(_Spielverlauf)
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

#End Region

    Public Function CompareTo(ByVal other As Spieler) As Integer Implements IComparable(Of Spieler).CompareTo
        Return _Comparer.Compare(Me, other)
    End Function

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged


End Class
