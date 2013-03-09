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
            Return XmlPerson.@firstname
        End Get
        Set(value As String)
            XmlPerson.@firstname = value            
        End Set
    End Property
    Property Nachname As String
        Get
            Return XmlPerson.@lastname
        End Get
        Set(value As String)
            XmlPerson.@lastname = value
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
            Return XmlPerson.Attribute("club-name").Value
        End Get
        Set(value As String)
            XmlPerson.Attribute("club-name").Value = value            
        End Set
    End Property

    Private Property LocalNameSpace As XNamespace = XNamespace.None

    Private ReadOnly Property XmlPerson As XElement
        Get
            Return _XmlKnoten.<person>.First
        End Get
    End Property

    Private _XmlKnoten As IEnumerable(Of XElement)
    Property XmlKnoten As IEnumerable(Of XElement)
        Get
            Return _XmlKnoten
        End Get
        Set(value As IEnumerable(Of XElement))
            _Fremd = False
            value.All(Function(x) XElement.DeepEquals(value.First, x))
            If Not String.IsNullOrEmpty(value.First.Name.NamespaceName) Then
                _Fremd = True
                LocalNameSpace = value.First.GetNamespaceOfPrefix("ppc")
            End If
            _XmlKnoten = value
            If Not XmlKnoten.Any Then Throw New ArgumentException("Konsistenzfehler: mindestens ein Spieler notwendig")
            If Not XmlKnoten.<person>.Any Then Throw New ArgumentException("Konsistenzfehler: mindestens ein Spieler notwendig")
        End Set
    End Property
    Property Anwesend As Boolean
    Property TTR As Integer
        Get
            Return Integer.Parse(XmlPerson.@ttr)
        End Get
        Set(value As Integer)
            XmlPerson.@ttr = value.ToString
            OnPropertyChanged("TTR")
        End Set
    End Property

    Property TTRMatchCount As Integer
        Get
            Return Integer.Parse(XmlPerson.Attribute("ttr-match-count").Value)
        End Get
        Set(value As Integer)
            XmlPerson.Attribute("ttr-match-count").Value = value.ToString            
        End Set
    End Property
    Property LizenzNr As Integer
        Get
            Return Integer.Parse(XmlPerson.Attribute("licence-nr").Value)            
        End Get
        Set(value As Integer)
            XmlPerson.Attribute("licence-nr").Value = value.ToString            
        End Set
    End Property

    Property Klassements As IEnumerable(Of String)

    Property Geschlecht As Integer
        Get
            Return Integer.Parse(XmlPerson.@sex)
        End Get
        Set(value As Integer)
            xmlperson.@sex = value.ToString            
        End Set
    End Property

    WriteOnly Property ID As String
        Set(value As String)
            For Each knoten In XmlKnoten
                XmlKnoten.@ID = value
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
        _OldValue = (From x In XmlKnoten Select New XElement(x)).ToList
    End Sub

    Public Sub CancelEdit() Implements IEditableObject.CancelEdit
        XmlKnoten = _OldValue
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