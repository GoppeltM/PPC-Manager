Public Interface IStartlistenController

    Sub Initialize(spielerListe As SpielerListe, klassementListe As KlassementListe)

    Sub Öffnend(doc As XDocument, pfad As String)

    Sub NeuerFremdSpieler(neuerTTR As Integer)

    Sub LöscheFremdSpieler(spieler As Spieler)

    Sub EditiereFremdSpieler(aktuellerSpieler As Spieler)

    Sub Speichern()

End Interface
