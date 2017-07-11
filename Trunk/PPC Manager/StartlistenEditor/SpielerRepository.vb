Imports System.Collections.Specialized
Imports System.ComponentModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class SpielerRepository

    Private ReadOnly _Speicher As ISpeicher
    Private ReadOnly _NotifyCollectionChanged As INotifyCollectionChanged
    Private ReadOnly _Liste As IEnumerable(Of SpielerInfo)

    Private Sub OnSpielerEvent(o As Object, args As PropertyChangedEventArgs)
        Dim spieler As SpielerInfo = CType(o, SpielerInfo)
        _Speicher.Speichere(Sub(x) SpielerChanged(x, spieler, args))
    End Sub

    Public Sub New(speicher As ISpeicher, notifycollectionChanged As INotifyCollectionChanged, list As IEnumerable(Of SpielerInfo))
        _Speicher = speicher
        _NotifyCollectionChanged = notifycollectionChanged
        _Liste = list
        AddHandler notifycollectionChanged.CollectionChanged, AddressOf OnCollectionChanged
        For Each el In list
            AddHandler el.PropertyChanged, AddressOf OnSpielerEvent
        Next
    End Sub

    Public Sub Deregister()
        RemoveHandler _NotifyCollectionChanged.CollectionChanged, AddressOf OnCollectionChanged
        For Each s In _Liste
            RemoveHandler s.PropertyChanged, AddressOf OnSpielerEvent
        Next
    End Sub

    Private Shared Sub SpielerChanged(klassements As IEnumerable(Of XElement),
                                      spieler As SpielerInfo,
                                      e As PropertyChangedEventArgs)
        Dim suche = (From klassement In klassements
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
                    suche.klassement.<matches>.Single.Add(<ppc:inactiveplayer player=<%= spieler.ID %> group="0"/>)
                Else
                    inaktivKnoten.Remove
                End If
            Case Else : Throw New InvalidOperationException("Ungültige Änderung empfangen:" & e.PropertyName)
        End Select
    End Sub

    Private Sub FügeKnotenHinzu(neue As IEnumerable(Of SpielerInfo), klassements As IEnumerable(Of XElement))
        For Each el In neue
            If Not el.Fremd Then Throw New InvalidOperationException("Nur Fremdspieler hinzufügbar")

            Dim klassementNode = (From x In klassements
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
    End Sub

    Private Sub OnCollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs)
        Dim neue = If(e.NewItems, New List(Of SpielerInfo)).OfType(Of SpielerInfo)
        Dim alte = If(e.OldItems, New List(Of SpielerInfo)).OfType(Of SpielerInfo)

        For Each el In neue
            AddHandler el.PropertyChanged, AddressOf OnSpielerEvent
        Next

        For Each el In alte
            RemoveHandler el.PropertyChanged, AddressOf OnSpielerEvent
        Next

        Select Case e.Action
            Case NotifyCollectionChangedAction.Add
                _Speicher.Speichere(Sub(klassements) FügeKnotenHinzu(neue, klassements))
            Case NotifyCollectionChangedAction.Remove
                _Speicher.Speichere(Sub(klassements)
                                        For Each el In alte
                                            If Not el.Fremd Then Throw New InvalidOperationException("Nur Fremdspieler löschbar")
                                            Dim klassement = (From x In klassements Where x.Attribute("age-group").Value = el.Klassement).First
                                            Dim zuLöschen = (From x In klassement.<players>.<ppc:player>
                                                             Where x.@id = el.ID)
                                            zuLöschen.Remove()
                                            Dim inaktivKnoten = (From x In klassement.<matches>.<ppc:inactiveplayer>
                                                                 Where x.@player = el.ID).FirstOrDefault
                                            If inaktivKnoten IsNot Nothing Then
                                                inaktivKnoten.Remove()
                                            End If

                                        Next
                                    End Sub)
            Case Else
                Throw New InvalidOperationException("Nicht unterstützt von dieser Collection")
        End Select
    End Sub


End Class
