Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media

Public Class Selectable
    Inherits Border

    Dim backgroundBrush As SolidColorBrush = New SolidColorBrush(SystemColors.HighlightColor)
    Dim hoverBrush As SolidColorBrush = New SolidColorBrush(SystemColors.HighlightColor)

    ' Dependency property to track selection state
    Public Shared ReadOnly IsSelectedProperty As DependencyProperty = DependencyProperty.Register(
        "IsSelected",
        GetType(Boolean),
        GetType(Selectable),
        New PropertyMetadata(False, AddressOf OnIsSelectedChanged))

    Public Property IsSelected As Boolean
        Get
            Return CType(GetValue(IsSelectedProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsSelectedProperty, value)
        End Set
    End Property

    Private Shared Sub OnIsSelectedChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim border As Selectable = CType(d, Selectable)
        border.UpdateVisualState()
    End Sub

    Public Sub New()
        MyBase.New()
        AddHandler Me.MouseLeftButtonDown, AddressOf OnMouseLeftButtonDown
        AddHandler Me.MouseEnter, AddressOf OnMouseEnter
        AddHandler Me.MouseLeave, AddressOf OnMouseLeave
        BorderBrush = SystemColors.HighlightBrush
        backgroundBrush.Opacity = 0.3
        hoverBrush.Opacity = 0.15
    End Sub

    Private Sub OnMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        If Not IsEnabled Then Return

        'find all instances of Selectable in the parent container and set IsSelected to false
        Dim selectables As List(Of Selectable) = VisualTreeHelperExtensions.FindVisualChildren(Of Selectable)(Application.Current.MainWindow)

        For Each selectableInstance As Selectable In selectables
            selectableInstance.IsSelected = False
        Next

        IsSelected = Not IsSelected
    End Sub

    Private Sub OnMouseEnter(sender As Object, e As MouseEventArgs)
        If IsSelected Then Return
        BorderThickness = New Thickness(1)
        Margin = New Thickness(-1)
        Background = hoverBrush
    End Sub

    Private Sub OnMouseLeave(sender As Object, e As MouseEventArgs)
        If IsSelected Or Not IsEnabled Then Return
        Background = Brushes.Transparent
        BorderThickness = New Thickness(0)
        Margin = New Thickness(0)
    End Sub

    Private Sub UpdateVisualState()
        If IsSelected Then
            Background = backgroundBrush
            BorderThickness = New Thickness(1)
            Margin = New Thickness(-1)
        Else
            Background = Brushes.Transparent
            BorderThickness = New Thickness(0)
            Margin = New Thickness(0)
        End If
    End Sub
End Class

Public Module VisualTreeHelperExtensions
    Public Function FindVisualChildren(Of T As DependencyObject)(parent As DependencyObject) As List(Of T)
        Dim foundChildren As New List(Of T)()

        If parent Is Nothing Then
            Return foundChildren
        End If

        For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(parent) - 1
            Dim child As DependencyObject = VisualTreeHelper.GetChild(parent, i)
            If child IsNot Nothing AndAlso TypeOf child Is T Then
                foundChildren.Add(DirectCast(child, T))
            End If

            foundChildren.AddRange(FindVisualChildren(Of T)(child))
        Next

        Return foundChildren
    End Function
End Module