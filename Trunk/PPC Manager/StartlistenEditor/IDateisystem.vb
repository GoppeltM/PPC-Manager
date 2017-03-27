Public Interface IDateisystem
    Function LadeXml() As XDocument
    Sub SpeichereXml(doc As XDocument)
End Interface
