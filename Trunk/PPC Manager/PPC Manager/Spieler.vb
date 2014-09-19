Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class Spieler
    Implements IComparable(Of Spieler), INotifyPropertyChanged

#Region "Public Properties"


    Public Sub New(competition As Competition)
        _Competition = competition
    End Sub

    ''' <summary>
    ''' Darf nur einmalig gesetzt werden, und darf nur lesenderweise betreten werden!!
    ''' Der öffentliche Konstruktor existiert nur deshalb, weil das AddNewItem Event nicht mit unspezifierten Konstruktoren umgehen kann
    ''' </summary>
    ''' <remarks></remarks>
    Protected ReadOnly Property SpielRunden As SpielRunden
        Get
            Return _Competition.SpielRunden
        End Get
    End Property

    Private ReadOnly _Competition As Competition
    
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

    Private ReadOnly Property MeineGewonnenenSpieleExport As IEnumerable(Of SpielPartie)
        Get
            Dim GewonneneSpiele = From x In VergangenePartien Let Meine = x.MeineGewonnenenSätze(Me).Count
                             Where Meine >= _Competition.SpielRegeln.Gewinnsätze Select x

            Return GewonneneSpiele.ToList
        End Get
    End Property

    Private ReadOnly Property MeineGewonnenenSpiele As IEnumerable(Of SpielPartie)
        Get
            Dim GewonneneSpiele = From x In GespieltePartien Let Meine = x.MeineGewonnenenSätze(Me).Count
                             Where Meine >= _Competition.SpielRegeln.Gewinnsätze Select x

            Return GewonneneSpiele.ToList
        End Get
    End Property

    Public ReadOnly Property MeineSpieleDruck As IEnumerable(Of String)
        Get
            Dim l As New List(Of String)
            For Each s In GespieltePartien

                Dim text = s.MeinGegner(Me).StartNummer.ToString
                If s.Gewonnen(Me) Then
                    text &= "G"
                Else
                    text &= "V"
                End If
                l.Add(text)
            Next
            Return l
        End Get
    End Property

    ' 
    ' Diese beiden Einträge sind notwendig, weil zum Zeitpunkt der Paarung bereits die aktuellen Buchholzpunkte verändert werden.
    ' Ergo: ich brauche für den Export nur die Werte der Vergangenheit - exklusive der aktuellen Runde
#Region "Export Properties"

    Private ReadOnly Property VergangenePartien As IEnumerable(Of SpielPartie)
        Get
            Dim r = From x In SpielRunden.Skip(1).Reverse From y In x Where y.SpielerLinks = Me Or y.SpielerRechts = Me Select y
            Return r.ToList
        End Get
    End Property

    Public ReadOnly Property ExportPunkte As Integer
        Get
            Return MeineGewonnenenSpieleExport.Count
        End Get
    End Property

    Private ReadOnly Property ExportPunkteOhneFreilos As Integer
        Get
            Return Aggregate x In MeineGewonnenenSpieleExport Where Not TypeOf x Is FreiLosSpiel Into Count()
        End Get
    End Property

    Public ReadOnly Property ExportBHZ As Integer
        Get
            Dim _punkte = Aggregate x In VergangenePartien Where Not TypeOf x Is FreiLosSpiel Let y = x.MeinGegner(Me).ExportPunkteOhneFreilos Into Sum(y)
            Return _punkte
        End Get
    End Property

    Public ReadOnly Property ExportSonneborn As Integer
        Get
            Dim _punkte = Aggregate x In MeineGewonnenenSpieleExport Where Not TypeOf x Is FreiLosSpiel Let y = x.MeinGegner(Me).ExportPunkteOhneFreilos Into Sum(y)
            Return _punkte
        End Get
    End Property

    Public ReadOnly Property ExportSätzeGewonnen As Integer
        Get
            Return Aggregate x In VergangenePartien Where Not TypeOf x Is FreiLosSpiel Into Sum(x.MeineGewonnenenSätze(Me).Count)
        End Get
    End Property

    Public ReadOnly Property ExportSätzeVerloren As Integer
        Get
            Return Aggregate x In VergangenePartien Where Not TypeOf x Is FreiLosSpiel Into Sum(x.MeineVerlorenenSätze(Me).Count)
        End Get
    End Property
#End Region

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

    Public ReadOnly Property GespieltePartien As List(Of SpielPartie)
        Get
            Dim r = From x In SpielRunden.Reverse From y In x Where y.SpielerLinks = Me Or y.SpielerRechts = Me Select y
            Return r.ToList
        End Get
    End Property

    Public ReadOnly Property HatFreilos As Boolean
        Get
            Return Aggregate Runde In SpielRunden, Freilos In Runde.OfType(Of FreiLosSpiel)()
                   Where Freilos.SpielerLinks = Me Or Freilos.SpielerRechts = Me
                   Into Any()
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


    Public Function CompareTo(ByVal other As Spieler) As Integer Implements System.IComparable(Of Spieler).CompareTo        
        Dim diff As Integer = 0
        diff = other.Ausgeschieden.CompareTo(Me.Ausgeschieden)
        If diff <> 0 Then Return diff
        diff = Me.Punkte - other.Punkte
        If diff <> 0 Then Return diff
        diff = Me.BuchholzPunkte - other.BuchholzPunkte
        If diff <> 0 Then Return diff

        If _Competition.SpielRegeln.SonneBornBerger Then
            diff = Me.SonneBornBergerPunkte - other.SonneBornBergerPunkte
            If diff <> 0 Then Return diff
        End If

        If _Competition.SpielRegeln.SatzDifferenz Then
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

    Public Shared Function FromXML(ByVal spielerNode As XElement, competition As Competition) As Spieler
        Dim spieler As New Spieler(competition)        
        With spieler
            .Id = spielerNode.@id
            Dim ppc = spielerNode.GetNamespaceOfPrefix("ppc")
            .Fremd = ppc IsNot Nothing AndAlso ppc.NamespaceName = "http://www.ttc-langensteinbach.de/"
            spielerNode = spielerNode.<person>.First
            .Vorname = spielerNode.@firstname
            .Nachname = spielerNode.@lastname
            .TTRMatchCount = CInt(spielerNode.Attribute("ttr-match-count").Value)
            .Geschlecht = CInt(spielerNode.@sex)
            .Geburtsjahr = CInt(spielerNode.@birthyear)
            .Vereinsname = spielerNode.Attribute("club-name").Value
            .TTRating = CInt(spielerNode.@ttr)
            .Lizenznummer = CInt(spielerNode.Attribute("licence-nr").Value)
        End With
        Return spieler
    End Function

    Function HatBereitsGespieltGegen(ByVal zuprüfenderSpieler As Spieler) As Boolean
        Dim meineGegner = From x In GespieltePartien Select x.MeinGegner(Me)

        Return meineGegner.Contains(zuprüfenderSpieler)
    End Function

End Class
