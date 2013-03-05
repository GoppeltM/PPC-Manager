Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de/">
Imports System.ComponentModel

Public Class Spieler
    Implements IComparable(Of Spieler), IEditableObject, INotifyPropertyChanged



    Public Sub New()
        XmlKnoten = New XElement() {
            <ppc:player>
                <person club-name="" sex="1" ttr-match-count="0"
                    firstname="" lastname="" ttr="0"/>
            </ppc:player>}
    End Sub

    Property Vorname As String
        Get
            Return XmlKnoten.First.@firstname
        End Get
        Set(value As String)
            For Each Spieler In XmlKnoten
                Spieler.@firstname = value
            Next
        End Set
    End Property
    Property Nachname As String
        Get
            Return XmlKnoten.First.@lastname
        End Get
        Set(value As String)
            For Each Spieler In XmlKnoten
                Spieler.@lastname = value
            Next
        End Set
    End Property
    Private _Fremd As Boolean
    ReadOnly Property Fremd As Boolean
        Get
            Return _Fremd
        End Get
    End Property

    Property Verein As String
        Get
            Return XmlKnoten.First.Attribute("club-name").Value
        End Get
        Set(value As String)
            For Each Spieler In XmlKnoten
                Spieler.Attribute("club-name").Value = value
            Next
        End Set
    End Property

    Private Property LocalNameSpace As XNamespace = XNamespace.None


    Private _XmlKnoten As IEnumerable(Of XElement)
    Property XmlKnoten As IEnumerable(Of XElement)
        Get
            Return _XmlKnoten.<person>
        End Get
        Set(value As IEnumerable(Of XElement))
            _Fremd = False
            value.All(Function(x) XElement.DeepEquals(value.First, x))
            If value.First.GetNamespaceOfPrefix("ppc") IsNot Nothing Then
                _Fremd = True
                LocalNameSpace = value.First.GetNamespaceOfPrefix("ppc")
            End If
            _XmlKnoten = value
            If Not XmlKnoten.Any Then Throw New ArgumentException("Konsistenzfehler: mindestens ein Spieler notwendig")
        End Set
    End Property
    Property Anwesend As Boolean
    Property TTR As Integer
        Get
            Return Integer.Parse(XmlKnoten.First.@ttr)
        End Get
        Set(value As Integer)
            For Each Spieler In XmlKnoten
                Spieler.@ttr = value.ToString
            Next
            OnPropertyChanged("TTR")
        End Set
    End Property

    Property TTRMatchCount As Integer
        Get
            Return Integer.Parse(XmlKnoten.First.Attribute("ttr-match-count").Value)
        End Get
        Set(value As Integer)
            For Each Spieler In XmlKnoten
                Spieler.Attribute("ttr-match-count").Value = value.ToString
            Next
        End Set
    End Property
    Property LizenzNr As Integer
        Get
            Dim Nr = -1
            If XmlKnoten.First.Attribute("licence-nr") Is Nothing Then Return Nr
            Integer.TryParse(XmlKnoten.First.Attribute("licence-nr").Value, Nr)
            Return Nr
        End Get
        Set(value As Integer)
            For Each Spieler In XmlKnoten
                Spieler.Attribute("licence-nr").Value = value.ToString
            Next
        End Set
    End Property

    Property Klassements As IEnumerable(Of String)

    Property Geschlecht As Integer
        Get
            Return Integer.Parse(XmlKnoten.First.@sex)
        End Get
        Set(value As Integer)
            For Each Spieler In XmlKnoten
                Spieler.@sex = value.ToString
            Next
        End Set
    End Property

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

    Private _OldValue As IEnumerable(Of XElement)

    Public Sub BeginEdit() Implements IEditableObject.BeginEdit
        _OldValue = (From x In _XmlKnoten Select New XElement(x)).ToList
    End Sub

    Public Sub CancelEdit() Implements IEditableObject.CancelEdit
        _XmlKnoten = _OldValue
    End Sub

    Public Sub EndEdit() Implements IEditableObject.EndEdit
        _OldValue = Nothing
    End Sub

    Private Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub


    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class

Public Class SpielerListe
    Inherits ObservableCollection(Of Spieler)

    Public Sub New()

    End Sub

End Class