Imports System.Windows

Public Class TestData
    Inherits List(Of String)

    Public Sub New()
        Add("Hello World")
    End Sub
End Class

Public Class Test

    Public Sub Test()
        DataContext = New List(Of String) From {"Hello World"}
    End Sub

    Private Sub CommandBinding_CanExecute(sender As Object, e As Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub CommandBinding_Executed(sender As Object, e As Windows.Input.ExecutedRoutedEventArgs)
        MessageBox.Show("Hello World")
    End Sub
End Class
