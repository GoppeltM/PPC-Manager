Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class Spieler
    Implements IComparable(Of Spieler), INotifyPropertyChanged

#Region "Public Properties"


    ''' <summary>
    ''' Darf nur einmalig gesetzt werden, und darf nur lesenderweise betreten werden!!
    ''' Der öffentliche Konstruktor existiert nur deshalb, weil das AddNewItem Event nicht mit unspezifierten Konstruktoren umgehen kann
    ''' </summary>
    ''' <remarks></remarks>
    Protected SpielRunden As New SpielRunden

    Public Property Id As String = "new"

    Private _Vorname As String

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
    Public Property Vereinsname As String
        Get
            Return _Verein
        End Get
        Set(ByVal value As String)
            _Verein = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Verein"))
        End Set
    End Property

    Public Property Vereinsnummer As Integer

    Public Property Verbandsspitzname As String

    Public Property Lizenznummer As Integer

    Private _StartNummer As Integer

    Private _Rating As Integer
    Public Property TTRating() As Integer
        Get
            Return _Rating
        End Get
        Set(ByVal value As Integer)
            _Rating = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Rating"))
        End Set
    End Property

    Public Property TTRMatchCount As Integer

    Public Property Geschlecht As Integer

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

    Property Fremd As Boolean

    Public Sub PunkteGeändert()
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Punkte"))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("BuchholzPunkte"))
    End Sub

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
        Return Me.Id = other.Id
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Id.GetHashCode
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
        Return Me.TTRating - other.TTRating
    End Function


    Public Shared Operator <>(ByVal left As Spieler, ByVal right As Spieler) As Boolean
        Return Not left = right
    End Operator

    Public Shared Operator =(ByVal left As Spieler, ByVal right As Spieler) As Boolean
        Return left.Equals(right)
    End Operator

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Public Shared Function FromXML(ByVal spielerNode As XElement, spielRunden As SpielRunden) As Spieler
        Dim spieler As New Spieler()
        With spieler
            .Id = spielerNode.@id
            Dim ppc = spielerNode.GetNamespaceOfPrefix("ppc")
            .Fremd = ppc IsNot Nothing AndAlso ppc.NamespaceName = "http://www.ttc-langensteinbach.de/"
            .SpielRunden = spielRunden
            spielerNode = spielerNode.<person>.First
            .Vorname = spielerNode.@firstname
            .Nachname = spielerNode.@lastname
            .TTRMatchCount = CInt(spielerNode.Attribute("ttr-match-count").Value)
            .Geschlecht = CInt(spielerNode.@sex)
            .Vereinsname = spielerNode.Attribute("club-name").Value
            .Lizenznummer = CInt(spielerNode.Attribute("licence-nr").Value)
            .TTRating = CInt(spielerNode.@ttr)
        End With
        Return spieler
    End Function

    Function HatBereitsGespieltGegen(ByVal zuprüfenderSpieler As Spieler) As Boolean
        Dim meineGegner = From x In GespieltePartien Select x.MeinGegner(Me)

        Return meineGegner.Contains(zuprüfenderSpieler)
    End Function

End Class
