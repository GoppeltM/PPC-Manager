Imports System.ComponentModel
Imports System.Collections.ObjectModel

''' <summary>
''' 
''' </summary>
''' <remarks>
''' Ein Spiel besteht aus den darin teilnehmenden Spielern, und den Ergebnissen die sie verbuchen konnten.
''' Jedes Ergebnis besteht aus 3 oder 5 Sätzen, und den Punkten die darin angehäuft wurden.
''' </remarks>
''' 
<DebuggerDisplay("Runde = {RundenName}")>
Public Class SpielPartie
    Inherits ObservableCollection(Of Satz)

    Protected ReadOnly _GewinnSätze As Integer
    Private ReadOnly Spieler As KeyValuePair(Of Spieler, Spieler)
    Private ReadOnly _RundenName As String
    Public ReadOnly Property RundenName As String
        Get
            Return _RundenName
        End Get
    End Property


    Public Sub New(rundenName As String, ByVal spielerLinks As Spieler, ByVal spielerRechts As Spieler, gewinnsätze As Integer)
        If spielerLinks Is Nothing Then Throw New ArgumentNullException
        If spielerRechts Is Nothing Then Throw New ArgumentNullException

        Spieler = New KeyValuePair(Of Spieler, Spieler)(spielerLinks, spielerRechts)
        _RundenName = rundenName
        _GewinnSätze = gewinnsätze
        AddHandler Me.CollectionChanged, Sub()
                                             Me.OnPropertyChanged(New PropertyChangedEventArgs("Abgeschlossen"))
                                         End Sub
    End Sub




    Public ReadOnly Property SpielerLinks As Spieler
        Get
            Return Spieler.Key
        End Get
    End Property

    Public ReadOnly Property SpielerRechts As Spieler
        Get
            Return Spieler.Value
        End Get
    End Property

    Public Sub Update()
        SpielerLinks.PunkteGeändert()
        SpielerRechts.PunkteGeändert()
    End Sub

    Public Property ZeitStempel As Date

    Public ReadOnly Property MeinGegner(ByVal ich As Spieler) As Spieler
        Get
            If Spieler.Key = ich Then
                Return Spieler.Value
            Else
                Return Spieler.Key
            End If
        End Get
    End Property

    Public ReadOnly Property Gewonnen(ich As Spieler) As Boolean
        Get
            If Abgeschlossen() AndAlso MeineGewonnenenSätze(ich).Count > MeineVerlorenenSätze(ich).Count Then Return True
            Return False
        End Get
    End Property

    Public Overridable ReadOnly Property Abgeschlossen() As Boolean
        Get
            Dim AbgeschlosseneSätzeLinks = Aggregate x In Me Where x.GewonnenLinks Into Count()

            Dim AbgeschlosseneSätzeRechts = Aggregate x In Me Where x.GewonnenRechts Into Count()

            Return Math.Max(AbgeschlosseneSätzeLinks, AbgeschlosseneSätzeRechts) >= _GewinnSätze
        End Get
    End Property

    Public Overridable ReadOnly Property MeineVerlorenenSätze(ByVal ich As Spieler) As IList(Of Satz)
        Get
            Dim verlorenLinks = From x In Me Where x.Abgeschlossen AndAlso x.PunkteRechts > x.PunkteLinks
                                Select x

            Dim verlorenRechts = From x In Me Where x.Abgeschlossen AndAlso x.PunkteLinks > x.PunkteRechts
                                 Select x

            If SpielerLinks = ich Then
                Return verlorenLinks.ToList
            Else
                Return verlorenRechts.ToList
            End If
        End Get
    End Property

    Public Overridable ReadOnly Property MeineGewonnenenSätze(ByVal ich As Spieler) As IList(Of Satz)
        Get
            Dim gewonnenLinks = From x In Me Where x.GewonnenLinks

            Dim gewonnenRechts = From x In Me Where x.GewonnenRechts

            If SpielerLinks = ich Then
                Return gewonnenLinks.ToList
            Else
                Return gewonnenRechts.ToList
            End If
        End Get
    End Property

    Overridable Function ToXML(matchNr As Integer) As XElement

        Dim SätzeLinks = MeineGewonnenenSätze(SpielerLinks).Count
        Dim SätzeRechts = MeineGewonnenenSätze(SpielerRechts).Count

        Dim GewonnenLinks = 0
        Dim GewonnenRechts = 0
        If (SätzeLinks > SätzeRechts) Then GewonnenLinks = 1
        If (SätzeLinks < SätzeRechts) Then GewonnenRechts = 1

        Dim SatzReihe = Function(ident As String, ergebnisse As IEnumerable(Of Integer)) As IEnumerable(Of XAttribute)
                            Dim attributes = (From x In Enumerable.Range(1, 7) Select New XAttribute(String.Format("set-{0}-{1}", ident, x), 0)).ToList
                            Dim current = 0
                            For Each ergebnis In ergebnisse
                                attributes(current).Value = ergebnis.ToString
                                current += 1
                            Next
                            Return attributes
                        End Function

        Dim node = <match player-a=<%= SpielerLinks.Id %> player-b=<%= SpielerRechts.Id %>
                       games-a=<%= Aggregate x In Me Into Sum(x.PunkteLinks) %> games-b=<%= Aggregate x In Me Into Sum(x.PunkteRechts) %>
                       sets-a=<%= SätzeLinks %> sets-b=<%= SätzeRechts %>
                       matches-a=<%= GewonnenLinks %> matches-b=<%= GewonnenRechts %>
                       scheduled=<%= ZeitStempel.ToString("yyyy-MM-dd HH:mm") %>
                       group=<%= RundenName %> nr=<%= matchNr %>
                       <%= SatzReihe("a", From x In Me Select x.PunkteLinks) %>
                       <%= SatzReihe("b", From x In Me Select x.PunkteRechts) %>
                   />

        If SpielerLinks.Fremd Or SpielerRechts.Fremd Then
            Dim ns = XNamespace.Get("http://www.ttc-langensteinbach.de")
            node.Name = ns + "match"
            node.Add(New XAttribute(XNamespace.Xmlns + "ppc", ns.NamespaceName))
        End If
        Return node
    End Function


    Shared Function FromXML(ByVal spielerListe As IEnumerable(Of Spieler), ByVal xSpielPartie As XElement, gewinnsätze As Integer) As SpielPartie
        Dim spielerA = (From x In spielerListe Where x.Id = xSpielPartie.Attribute("player-a").Value Select x).First
        Dim spielerB = (From x In spielerListe Where x.Id = xSpielPartie.Attribute("player-b").Value Select x).First

        Dim partie As New SpielPartie(xSpielPartie.@group, spielerA, spielerB, gewinnsätze)

        Dim SätzeA = From x In xSpielPartie.Attributes Where x.Name.LocalName.Contains("set-a") Order By x.Name.LocalName Ascending

        Dim SätzeB = From x In xSpielPartie.Attributes Where x.Name.LocalName.Contains("set-b") Order By x.Name.LocalName Ascending

        For Each Satz In SätzeA.Zip(SätzeB, Function(x, y) New Satz With {.PunkteLinks = CInt(x.Value), .PunkteRechts = CInt(y.Value)}) _
            .Where(Function(s) s.PunkteLinks <> 0 Or s.PunkteRechts <> 0)
            partie.Add(Satz)
        Next

        partie.ZeitStempel = Date.Parse(xSpielPartie.@scheduled)

        Return partie
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim other = TryCast(obj, SpielPartie)
        If other Is Nothing Then Return False
        If Me.SpielerLinks <> other.SpielerLinks Then Return False
        If Me.SpielerRechts <> other.SpielerRechts Then Return False
        If Me.RundenName <> other.RundenName Then Return False
        Return True
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return SpielerLinks.GetHashCode Xor SpielerRechts.GetHashCode Xor RundenName.GetHashCode
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0} : {1}", SpielerLinks, SpielerRechts)
    End Function

End Class
