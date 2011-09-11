Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports <xmlns="http://PPCManager/SpeicherStand">

Public Class Spieler
    Implements IComparable(Of Spieler), INotifyPropertyChanged

#Region "Public Properties"

    Private _Vorname As String

    Public Sub New()
        Dim runden = CType(Application.Current.FindResource("SpielRunden"), SpielRunden)
        Me.SpielRunden = runden
    End Sub

    Protected Sub New(ByVal runden As SpielRunden)
        Me.SpielRunden = runden
    End Sub


    ''' <summary>
    ''' Darf nur einmalig gesetzt werden, und darf nur lesenderweise betreten werden!!
    ''' Der öffentliche Konstruktor existiert nur deshalb, weil das AddNewItem Event nicht mit unspezifierten Konstruktoren umgehen kann
    ''' </summary>
    ''' <remarks></remarks>
    Protected ReadOnly SpielRunden As SpielRunden


    Public Property Vorname As String
        Get
            Return _Vorname
        End Get
        Set(ByVal value As String)
            _Vorname = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Vorname"))
        End Set
    End Property

    Private _Nachname As String = "Mustermann"
    Public Property Nachname As String
        Get
            Return _Nachname
        End Get
        Set(ByVal value As String)
            _Nachname = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Nachname"))
        End Set
    End Property

    Private _Verein As String
    Public Property Verein As String
        Get
            Return _Verein
        End Get
        Set(ByVal value As String)
            _Verein = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Verein"))
        End Set
    End Property

    Private _SpielKlasse As String
    Public Property SpielKlasse As String
        Get
            Return _SpielKlasse
        End Get
        Set(ByVal value As String)
            _SpielKlasse = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("SpielKlasse"))
        End Set
    End Property


    Private _Position As Integer
    Public Property Position() As Integer
        Get
            Return _Position
        End Get
        Set(ByVal value As Integer)
            _Position = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Position"))
        End Set
    End Property


    Private _turnierKlasse As String
    Public Property TurnierKlasse() As String
        Get
            Return _turnierKlasse
        End Get
        Set(ByVal value As String)
            _turnierKlasse = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("TurnierKlasse"))
        End Set
    End Property


    Private _StartNummer As Integer
    Public Property StartNummer() As Integer
        Get
            Return _StartNummer
        End Get
        Set(ByVal value As Integer)
            _StartNummer = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("StartNummer"))
        End Set
    End Property

    Private _Rating As Integer
    Public Property Rating() As Integer
        Get
            Return _Rating
        End Get
        Set(ByVal value As Integer)
            _Rating = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Rating"))
        End Set
    End Property


    Private _Rang As Integer
    Public Property Rang() As Integer
        Get
            Return _Rang
        End Get
        Set(ByVal value As Integer)
            _Rang = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Rang"))
        End Set
    End Property



    Public ReadOnly Property Punkte As Integer
        Get
            Dim _punkte = Aggregate x In GespieltePartien
                          Where x.MeineGewonnenenSätze(Me).Count > x.MeineGewonnenenSätze(x.MeinGegner(Me)).Count
                          Into Count()

            Return _punkte
        End Get
    End Property

    Public ReadOnly Property BuchholzPunkte As Integer
        Get
            Dim _punkte = Aggregate x In GespieltePartien Let y = x.MeinGegner(Me).Punkte Into Sum(y)
            Return _punkte
        End Get        
    End Property

    Public ReadOnly Property GespieltePartien As List(Of SpielPartie)
        Get
            Dim r = From x In SpielRunden From y In x Where y.SpielerLinks = Me Or y.SpielerRechts = Me Select y
            Return r.ToList
        End Get
    End Property

    Public ReadOnly Property HatFreilos As Boolean
        Get
            Return Aggregate x In SpielRunden From y In x.OfType(Of FreiLosSpiel)() Into Any()
        End Get
    End Property

    Public ReadOnly Property SätzeGewonnen As Integer
        Get
            Return Aggregate x In GespieltePartien Into Sum(x.MeineGewonnenenSätze(Me).Count)
        End Get
    End Property

    Public ReadOnly Property SätzeVerloren As Integer
        Get
            Return Aggregate x In GespieltePartien Into Sum(x.MeineGewonnenenSätze(x.MeinGegner(Me)).Count)
        End Get
    End Property

    Private ReadOnly Property SatzDifferenz As Integer
        Get            
            Return SätzeGewonnen - SätzeVerloren
        End Get
    End Property

    Public ReadOnly Property Ausgeschieden As Boolean
        Get
            If Not SpielRunden.Any Then Return False
            Return SpielRunden.Peek.AusgeschiedeneSpieler.Contains(Me)
        End Get
    End Property

    Public Sub AusscheidenLassen()
        SpielRunden.Peek.AusgeschiedeneSpieler.Add(Me)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Ausgeschieden"))
    End Sub

#End Region

    Public Overrides Function Equals(ByVal obj As Object) As Boolean

        If Not TypeOf obj Is Spieler Then Return False
        Dim other = CType(obj, Spieler)
        Return Me.StartNummer = other.StartNummer
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return StartNummer.GetHashCode
    End Function

    Public Overrides Function ToString() As String
        Return Nachname
    End Function


    Public Function CompareTo(ByVal other As Spieler) As Integer Implements System.IComparable(Of Spieler).CompareTo
        Dim diff = other.Punkte - Me.Punkte
        If diff <> 0 Then Return diff
        diff = other.BuchholzPunkte - Me.BuchholzPunkte
        If diff <> 0 Then Return diff

        If My.Settings.SatzDifferenz Then
            diff = other.SatzDifferenz - Me.SatzDifferenz
            If diff <> 0 Then Return diff
        End If
        Return Me.StartNummer - other.StartNummer
    End Function


    Public Shared Operator <>(ByVal left As Spieler, ByVal right As Spieler) As Boolean
        Return Not left = right
    End Operator

    Public Shared Operator =(ByVal left As Spieler, ByVal right As Spieler) As Boolean
        Return left.Equals(right)
    End Operator

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged


    Public Function ToXML() As XElement
        Dim node = <Spieler Vorname=<%= Vorname %> Nachname=<%= Nachname %>
                       Verein=<%= Verein %> SpielKlasse=<%= SpielKlasse %>
                       StartNummer=<%= StartNummer %>
                       Position=<%= Position %> TurnierKlasse=<%= TurnierKlasse %>
                       ttRang=<%= Rang %>
                       ttRating=<%= Rating %>
                       >
                   </Spieler>
        Return node
    End Function

    Friend Shared Function FromXML(ByVal spielerNode As XElement) As Spieler
        Dim spieler As New Spieler()
        With spieler
            .Vorname = spielerNode.@Vorname
            .Nachname = spielerNode.@Nachname
            .Verein = spielerNode.@Verein
            .SpielKlasse = spielerNode.@SpielKlasse
            .StartNummer = CInt(spielerNode.@StartNummer)
            .Position = CInt(spielerNode.@Position)
            .TurnierKlasse = spielerNode.@TurnierKlasse
            .Rating = CInt(spielerNode.@ttRating)
            .Rang = CInt(spielerNode.@ttRang)
        End With
        Return spieler
    End Function

    Function HatBereitsGespieltGegen(ByVal zuprüfenderSpieler As Spieler) As Boolean
        Dim meineGegner = From x In GespieltePartien Select x.MeinGegner(Me)

        Return meineGegner.Contains(zuprüfenderSpieler)
    End Function

End Class
