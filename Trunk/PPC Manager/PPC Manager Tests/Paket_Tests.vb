Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class Paket_Tests

    <TestMethod()> Public Sub Runde_1()
        Dim doc = XDocument.Parse(My.Resources.Turnierteilnehmer)
        Dim SpielerListe = From x In doc.Root.<competition> Where x.Attribute("age-group").Value = "Herren C"
                        From y In x...<player> Select TestSpieler.FromXML(y)

        Dim SpielPartien = PaketBildung.organisierePakete(SpielerListe.ToList, 1)
        Assert.IsTrue(SpielPartien.Any)
    End Sub

End Class

Public Class TestSpieler
    Inherits Spieler

    Protected Overrides ReadOnly Property SpielRunden As SpielRunden
        Get
            Return New SpielRunden
        End Get
    End Property

    Public Overloads Shared Function FromXML(ByVal spielerNode As XElement) As Spieler
        Dim spieler As New TestSpieler()
        With spieler
            .Id = spielerNode.@id
            spielerNode = spielerNode.<person>.First
            .Vorname = spielerNode.@firstname
            .Nachname = spielerNode.@lastname
            .TTRMatchCount = CInt(spielerNode.Attribute("ttr-match-count").Value)
            .Geschlecht = CInt(spielerNode.@sex)
            .Vereinsname = spielerNode.Attribute("club-name").Value
            .Lizenznummer = CInt(spielerNode.Attribute("licence-nr").Value)
            .TTRating = CInt(spielerNode.@ttr)
        End With
        Return spieler
    End Function
End Class