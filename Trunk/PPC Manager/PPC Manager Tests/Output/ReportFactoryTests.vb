Public Class ReportFactoryTests

    <Test>
    Public Sub SchreibeReport_schreibt_Rangliste_umgedreht_und_jede_Runde_und_disposed()
        Dim s = {New SpielerInfo("A"), New SpielerInfo("B")}
        Dim partie1 = New SpielPartie("Runde 1", New SpielerInfo("B"), New SpielerInfo("C"))
        Dim partie2 = New SpielPartie("Runde 2", New SpielerInfo("D"), New SpielerInfo("E"))
        Dim runden = {
            New SpielRunde() From {
                partie2
            },
            New SpielRunde() From {
                partie1
            }
        }

        Dim dokument = New Mock(Of ITurnierReport)
        Dim action = Function(x As String) As ITurnierReport
                         Assert.That(x, [Is].EqualTo("C:\HelloWorld.xlsx"))
                         Return dokument.Object
                     End Function
        Dim vergleicher = Mock.Of(Of IComparer(Of SpielerInfo))
        Dim r = New ReportFactory("D:\temp\HelloWorld.xml", "Jugend A", s, vergleicher, runden, action)
        r.SchreibeReport("C:\HelloWorld.xlsx")
        dokument.Verify(Sub(m) m.SchreibeRangliste(It.Is(Function(x As IEnumerable(Of SpielerInfo)) _
                                                             s.Reverse.SequenceEqual(x)), 2), Times.Once)
        dokument.Verify(Sub(m) m.SchreibeNeuePartien({partie1}, 1),
                        Times.Once)
        dokument.Verify(Sub(m) m.SchreibeNeuePartien({partie2}, 2),
                        Times.Once)

        dokument.Verify(Sub(m) m.Dispose(), Times.Once)
    End Sub

    <Test>
    Public Sub ExcelPfad_leitet_sich_von_XML_Pfad_ab()
        Dim s = New SpielerInfo() {}
        Dim runden = New SpielRunde() {}
        Dim r = New ReportFactory("D:\temp\HelloWorld.xml", "Jugend A", s,
                                  Mock.Of(Of IComparer(Of SpielerInfo)),
                                  runden, Function(x) Nothing)
        Assert.That(r.ExcelPfad, Iz.EqualTo("D:\temp\Protokolle\HelloWorld_Jugend_A.xlsx"))
    End Sub
End Class
