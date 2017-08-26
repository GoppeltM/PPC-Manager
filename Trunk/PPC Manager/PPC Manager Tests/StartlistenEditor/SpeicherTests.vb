Imports StartlistenEditor

Public Class SpeicherTests

    <Test>
    Public Sub LeseSpieler_setzt_fehlende_TTR_Daten_auf_Null()
        ' Arrange
        Dim _BKlasse = <competition age-group="B-Klasse" xmlns:ppc="http://www.ttc-langensteinbach.de">
                           <players>
                               <player id="ID123">
                                   <person
                                       licence-nr="45"
                                       club-name="TS Durlach"
                                       sex="0"
                                       firstname="Max"
                                       lastname="Mustermann"
                                       birthyear="1981"
                                       >
                                   </person>
                               </player>
                           </players>
                       </competition>
        Dim dateisystem = Mock.Of(Of IDateisystem)(
                Function(m) m.LadeXml() Is New XDocument(<root>
                                                             <%= _BKlasse %>
                                                         </root>)
            )

        Dim r = New Speicher(dateisystem)
        ' Act
        Dim l = r.LeseSpieler
        ' Assert
        Assert.That(l.First.TTR, [Is].EqualTo(0))
        Assert.That(l.First.TTRMatchCount, [Is].EqualTo(0))
    End Sub

    <Test>
    Public Sub LeseSpieler_parst_alle_Spielerdaten()
        ' Arrange
        Dim _BKlasse = <competition age-group="B-Klasse" xmlns:ppc="http://www.ttc-langensteinbach.de">
                           <players>
                               <player id="ID123">
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
                               </player>
                           </players>
                       </competition>
        Dim dateisystem = Mock.Of(Of IDateisystem)(
                Function(m) m.LadeXml() Is New XDocument(<root>
                                                             <%= _BKlasse %>
                                                         </root>)
            )

        Dim r = New Speicher(dateisystem)
        ' Act
        Dim l = r.LeseSpieler

        ' Assert
        Assert.That(l.Count, [Is].EqualTo(1))
        Dim p = l.First()
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
    Public Sub LeseSpieler_wirft_UngültigeSpielerException_bei_falscher_LizenzNr()
        ' Arrange
        Dim _BKlasse = <competition age-group="B-Klasse" xmlns:ppc="http://www.ttc-langensteinbach.de">
                           <players>
                               <player id="ID123">
                                   <person
                                       licence-nr="INVALID"
                                       club-name="TS Durlach"
                                       sex="0"
                                       ttr-match-count="55"
                                       ttr="12"
                                       firstname="Max"
                                       lastname="Mustermann"
                                       birthyear="1981"
                                       >
                                   </person>
                               </player>
                           </players>
                       </competition>
        Dim dateisystem = Mock.Of(Of IDateisystem)(
                Function(m) m.LadeXml() Is New XDocument(<root>
                                                             <%= _BKlasse %>
                                                         </root>)
            )

        Dim r = New Speicher(dateisystem)
        ' Act / Assert
        Assert.That(AddressOf r.LeseSpieler, Throws.InstanceOf(Of UngültigeSpielerXmlException))
    End Sub

    <Test>
    Public Sub LeseSpieler_liest_sehr_große_Lizenznummern_richtig()
        ' Arrange
        Dim _BKlasse = <competition age-group="B-Klasse" xmlns:ppc="http://www.ttc-langensteinbach.de">
                           <players>
                               <player id="ID123">
                                   <person
                                       licence-nr="1234567890123456789"
                                       club-name="TS Durlach"
                                       sex="0"
                                       ttr-match-count="55"
                                       ttr="12"
                                       firstname="Max"
                                       lastname="Mustermann"
                                       birthyear="1981"
                                       >
                                   </person>
                               </player>
                           </players>
                       </competition>
        Dim dateisystem = Mock.Of(Of IDateisystem)(
                Function(m) m.LadeXml() Is New XDocument(<root>
                                                             <%= _BKlasse %>
                                                         </root>)
            )

        Dim r = New Speicher(dateisystem)
        ' Act / Assert
        Dim s = r.LeseSpieler.First
        Assert.That(s.LizenzNr, [Is].EqualTo(1234567890123456789L))
    End Sub
End Class
