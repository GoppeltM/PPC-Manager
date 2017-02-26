Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class SpielerRepository
    Inherits ObservableCollection(Of SpielerInfo)

    Private ReadOnly _Doc As XDocument
    Private Syncing As Boolean

    Public Sub New(doc As XDocument)
        _Doc = doc
    End Sub

    Public Sub Sync()
        Syncing = True
        For Each klassement In _Doc.Root.<competition>
            For Each xmlSpieler In klassement.<players>.<player>
                Dim id = xmlSpieler.@id
                Dim person = xmlSpieler.<person>.Single
                Dim spieler = New SpielerInfo With {
                        .ID = id,
                        .Fremd = False,
                        .Geburtsjahr = CInt(person.@birthyear),
                        .Geschlecht = CInt(person.@sex),
                        .Klassement = klassement.Attribute("age-group").Value,
                        .LizenzNr = CInt(person.Attribute("licence-nr").Value),
                        .Nachname = person.@lastname,
                        .Vorname = person.@firstname,
                        .TTR = CInt(person.@ttr),
                        .TTRMatchCount = CInt(person.Attribute("ttr-match-count")),
                        .Verein = person.Attribute("club-name").Value
                }
                Add(spieler)
            Next
        Next
        Syncing = False
    End Sub

    Protected Overrides Sub ClearItems()
        Dim players = _Doc.Root.<competition>.<players>
        players.Remove()
        MyBase.ClearItems()
    End Sub

    Private Sub SpielerChanged(o As Object, e As PropertyChangedEventArgs)
        Dim spieler = DirectCast(o, SpielerInfo)
        Dim playersKnoten = _Doc.Root.<competition>.<players>
        Dim alleSpieler = playersKnoten.<player>.Concat(playersKnoten.<ppc:player>)
        Dim xmlKnoten = (From x In alleSpieler Where x.@id = spieler.ID Select x.<person>).First
        Select Case e.PropertyName
            Case "Bezahlt" : xmlKnoten.@ppc:bezahlt = spieler.Bezahlt.ToString
            Case "Anwesend" : xmlKnoten.@ppc:anwesend = spieler.Anwesend.ToString
            Case "Abwesend" : xmlKnoten.@ppc:abwesend = spieler.Abwesend.ToString
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

                    Dim klassementNode = (From x In _Doc.Root.<competition>
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
                    Dim zuLöschen = (From x In _Doc.Root.<competition>.<players>.<ppc:player>
                                     Where x.@id = el.ID).Single
                    zuLöschen.Remove()
                Next
            Case Else
                Throw New InvalidOperationException("Nicht unterstützt von dieser Collection")
        End Select
        MyBase.OnCollectionChanged(e)
    End Sub


End Class
