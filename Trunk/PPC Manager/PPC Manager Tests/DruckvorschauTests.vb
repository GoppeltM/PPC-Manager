Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Windows.Shapes

Public Class DruckvorschauTests

    <Test, STAThread>
    Public Sub Konstruktor_erlaubt_leere_Menge()
        Dim vorschau = New Druckvorschau(New List(Of FixedPage))
        Assert.That(vorschau.Inhalt.Items.Count, [Is].EqualTo(0))
    End Sub

    <Test, STAThread>
    Public Sub Inhalt_enthält_so_viele_Elemente_wie_initialisiert()
        Dim l = New List(Of FixedPage)
        l.Add(New FixedPage)
        Dim vorschau = New Druckvorschau(l)
        Assert.That(vorschau.Inhalt.Items.Count, [Is].EqualTo(2))
    End Sub

    <Test, STAThread, Explicit>
    Public Sub Layout_Test()
        Dim l = New List(Of FixedPage)
        Dim pageA = New FixedPage
        pageA.Height = 500
        pageA.Width = 300
        pageA.Children.Add(New Rectangle With {.Height = 100, .Width = 50, .Stroke = Brushes.Red})
        Dim pageB = New FixedPage
        pageB.Children.Add(New Rectangle With {.Height = 200, .Width = 250, .Stroke = Brushes.Blue})
        pageB.Height = 500
        pageB.Width = 300
        l.Add(pageA)
        l.Add(pageB)
        Dim vorschau = New Druckvorschau(l)
        vorschau.ShowDialog()
    End Sub

    <Test, STAThread, Explicit>
    Public Sub Druck_Test()


        Dim document As New FixedDocument
        Dim ellipse = New Ellipse With {.Stroke = Brushes.Black, .Fill = Brushes.Red, .Height = 1300, .Width = 300}
        Dim b As New VisualBrush(ellipse) With {.Stretch = Stretch.None, .AlignmentX = AlignmentX.Left, .AlignmentY = AlignmentY.Top}

        Dim pageSize = New Size(300, 500)
        Dim pageCount = CInt(ellipse.Height) \ CInt(pageSize.Height)
        If CInt(ellipse.Height) Mod CInt(pageSize.Height) <> 0 Then
            pageCount += 1
        End If
        For Each x In Enumerable.Range(0, pageCount)
            b.Transform = New TranslateTransform(0, pageSize.Height * x * -1)
            Dim page = New FixedPage With {.Height = pageSize.Height, .Width = pageSize.Width}
            Dim canvas As New Canvas With {.Height = ellipse.Height, .Width = ellipse.Width, .Background = b.Clone}
            page.Children.Add(canvas)
            document.Pages.Add(New PageContent() With {.Child = page})
        Next

        Dim docViewer = New DocumentViewer()
        docViewer.Document = document
        Dim w As New Window With {.Content = docViewer}
        w.ShowDialog()
    End Sub
End Class
