Imports System.Collections.ObjectModel

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

End Class

Public Class SpielPartien
    Inherits ObservableCollection(Of SpielPartie)

    Public Sub New()
        ' TODO: für Testzwecke
        Dim x As New SpielPartie        
        Add(x)
    End Sub

End Class