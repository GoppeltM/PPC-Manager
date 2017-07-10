Imports StartlistenEditor

Public Class SpeicherCacheTests
    <Test>
    Public Sub SpeichereAlles_ruft_Speicher_einmalig_auf()
        Dim speicher = New Mock(Of ISpeicher)
        Dim s = New SpeicherCache(speicher.Object)

        s.Speichere(Sub(x)
                    End Sub)
        s.Speichere(Sub(x)
                    End Sub)

        s.SpeichereAlles()

        speicher.Verify(Sub(m) m.Speichere(It.IsAny(Of Veränderung)), Times.Once)
    End Sub

    <Test>
    Public Sub SpeichereAlles_ruft_Speichere_Aktionen()
        Dim speicher = New Mock(Of ISpeicher)
        speicher.Setup(Sub(m) m.Speichere(It.IsAny(Of Veränderung))).Callback(Sub(m As Veränderung)
                                                                                  m.Invoke(Nothing)
                                                                              End Sub)
        Dim s = New SpeicherCache(speicher.Object)
        Dim aufgerufen = New List(Of String)
        s.Speichere(Sub(x)
                        aufgerufen.Add("C")
                    End Sub)
        s.Speichere(Sub(x)
                        aufgerufen.Add("A")
                    End Sub)
        s.Speichere(Sub(x)
                        aufgerufen.Add("B")
                    End Sub)

        s.SpeichereAlles()
        Assert.That(aufgerufen, [Is].EquivalentTo({"C", "A", "B"}))

    End Sub

    <Test>
    Public Sub SpeichereAlles_speichert_nur_Veränderungen_seit_letztem_Speichern()
        ' Arrange
        Dim speicher = New Mock(Of ISpeicher)
        Dim s = New SpeicherCache(speicher.Object)
        Dim aufgerufen = New List(Of String)
        s.Speichere(Sub(x)
                        aufgerufen.Add("A")
                    End Sub)
        s.Speichere(Sub(x)
                        aufgerufen.Add("B")
                    End Sub)
        s.SpeichereAlles()
        speicher.Setup(Sub(m) m.Speichere(It.IsAny(Of Veränderung))).Callback(Sub(m As Veränderung)
                                                                                  m.Invoke(Nothing)
                                                                              End Sub)
        ' Act
        s.Speichere(Sub(x)
                        aufgerufen.Add("C")
                    End Sub)
        s.SpeichereAlles()
        ' Assert
        Assert.That(aufgerufen, [Is].EquivalentTo({"C"}))

    End Sub

End Class
