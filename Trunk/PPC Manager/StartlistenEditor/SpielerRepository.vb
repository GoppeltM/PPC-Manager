Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class SpielerRepository
    Inherits ObservableCollection(Of SpielerInfo)

    Private ReadOnly _Klassements As IEnumerable(Of XElement)
    Private Syncing As Boolean

    Public Sub New(klassements As IEnumerable(Of XElement))
        _Klassements = klassements
    End Sub

    Public Sub Sync()
        Syncing = True
        For Each klassement In _Klassements

            Dim neuerSpieler = Function(id As String, fremd As Boolean, person As XElement) As SpielerInfo
                                   Dim s = New SpielerInfo With {
                        .ID = id,
                        .Fremd = fremd,
                        .Geschlecht = CInt(person.@sex),
                        .Klassement = klassement.Attribute("age-group").Value,
                        .LizenzNr = CInt(person.Attribute("licence-nr").Value),
                        .Nachname = person.@lastname,
                        .Vorname = person.@firstname,
                        .TTR = CInt(person.@ttr),
                        .TTRMatchCount = CInt(person.Attribute("ttr-match-count")),
                        .Verein = person.Attribute("club-name").Value
                                    }
                                   Integer.TryParse(person.@birthyear, s.Geburtsjahr)
                                   Return s
                               End Function

            For Each xmlSpieler In klassement.<players>.<player>
                Dim id = xmlSpieler.@id
                Dim person = xmlSpieler.<person>.Single
                Add(neuerSpieler(id, False, person))
            Next
            For Each xmlSpieler In klassement.<players>.<ppc:player>
                Dim id = xmlSpieler.@id
                Dim person = xmlSpieler.<person>.Single
                Add(neuerSpieler(id, True, person))
            Next
        Next
        Syncing = False
    End Sub

    Protected Overrides Sub ClearItems()
        Throw New NotSupportedException
    End Sub

    Private Sub SpielerChanged(o As Object, e As PropertyChangedEventArgs)
        Dim spieler = DirectCast(o, SpielerInfo)
        Dim suche = (From klassement In _Klassements
                     From y In klassement.<players>.<player>.Concat(klassement.<players>.<ppc:player>)
                     Where y.@id = spieler.ID
                     Select y.<person>, klassement).First
        Select Case e.PropertyName
            Case "Bezahlt" : suche.person.@ppc:bezahlt = spieler.Bezahlt.ToString
            Case "Anwesend" : suche.person.@ppc:anwesend = spieler.Anwesend.ToString
            Case "Abwesend"
                suche.person.@ppc:abwesend = spieler.Abwesend.ToString
                Dim inaktivKnoten = From x In suche.klassement.<matches>.<ppc:inactiveplayer>
                                    Where x.@player = spieler.ID
                Dim istInaktiv = inaktivKnoten.Any
                If spieler.Abwesend = istInaktiv Then
                    Return
                End If
                If spieler.Abwesend Then
                    If Not suche.klassement.<matches>.Any Then
                        suche.klassement.Add(<matches/>)
                    End If
                    suche.klassement.<matches>.Single.Add(<ppc:inactivePlayer player=<%= spieler.ID %> group="0"/>)
                Else
                    inaktivKnoten.Remove
                End If
            Case Else : Throw New InvalidOperationException("Ungültige Änderung empfangen:" & e.PropertyName)
        End Select
    End Sub


    Protected Overrides Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
        Dim neue = If(e.NewItems, New List(Of SpielerInfo)).OfType(Of SpielerInfo)
        Dim alte = If(e.OldItems, New List(Of SpielerInfo)).OfType(Of SpielerInfo)
        For Each el In neue
            AddHandler el.PropertyChanged, AddressOf SpielerChanged
        Next
        If Syncing Then Return
        Select Case e.Action
            Case NotifyCollectionChangedAction.Add
                For Each el In neue
                    If Not el.Fremd Then Throw New InvalidOperationException("Nur Fremdspieler hinzufügbar")

                    Dim klassementNode = (From x In _Klassements
                                          Where x.Attribute("age-group").Value = el.Klassement).<players>.Single
                    Dim person = <person
                                     licence-nr=<%= el.LizenzNr %>
                                     club-name=<%= el.Verein %>
                                     sex=<%= el.Geschlecht %>
                                     ttr-match-count=<%= el.TTRMatchCount %>
                                     lastname=<%= el.Nachname %>
                                     ttr=<%= el.TTR %>
                                     firstname=<%= el.Vorname %>
                                     birthyear=<%= el.Geburtsjahr %>>
                                 </person>

                    klassementNode.Add(<ppc:player id=<%= el.ID %>>
                                           <%= person %>
                                       </ppc:player>)
                Next
            Case NotifyCollectionChangedAction.Remove
                For Each el In alte
                    If Not el.Fremd Then Throw New InvalidOperationException("Nur Fremdspieler löschbar")
                    Dim zuLöschen = (From x In _Klassements.<players>.<ppc:player>
                                     Where x.@id = el.ID).Single
                    zuLöschen.Remove()
                Next
            Case Else
                Throw New InvalidOperationException("Nicht unterstützt von dieser Collection")
        End Select
        MyBase.OnCollectionChanged(e)
    End Sub


End Class
