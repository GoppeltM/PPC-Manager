Public Delegate Sub Veränderung(klassements As IEnumerable(Of XElement))

Public Interface ISpeicher

    ReadOnly Property KlassementNamen As IEnumerable(Of String)

    Sub Speichere(veränderung As Veränderung)

End Interface
