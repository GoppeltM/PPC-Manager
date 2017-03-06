Imports StartlistenEditor

Public Class SpeicherTests
    <Test>
    Public Sub Sync_legt_Spieler_für_XML_Daten_an()
        ' Arrange
        Dim _BKlasse = <competition age-group="B-Klasse" xmlns:ppc="http://www.ttc-langensteinbach.de">
                           <players></players>
                       </competition>
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
        Dim r = New Speicher("D:\Dummy.txt")
        ' Act
        ' Dim l = r.LeseSpieler

        '' Assert
        'Assert.That(r.Count, [Is].EqualTo(1))
        'Dim p = r.First()
        'Assert.That(p.Fremd, Iz.EqualTo(False))
        'Assert.That(p.Geburtsjahr, Iz.EqualTo(1981))
        'Assert.That(p.Geschlecht, Iz.EqualTo(0))
        'Assert.That(p.ID, Iz.EqualTo("ID123"))
        'Assert.That(p.Klassement, Iz.EqualTo("B-Klasse"))
        'Assert.That(p.LizenzNr, Iz.EqualTo(45))
        'Assert.That(p.Nachname, Iz.EqualTo("Mustermann"))
        'Assert.That(p.TTR, Iz.EqualTo(12))
        'Assert.That(p.TTRMatchCount, Iz.EqualTo(55))
        'Assert.That(p.Verein, Iz.EqualTo("TS Durlach"))
        'Assert.That(p.Vorname, Iz.EqualTo("Max"))
    End Sub
End Class
