Public Interface IStartlistenController

    Sub Initialize(spielerListe As SpielerListe, klassementListe As KlassementListe)

    Sub Schließend(dataGrid As DataGrid, e As ComponentModel.CancelEventArgs)

    Sub Öffnend()

    Sub NeuerFremdSpieler(dataGrid As DataGrid)

    Sub LöscheFremdSpieler(dataGrid As DataGrid)

    Sub EditiereFremdSpieler(dataGrid As DataGrid)

    Sub Speichern(dataGrid As DataGrid)

End Interface
