Imports System.Collections.ObjectModel
Imports <xmlns:ppc="http://www.ttc-langensteinbach.de">
Imports System.ComponentModel

Public Class Spieler
    Implements IComparable(Of Spieler), IEditableObject, INotifyPropertyChanged



    Public Sub New()
        XmlKnoten = <ppc:player>
                        <person club-name="" sex="1" ttr-match-count="0"
                            firstname="" lastname="" ttr="0" licence-nr="0"/>
                    </ppc:player>
    End Sub

    Public Sub New(knoten As XElement)
        XmlKnoten = knoten
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

    Private _XmlKnoten As XElement
    Property XmlKnoten As XElement
        Get
            Return _XmlKnoten
        End Get
        Set(value As XElement)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            If Not value.<person>.Any Then Throw New ArgumentException("Konsistenzfehler: mindestens ein Spieler notwendig")
            _Fremd = False
            If Not String.IsNullOrEmpty(value.Name.NamespaceName) Then
                _Fremd = True
                LocalNameSpace = value.GetNamespaceOfPrefix("ppc")
            End If
            _XmlKnoten = value            
        End Set
    End Property

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

    ReadOnly Property Klassement As String
        Get
            Dim KlassementName = From x In XmlKnoten.Ancestors("competition") Select x.Attribute("age-group").Value

            Return KlassementName.Single
        End Get
    End Property

    Property Geschlecht As Integer
        Get
            Return Integer.Parse(XmlPerson.@sex)
        End Get
        Set(value As Integer)
            XmlPerson.@sex = value.ToString
        End Set
    End Property

    WriteOnly Property ID As String
        Set(value As String)
            XmlKnoten.@id = value            
        End Set
    End Property

    Property Bezahlt As Boolean
        Get
            Dim result As Boolean
            Return Boolean.TryParse(XmlPerson.@ppc:bezahlt, result)
            Return result
        End Get
        Set(value As Boolean)
            XmlPerson.@ppc:bezahlt = value.ToString
        End Set
    End Property

    Property Anwesend As Boolean
        Get
            Dim result As Boolean
            Return Boolean.TryParse(XmlPerson.@ppc:anwesend, result)
            Return result
        End Get
        Set(value As Boolean)
            XmlPerson.@ppc:anwesend = value.ToString
        End Set
    End Property

    Property Abwesend As Boolean
        Get
            Dim result As Boolean
            Return Boolean.TryParse(XmlPerson.@ppc:abwesend, result)
            Return result
        End Get
        Set(value As Boolean)
            XmlPerson.@ppc:abwesend = value.ToString
        End Set
    End Property

    Shared Function FromXML(SpielerKnoten As XElement) As Spieler
        Return New Spieler(SpielerKnoten)        
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

    Private _OldValue As XElement

    Public Sub BeginEdit() Implements IEditableObject.BeginEdit
        _OldValue = XmlKnoten
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