'
' LTRLib
' 
' Copyright (c) Olof Lagerkvist, LTR Data
' http://ltr-data.se   https://github.com/LTRData
'

#Disable Warning IDE0079 ' Remove unnecessary suppression
#Disable Warning SYSLIB0003 ' Type or member is obsolete

Imports System.Security
Imports System.Security.Permissions

<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.AllFlags)>
<SecurityCritical>
Public Class PropertyDialog

    Public Property SelectedObject As Object
        <SecuritySafeCritical>
        Get
            Return PropertyGrid.SelectedObject
        End Get
        <SecuritySafeCritical>
        Set
            PropertyGrid.SelectedObject = Value
        End Set
    End Property

    Public Property SelectedObjects As Object()
        <SecuritySafeCritical>
        Get
            Return PropertyGrid.SelectedObjects
        End Get
        <SecuritySafeCritical>
        Set
            PropertyGrid.SelectedObjects = Value
        End Set
    End Property

#Disable Warning IDE1006 ' Naming Styles

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    <SecuritySafeCritical>
    Protected Overrides Sub OnShown(e As System.EventArgs)
        MyBase.OnShown(e)

        PropertyGrid.Focus()
    End Sub

End Class
