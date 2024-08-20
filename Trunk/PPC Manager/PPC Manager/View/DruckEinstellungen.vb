Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Printing

Public Class DruckEinstellungen
    Implements INotifyPropertyChanged

    Public Sub New()
        RestorePrintDialogSettings(1)
        RestorePrintDialogSettings(2)
        RestorePrintDialogSettings(3)
        RestorePrintDialogSettings(4)
    End Sub

    Public Property DruckeNeuePaarungen As Boolean
    Public Property DruckeSchiedsrichterzettel As Boolean
    Public Property DruckeSpielergebnisse As Boolean
    Public Property DruckeRangliste As Boolean

    Public Property EinstellungenNeuePaarungen As PrintDialog
        Get
            Return RestorePrintDialogSettings(1)
        End Get
        Set(value As PrintDialog)
            SavePrintDialogSettings(value, 1)
            OnPropertyChanged()
        End Set
    End Property

    Public Property EinstellungenSchiedsrichterzettel As PrintDialog
        Get
            Return RestorePrintDialogSettings(2)
        End Get
        Set(value As PrintDialog)
            SavePrintDialogSettings(value, 2)
            OnPropertyChanged()
        End Set
    End Property

    Public Property EinstellungenSpielergebnisse As PrintDialog
        Get
            Return RestorePrintDialogSettings(3)
        End Get
        Set(value As PrintDialog)
            SavePrintDialogSettings(value, 3)
            OnPropertyChanged()
        End Set
    End Property

    Public Property EinstellungenRangliste As PrintDialog
        Get
            Return RestorePrintDialogSettings(4)
        End Get
        Set(value As PrintDialog)
            SavePrintDialogSettings(value, 4)
            OnPropertyChanged()
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub OnPropertyChanged(<CallerMemberName()> Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private Sub SavePrintDialogSettings(printDialog As PrintDialog, index As Integer)
        Select Case index
            Case 1
                My.Settings.PrinterName1 = If(printDialog.PrintQueue IsNot Nothing, printDialog.PrintQueue.Name, String.Empty)
                My.Settings.CopyCount1 = CInt(If(printDialog.PrintTicket IsNot Nothing AndAlso printDialog.PrintTicket.CopyCount IsNot Nothing, printDialog.PrintTicket.CopyCount, 1))
            Case 2
                My.Settings.PrinterName2 = If(printDialog.PrintQueue IsNot Nothing, printDialog.PrintQueue.Name, String.Empty)
                My.Settings.CopyCount2 = CInt(If(printDialog.PrintTicket IsNot Nothing AndAlso printDialog.PrintTicket.CopyCount IsNot Nothing, printDialog.PrintTicket.CopyCount, 1))
            Case 3
                My.Settings.PrinterName3 = If(printDialog.PrintQueue IsNot Nothing, printDialog.PrintQueue.Name, String.Empty)
                My.Settings.CopyCount3 = CInt(If(printDialog.PrintTicket IsNot Nothing AndAlso printDialog.PrintTicket.CopyCount IsNot Nothing, printDialog.PrintTicket.CopyCount, 1))
            Case 4
                My.Settings.PrinterName4 = If(printDialog.PrintQueue IsNot Nothing, printDialog.PrintQueue.Name, String.Empty)
                My.Settings.CopyCount4 = CInt(If(printDialog.PrintTicket IsNot Nothing AndAlso printDialog.PrintTicket.CopyCount IsNot Nothing, printDialog.PrintTicket.CopyCount, 1))
        End Select

        ' Save the settings
        My.Settings.Save()
    End Sub

    Private Function RestorePrintDialogSettings(index As Integer) As PrintDialog
        Dim printDialog As New PrintDialog()

        Select Case index
            Case 1
                If Not String.IsNullOrEmpty(My.Settings.PrinterName1) Then
                    printDialog.PrintQueue = New PrintQueue(New PrintServer(), My.Settings.PrinterName1)
                    printDialog.PrintTicket.CopyCount = My.Settings.CopyCount1
                    ' Set other properties like PaperSize if needed
                End If
            Case 2
                If Not String.IsNullOrEmpty(My.Settings.PrinterName2) Then
                    printDialog.PrintQueue = New PrintQueue(New PrintServer(), My.Settings.PrinterName2)
                    printDialog.PrintTicket.CopyCount = My.Settings.CopyCount2
                    ' Set other properties like PaperSize if needed
                End If
            Case 3
                If Not String.IsNullOrEmpty(My.Settings.PrinterName3) Then
                    printDialog.PrintQueue = New PrintQueue(New PrintServer(), My.Settings.PrinterName3)
                    printDialog.PrintTicket.CopyCount = My.Settings.CopyCount3
                    ' Set other properties like PaperSize if needed
                End If
            Case 4
                If Not String.IsNullOrEmpty(My.Settings.PrinterName4) Then
                    printDialog.PrintQueue = New PrintQueue(New PrintServer(), My.Settings.PrinterName4)
                    printDialog.PrintTicket.CopyCount = My.Settings.CopyCount4
                    ' Set other properties like PaperSize if needed
                End If
        End Select

        Return printDialog
    End Function
End Class

