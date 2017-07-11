Imports System.IO
Imports System.Xml.Schema

<TestFixture()> Public Class XMLValidation

    Private Function GetSchemaSet() As XmlSchemaSet
        Dim schema As XmlSchema
        Using stream = New StringReader(My.Resources.SpeicherStandSchema)
            schema = XmlSchema.Read(stream, Nothing)
        End Using

        Dim schemaSet As New XmlSchemaSet
        schemaSet.Add(schema)

        schemaSet.Compile()
        Return schemaSet
    End Function


    <Test>
    Sub Competition_Validieren()
        Dim doc = XDocument.Parse(My.Resources.Competition)
        doc.Validate(GetSchemaSet, Nothing)
    End Sub

    <Test>
    Sub PPC_15_Validieren()
        Dim doc = XDocument.Parse(My.Resources.PPC_15_Anmeldungen)
        doc.Validate(GetSchemaSet, Nothing)
    End Sub

    <Test>
    Sub Testturnier_Validieren()
        Dim doc = XDocument.Parse(My.Resources.Testturnier)
        doc.Validate(GetSchemaSet, Nothing)
    End Sub

    <Test> Public Sub TTRGültig()
        Dim doc = XDocument.Parse(My.Resources.Turnierteilnehmer)
        For Each person In doc...<person>
            Assert.IsNotNull(person.@ttr)
        Next
    End Sub

    <Explicit("Manuell")>
    <Test>
    Public Sub BereinigeNamespaces()
        Dim doc = XDocument.Load("D:\Eigene Dateien - Marius\Desktop\Turnierteilnehmer_mu13_2013_test.xml")
        Dim NodesToRemove = From x In doc.Root.Descendants Where x.Name.NamespaceName = "http://www.ttc-langensteinbach.de"

        Dim AttributesToRemove = From x In doc.Root.Descendants
                                 From y In x.Attributes
                                 Where y.Name.NamespaceName = "http://www.ttc-langensteinbach.de" Or
                                 y.Value = "http://www.ttc-langensteinbach.de" Select y

        For Each attr In AttributesToRemove.ToList
            attr.Remove()
        Next

        For Each node In NodesToRemove.ToList
            node.Remove()
        Next

    End Sub

    <Explicit("Manuell")>
    <Test>
    Public Sub FügeSchemaHinzu()
        Dim doc = XDocument.Load("D:\Eigene Dateien - Marius\Desktop\Turnierteilnehmer_mu13_2013_test.xml")
        Dim schema As XmlSchema
        Using stream = New IO.FileStream("E:\Skydrive\Dokumente\Repositories\Programme\Trunk\PPC Manager\PPC Manager\SpeicherStandSchema.xsd", IO.FileMode.Open)
            schema = XmlSchema.Read(stream, Nothing)
        End Using

        Dim schemaSet As New XmlSchemaSet
        schemaSet.Add(schema)

        schemaSet.Compile()

        doc.Validate(schemaSet, Nothing)

        Using s As New IO.MemoryStream
            schema.Write(s)
            s.Position = 0
            Dim schemaDoc = XDocument.Load(s) '"D:\Blubb.xml")
            doc.Root.Add(schemaDoc.Root)
        End Using

        doc.Save("D:\Eigene Dateien - Marius\Desktop\Turnierteilnehmer_mu13_2013_test_Schema.xml")
    End Sub


End Class