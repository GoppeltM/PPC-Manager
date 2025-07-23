Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media

Public Class Selectable
    Inherits Border

    Dim backgroundBrush As SolidColorBrush = New SolidColorBrush(SystemColors.HighlightColor)
    Dim hoverBrush As SolidColorBrush = New SolidColorBrush(SystemColors.HighlightColor)

    Public Shared Sub DeselectAll()
        Dim selectables As List(Of Selectable) = VisualTreeHelperExtensions.FindVisualChildren(Of Selectable)(Application.Current.MainWindow)

        For Each selectableInstance As Selectable In selectables
            selectableInstance.IsSelected = False
        Next
    End Sub

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

    ' Routed event for selection
    Public Shared ReadOnly SelectedEvent As RoutedEvent = EventManager.RegisterRoutedEvent(
        "Selected",
        RoutingStrategy.Bubble,
        GetType(RoutedEventHandler),
        GetType(Selectable))

    ' CLR event wrapper for Selected routed event
    Public Custom Event Selected As RoutedEventHandler
        AddHandler(value As RoutedEventHandler)
            [AddHandler](SelectedEvent, value)
        End AddHandler

        RemoveHandler(value As RoutedEventHandler)
            [RemoveHandler](SelectedEvent, value)
        End RemoveHandler

        RaiseEvent(sender As Object, e As RoutedEventArgs)
            [RaiseEvent](e)
        End RaiseEvent
    End Event

    Private Shared Sub OnIsSelectedChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim border As Selectable = CType(d, Selectable)
        border.UpdateVisualState()
        If border.IsSelected Then
            border.OnSelected()
        End If
    End Sub

    Protected Overridable Sub OnSelected()
        Dim even = New RoutedEventArgs(Selectable.SelectedEvent)
        RaiseEvent Selected(Me, even)
    End Sub

    Public Sub New()
        MyBase.New()
        AddHandler Me.MouseLeftButtonDown, AddressOf UpdateSelection
        AddHandler Me.MouseEnter, AddressOf AddHoverStyle
        AddHandler Me.MouseLeave, AddressOf RemoveHoverStyle
        BorderBrush = SystemColors.HighlightBrush
        backgroundBrush.Opacity = 0.3
        hoverBrush.Opacity = 0.15
    End Sub

    Private Sub UpdateSelection(sender As Object, e As MouseButtonEventArgs)
        If Not IsEnabled Then Return

        DeselectAll()

        IsSelected = Not IsSelected
    End Sub

    Private Sub AddHoverStyle(sender As Object, e As MouseEventArgs)
        If IsSelected Then Return
        BorderThickness = New Thickness(1)
        Margin = New Thickness(-1)
        Background = hoverBrush
    End Sub

    Private Sub RemoveHoverStyle(sender As Object, e As MouseEventArgs)
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