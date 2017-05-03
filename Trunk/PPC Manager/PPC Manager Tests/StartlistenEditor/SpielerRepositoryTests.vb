Imports System.Collections.Specialized
Imports Moq
Imports StartlistenEditor
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class SpielerRepositoryTests

    <SetUp>
    Public Sub DummyDokument()
        Dim speicher = New Mock(Of ISpeicher)
        speicher.SetupGet(Function(m) m.KlassementNamen).Returns({"A-Klasse", "B-Klasse"})
        _Observable = New Mock(Of INotifyCollectionChanged)
        _Repository = New SpielerRepository(speicher.Object, _Observable.Object, New List(Of SpielerInfo))
        _BKlasse = <competition age-group="B-Klasse" xmlns:ppc="http://www.ttc-langensteinbach.de">
                       <players></players>
                   </competition>
        speicher.Setup(Sub(m) m.Speichere(It.IsAny(Of Veränderung))).Callback(Of Veränderung)(Sub(m) m({_BKlasse}))
    End Sub

    Private Property _BKlasse As XElement
    Private Property _Repository As SpielerRepository
    Private Property _Observable As Mock(Of INotifyCollectionChanged)

    <Test>
    Public Sub Add_fügt_Xml_Knoten_im_richtigen_Klassement_ein()

        _Observable.Raise(
        Sub(m) AddHandler m.CollectionChanged, Nothing,
                 New NotifyCollectionChangedEventArgs(
                 NotifyCollectionChangedAction.Add,
                 New SpielerInfo With {
                    .ID = "MeineID",
                    .Fremd = True,
                    .Klassement = "B-Klasse",
                    .LizenzNr = -41,
                    .Geburtsjahr = 1987,
                    .Geschlecht = 1,
                    .Vorname = "Dr. Max",
                    .Nachname = "Mustermann",
                    .TTR = 45,
                    .TTRMatchCount = 123,
                    .Verein = "TS Durlach"
                 }))
        Dim xmlSpieler = _BKlasse.<players>.<ppc:player>.First()
        Assert.That(xmlSpieler.<person>.Count(), Iz.EqualTo(1))
        Assert.That(xmlSpieler.Attribute("id").Value, Iz.EqualTo("MeineID"))
        Dim getValue = Function(name As String) xmlSpieler.<person>.First().Attribute(name).Value
        Assert.That(getValue("licence-nr"), Iz.EqualTo("-41"))
        Assert.That(getValue("club-name"), Iz.EqualTo("TS Durlach"))
        Assert.That(getValue("sex"), Iz.EqualTo("1"))
        Assert.That(getValue("ttr-match-count"), Iz.EqualTo("123"))
        Assert.That(getValue("lastname"), Iz.EqualTo("Mustermann"))
        Assert.That(getValue("ttr"), Iz.EqualTo("45"))
        Assert.That(getValue("firstname"), Iz.EqualTo("Dr. Max"))
        Assert.That(getValue("birthyear"), Iz.EqualTo("1987"))
    End Sub

    <Test>
    Public Sub Remove_entfernt_Xml_Knoten_aus_richtigem_Klassement()
        Dim spieler = New SpielerInfo With {
                    .ID = "MeineID",
                    .Fremd = True,
                    .Klassement = "B-Klasse"
                 }
        _Observable.Raise(
        Sub(m) AddHandler m.CollectionChanged, Nothing,
                 New NotifyCollectionChangedEventArgs(
                 NotifyCollectionChangedAction.Add,
                 spieler))
        _Observable.Raise(
        Sub(m) AddHandler m.CollectionChanged, Nothing,
                 New NotifyCollectionChangedEventArgs(
                 NotifyCollectionChangedAction.Remove,
                 spieler
))
        Assert.That(_BKlasse.<players>.Elements().Count, Iz.EqualTo(0))
    End Sub

    <Test>
    Public Sub Änderung_an_Bezahlt_legt_Xml_property_an()
        ' Arrange
        Dim spieler = New SpielerInfo With {
                    .ID = "A12",
                    .Fremd = True,
                    .Klassement = "B-Klasse"
                 }
        _Observable.Raise(
        Sub(m) AddHandler m.CollectionChanged, Nothing,
                 New NotifyCollectionChangedEventArgs(
                 NotifyCollectionChangedAction.Add, spieler))
        ' Act
        spieler.Bezahlt = True

        ' Assert
        Dim ergebnis = _BKlasse.<players>.<ppc:player>.<person>.First
        Assert.That(ergebnis.@ppc:bezahlt, Iz.EqualTo("True"))
    End Sub

    <Test>
    Public Sub Änderung_an_Anwesend_legt_Xml_property_an()
        ' Arrange
        Dim spieler = New SpielerInfo With {
                    .ID = "A12",
                    .Fremd = True,
                    .Klassement = "B-Klasse"
                 }
        _Observable.Raise(
        Sub(m) AddHandler m.CollectionChanged, Nothing,
                 New NotifyCollectionChangedEventArgs(
                 NotifyCollectionChangedAction.Add, spieler))
        ' Act
        spieler.Anwesend = True

        ' Assert
        Dim ergebnis = _BKlasse.<players>.<ppc:player>.<person>.First
        Assert.That(ergebnis.@ppc:anwesend, Iz.EqualTo("True"))
    End Sub

    <Test>
    Public Sub Änderung_an_Abwesend_legt_Xml_property_an()
        ' Arrange
        Dim spieler = New SpielerInfo With {
                    .ID = "A12",
                    .Fremd = True,
                    .Klassement = "B-Klasse"
                 }
        _Observable.Raise(
        Sub(m) AddHandler m.CollectionChanged, Nothing,
                 New NotifyCollectionChangedEventArgs(
                 NotifyCollectionChangedAction.Add, spieler))
        ' Act
        spieler.Abwesend = True

        ' Assert
        Dim ergebnis = _BKlasse.<players>.<ppc:player>.<person>.First
        Assert.That(ergebnis.@ppc:abwesend, Iz.EqualTo("True"))
    End Sub

    <Test>
    Public Sub Wenn_Abwesend_dann_Spieler_ausgeschieden_in_Runde_0()
        ' Arrange
        Dim spieler = New SpielerInfo With {
                    .ID = "A12",
                    .Fremd = True,
                    .Klassement = "B-Klasse"
                 }
        _Observable.Raise(
        Sub(m) AddHandler m.CollectionChanged, Nothing,
                 New NotifyCollectionChangedEventArgs(
                 NotifyCollectionChangedAction.Add, spieler))
        ' Act
        spieler.Abwesend = True
        ' Assert
        Dim ergebnis = _BKlasse.<matches>.<ppc:inactiveplayer>.Single
        Assert.That(ergebnis.@player, Iz.EqualTo("A12"))
        Assert.That(ergebnis.@group, Iz.EqualTo("0"))
    End Sub

    <Test>
    Sub Stammspieler_werden_aktualisiert()
        Dim speicher = New Mock(Of ISpeicher)
        speicher.SetupGet(Function(m) m.KlassementNamen).Returns({"A-Klasse", "B-Klasse"})
        Dim spieler = New SpielerInfo With {
            .Klassement = "B-Klasse",
            .ID = "123",
            .LizenzNr = 123}

        Dim repository = New SpielerRepository(speicher.Object, _Observable.Object,
                                               New List(Of SpielerInfo) From {spieler})
        _BKlasse = <competition age-group="B-Klasse" xmlns:ppc="http://www.ttc-langensteinbach.de">
                       <players>
                           <player id="123">
                               <person></person>
                           </player>
                       </players>
                   </competition>
        speicher.Setup(Sub(m) m.Speichere(It.IsAny(Of Veränderung))).Callback(Of Veränderung)(Sub(m) m({_BKlasse}))
        spieler.Abwesend = True
        Dim result = _BKlasse.<players>.<player>.<person>.@ppc:abwesend
        Assert.That(result, [Is].EqualTo("True"))
    End Sub
End Class
