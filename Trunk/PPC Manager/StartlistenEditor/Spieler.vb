Imports System.Collections.ObjectModel

Public Class Spieler
    Implements IComparable(Of Spieler)

    ReadOnly Property Vorname As String
        Get
            Return XmlKnoten.First.@lastname
        End Get
    End Property
    ReadOnly Property Nachname As String
        Get
            Return XmlKnoten.First.@firstname
        End Get
    End Property
    Property Fremd As Boolean

    Private _XmlKnoten As IEnumerable(Of XElement)
    Property XmlKnoten As IEnumerable(Of XElement)
        Get
            Return _XmlKnoten
        End Get
        Set(value As IEnumerable(Of XElement))
            If Not value.Any Then Throw New ArgumentException("Konsistenzfehler: mindestens ein Spieler notwendig")
            value.All(Function(x) XElement.DeepEquals(value.First, x))
            _XmlKnoten = value
        End Set
    End Property
    Property Anwesend As Boolean
    ReadOnly Property TTR As Integer
        Get
            Return Integer.Parse(XmlKnoten.First.@ttr)
        End Get
    End Property

    ReadOnly Property TTRMatchCount As Integer
        Get
            Return Integer.Parse(XmlKnoten.First.Attribute("ttr-match-count").Value)
        End Get
    End Property
    ReadOnly Property LizenzNr As Integer
        Get
            Return Integer.Parse(XmlKnoten.First.Attribute("licence-nr").Value)
        End Get
    End Property

    Property Klassements As IEnumerable(Of String)

    Shared Function FromXML(SpielerKnoten As IEnumerable(Of XElement), Competitions As IEnumerable(Of String)) As Spieler
        Return New Spieler With {
        .XmlKnoten = SpielerKnoten,
        .Klassements = Competitions
        }
    End Function

    Public Function CompareTo(other As Spieler) As Integer Implements IComparable(Of Spieler).CompareTo
        Dim diff = Me.TTR - other.TTR
        If diff <> 0 Then Return diff
        diff = Me.TTRMatchCount - other.TTRMatchCount
        If diff <> 0 Then Return diff
        diff = Me.Nachname.CompareTo(other.Nachname)
        If diff <> 0 Then Return diff
        diff = Me.Vorname.CompareTo(other.Vorname)
        If diff <> 0 Then Return diff
        Return Me.LizenzNr - other.LizenzNr
    End Function
End Class

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

    Public Sub New()
        
    End Sub

End Class