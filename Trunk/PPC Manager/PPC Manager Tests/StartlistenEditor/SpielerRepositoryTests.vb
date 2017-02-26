Imports StartlistenEditor
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">

Public Class SpielerRepositoryTests

    <SetUp>
    Public Sub DummyDokument()
        _BKlasse = <competition age-group="B-Klasse" xmlns:ppc="http://www.ttc-langensteinbach.de">
                       <players></players>
                   </competition>
        _Doc = New XDocument(<tournament>
                                 <competition age-group="A-Klasse" xmlns:ppc="http://www.ttc-langensteinbach.de">
                                     <players></players>
                                 </competition>
                                 <%= _BKlasse %>
                             </tournament>)
    End Sub

    Private Property _BKlasse As XElement
    Private Property _Doc As XDocument

    <Test>
    Public Sub Add_fügt_Xml_Knoten_im_richtigen_Klassement_ein()

        Dim r = New SpielerRepository(_Doc)
        r.Add(New SpielerInfo With {
                .ID = "MeineID",
                .Klassement = "B-Klasse",
                .LizenzNr = -41,
                .Verein = "TS Durlach",
                .Geschlecht = 1,
                .TTRMatchCount = 123,
                .TTR = 45,
                .Nachname = "Mustermann",
                .Vorname = "Dr. Max",
                .Geburtsjahr = 1987,
                .Fremd = True
              })
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
        Dim r = New SpielerRepository(_Doc)
        Dim s As New SpielerInfo With {
                .ID = "MeineId",
                .Klassement = "B-Klasse",
                .Fremd = True
            }
        r.Add(s)
        r.Remove(s)

        Assert.That(_BKlasse.<players>.Elements().Count, Iz.EqualTo(0))
    End Sub

    <Test>
    Public Sub Sync_legt_Spieler_für_XML_Daten_an()
        ' Arrange
        _BKlasse.<players>.First.Add(<player id="ID123">
                                         <person
                                             licence-nr="45"
                                             club-name="TS Durlach"
                                             sex="0"
                                             ttr-match-count="55"
                                             ttr="12"
                                             firstname="Max"
                                             lastname="Mustermann"
                                             birthyear="1981"
                                             >
                                         </person>
                                     </player>)
        Dim r = New SpielerRepository(_Doc)

        ' Act
        r.Sync()

        ' Assert
        Assert.That(r.Count, [Is].EqualTo(1))
        Dim p = r.First()
        Assert.That(p.Fremd, Iz.EqualTo(False))
        Assert.That(p.Geburtsjahr, Iz.EqualTo(1981))
        Assert.That(p.Geschlecht, Iz.EqualTo(0))
        Assert.That(p.ID, Iz.EqualTo("ID123"))
        Assert.That(p.Klassement, Iz.EqualTo("B-Klasse"))
        Assert.That(p.LizenzNr, Iz.EqualTo(45))
        Assert.That(p.Nachname, Iz.EqualTo("Mustermann"))
        Assert.That(p.TTR, Iz.EqualTo(12))
        Assert.That(p.TTRMatchCount, Iz.EqualTo(55))
        Assert.That(p.Verein, Iz.EqualTo("TS Durlach"))
        Assert.That(p.Vorname, Iz.EqualTo("Max"))
    End Sub

    <Test>
    Public Sub Änderung_an_Bezahlt_legt_Xml_property_an()
        ' Arrange
        Dim r = New SpielerRepository(_Doc)
        Dim spieler = New SpielerInfo With {
              .ID = "A12",
              .Fremd = True,
              .Klassement = "B-Klasse"
              }
        r.Add(spieler)
        ' Act
        spieler.Bezahlt = True

        ' Assert
        Dim ergebnis = _BKlasse.<players>.<ppc:player>.<person>.First
        Assert.That(ergebnis.@ppc:bezahlt, Iz.EqualTo("True"))
    End Sub

    Private Function GetDummySpieler() As SpielerInfo
        Dim spieler = New SpielerInfo With {
              .ID = "A12",
              .Fremd = True,
              .Klassement = "B-Klasse"
              }
        Return spieler
    End Function

    <Test>
    Public Sub Änderung_an_Anwesend_legt_Xml_property_an()
        ' Arrange
        Dim r = New SpielerRepository(_Doc)
        Dim spieler = GetDummySpieler()
        r.Add(spieler)
        ' Act
        spieler.Anwesend = True

        ' Assert
        Dim ergebnis = _BKlasse.<players>.<ppc:player>.<person>.First
        Assert.That(ergebnis.@ppc:anwesend, Iz.EqualTo("True"))
    End Sub

    <Test>
    Public Sub Änderung_an_Abwesend_legt_Xml_property_an()
        ' Arrange
        Dim r = New SpielerRepository(_Doc)
        Dim spieler = GetDummySpieler()
        r.Add(spieler)
        ' Act
        spieler.Abwesend = True

        ' Assert
        Dim ergebnis = _BKlasse.<players>.<ppc:player>.<person>.First
        Assert.That(ergebnis.@ppc:abwesend, Iz.EqualTo("True"))
    End Sub
End Class
