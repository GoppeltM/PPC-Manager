Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class Spieler
    Implements IComparable(Of Spieler), INotifyPropertyChanged

#Region "Public Properties"


    Public Sub New(spielRunden As SpielRunden, spielRegeln As SpielRegeln)
        _SpielRegeln = spielRegeln
        _SpielRunden = SpielRunden
    End Sub

    Protected Sub New(spieler As Spieler)
        _SpielRegeln = spieler._SpielRegeln
        _SpielRunden = spieler.SpielRunden
        Fremd = spieler.Fremd
        Geburtsjahr = spieler.Geburtsjahr
        Geschlecht = spieler.Geschlecht
        Id = spieler.Id
        Lizenznummer = spieler.Lizenznummer
        Nachname = spieler.Nachname
        TTRating = spieler.TTRating
        TTRMatchCount = spieler.TTRMatchCount
        Verbandsspitzname = spieler.Verbandsspitzname
        Vereinsname = spieler.Vereinsname
        Vereinsnummer = spieler.Vereinsnummer
        Vorname = spieler.Vorname
    End Sub


    Private ReadOnly _SpielRunden As SpielRunden
    Private ReadOnly _SpielRegeln As SpielRegeln


    Protected ReadOnly Property SpielRunden As SpielRunden
        Get
            Return _SpielRunden
        End Get
    End Property

    Protected ReadOnly Property SpielRegeln As SpielRegeln
        Get
            Return _SpielRegeln
        End Get
    End Property


    Public Property Id As String = "new"

    Private _Vorname As String = String.Empty

    Public Property Vorname As String
        Get
            Return _Vorname
        End Get
        Set(ByVal value As String)
            _Vorname = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Vorname"))
        End Set
    End Property

    Private _Nachname As String = String.Empty
    Public Property Nachname As String
        Get
            Return _Nachname
        End Get
        Set(ByVal value As String)
            _Nachname = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Nachname"))
        End Set
    End Property

    Private _Verein As String = String.Empty
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

    Public ReadOnly Property StartNummer As Integer
        Get
            Return Integer.Parse(Regex.Match(Id, "-?\d+\Z").Value)
        End Get
    End Property

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
            Return MeineGewonnenenSpiele.Count
        End Get
    End Property

    Private ReadOnly Property MeineGewonnenenSpiele As IEnumerable(Of SpielPartie)
        Get
            Dim GewonneneSpiele = From x In GespieltePartien Let Meine = x.MeineGewonnenenSätze(Me).Count
                             Where Meine >= _SpielRegeln.Gewinnsätze Select x

            Return GewonneneSpiele.ToList
        End Get
    End Property

    Private ReadOnly Property PunkteOhneFreilos As Integer
        Get
            Return Aggregate x In MeineGewonnenenSpiele Where Not TypeOf x Is FreiLosSpiel Into Count()
        End Get
    End Property

    Public ReadOnly Property BuchholzPunkte As Integer
        Get
            Dim _punkte = Aggregate x In GespieltePartien Where Not TypeOf x Is FreiLosSpiel Let y = x.MeinGegner(Me).PunkteOhneFreilos Into Sum(y)
            Return _punkte
        End Get
    End Property

    Public ReadOnly Property SonneBornBergerPunkte As Integer
        Get
            Dim _punkte = Aggregate x In MeineGewonnenenSpiele Where Not TypeOf x Is FreiLosSpiel Let y = x.MeinGegner(Me).PunkteOhneFreilos Into Sum(y)
            Return _punkte
        End Get
    End Property

    Property Fremd As Boolean

    Public Property Geburtsjahr As Integer

    Public Sub PunkteGeändert()
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Punkte"))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("BuchholzPunkte"))
    End Sub

    Protected Overridable ReadOnly Property GespieltePartien As IEnumerable(Of SpielPartie)
        Get
            Dim r = From x In SpielRunden.Reverse From y In x Where y.SpielerLinks = Me Or y.SpielerRechts = Me Select y
            Return r.ToList
        End Get
    End Property

    Public ReadOnly Property SätzeGewonnen As Integer
        Get
            Return Aggregate x In GespieltePartien Where Not TypeOf x Is FreiLosSpiel Into Sum(x.MeineGewonnenenSätze(Me).Count)
        End Get
    End Property

    Public ReadOnly Property SätzeVerloren As Integer
        Get
            Return Aggregate x In GespieltePartien Where Not TypeOf x Is FreiLosSpiel Into Sum(x.MeineVerlorenenSätze(Me).Count)
        End Get
    End Property

    Public ReadOnly Property SatzDifferenz As Integer
        Get
            Return SätzeGewonnen - SätzeVerloren
        End Get
    End Property

    Public ReadOnly Property Ausgeschieden As Boolean
        Get
            Return Aggregate x In SpielRunden.AusgeschiedeneSpieler Where x.Spieler = Me Into Any()
        End Get
    End Property

    Public Sub AusscheidenLassen()
        SpielRunden.AusgeschiedeneSpieler.Add(New Ausgeschieden With {.Spieler = Me, .Runde = SpielRunden.Count})
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
        Return String.Format("{0} {1}", Vorname, Nachname)
    End Function


    Public Function CompareTo(ByVal other As Spieler) As Integer Implements IComparable(Of Spieler).CompareTo
        Dim diff As Integer = 0
        diff = other.Ausgeschieden.CompareTo(Me.Ausgeschieden)
        If diff <> 0 Then Return diff
        diff = Me.Punkte - other.Punkte
        If diff <> 0 Then Return diff
        diff = Me.BuchholzPunkte - other.BuchholzPunkte
        If diff <> 0 Then Return diff

        If _SpielRegeln.SonneBornBerger Then
            diff = Me.SonneBornBergerPunkte - other.SonneBornBergerPunkte
            If diff <> 0 Then Return diff
        End If

        If _SpielRegeln.SatzDifferenz Then
            diff = Me.SatzDifferenz - other.SatzDifferenz
            If diff <> 0 Then Return diff
        End If
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


    Public Shared Operator <>(ByVal left As Spieler, ByVal right As Spieler) As Boolean
        Return Not left = right
    End Operator

    Public Shared Operator =(ByVal left As Spieler, ByVal right As Spieler) As Boolean
        Return left.Equals(right)
    End Operator

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged


End Class
