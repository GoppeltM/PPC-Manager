Imports System.Collections.ObjectModel


Public Class SpielRunden
    Inherits Stack(Of SpielRunde)

    <DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>
    Public Property AusgeschiedeneSpieler As New ObservableCollection(Of Ausgeschieden(Of Spieler))

End Class



