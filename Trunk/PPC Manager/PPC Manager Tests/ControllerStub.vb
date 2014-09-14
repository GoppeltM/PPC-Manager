Public Class ControllerStub
    Implements StartlistenEditor.IStartlistenController


    Public Sub EditiereFremdSpieler(dataGrid As Windows.Controls.DataGrid) Implements StartlistenEditor.IStartlistenController.EditiereFremdSpieler

    End Sub

    Private _SpielerListe As StartlistenEditor.SpielerListe
    Private _KlassementListe As StartlistenEditor.KlassementListe

    Public Sub Initialize(spielerListe As StartlistenEditor.SpielerListe, klassementListe As StartlistenEditor.KlassementListe) Implements StartlistenEditor.IStartlistenController.Initialize
        _SpielerListe = spielerListe
        _KlassementListe = klassementListe
    End Sub

    Public Sub LöscheFremdSpieler(dataGrid As Windows.Controls.DataGrid) Implements StartlistenEditor.IStartlistenController.LöscheFremdSpieler

    End Sub

    Public Sub NeuerFremdSpieler(dataGrid As Windows.Controls.DataGrid) Implements StartlistenEditor.IStartlistenController.NeuerFremdSpieler

    End Sub

    Public Sub Öffnend() Implements StartlistenEditor.IStartlistenController.Öffnend
        Dim Doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        Dim AlleSpieler = StartlistenEditor.StartlistenController.XmlZuSpielerListe(Doc)

        Dim AlleKlassements = (From x In Doc.Root.<competition> Select x.Attribute("age-group").Value).Distinct
        With _KlassementListe
            For Each Klassement In AlleKlassements
                .Add(New StartlistenEditor.KlassementName With {.Name = Klassement})
            Next
        End With

        For Each s In AlleSpieler
            _SpielerListe.Add(s)
        Next
    End Sub

    Public Sub Schließend(dataGrid As Windows.Controls.DataGrid, e As ComponentModel.CancelEventArgs) Implements StartlistenEditor.IStartlistenController.Schließend

    End Sub

    Public Sub Speichern(dataGrid As Windows.Controls.DataGrid) Implements StartlistenEditor.IStartlistenController.Speichern

    End Sub
End Class
