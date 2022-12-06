'
' LTRLib
' 
' Copyright (c) Olof Lagerkvist, LTR Data
' http://ltr-data.se   https://github.com/LTRData
'

Imports System.Drawing
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Permissions
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
Imports LTRLib.IO

#Disable Warning IDE0079 ' Remove unnecessary suppression
#Disable Warning SYSLIB0003 ' Type or member is obsolete

Namespace LTRGeneric

    Public MustInherit Class ShellSupport

        Private Sub New()
        End Sub

        <SecurityCritical>
        <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.AllFlags)>
        Public Shared Sub CreateShellShortcut(ShortcutPath As String,
                                              Optional TargetPath As String = Nothing,
                                              Optional Description As String = Nothing,
                                              Optional Hotkey As String = Nothing,
                                              Optional Arguments As String = Nothing,
                                              Optional WorkingDirectory As String = Nothing,
                                              Optional RelativePath As String = Nothing,
                                              Optional IconLocation As String = Nothing,
                                              Optional FullName As String = Nothing,
                                              Optional WindowStyle As IWshRuntimeLibrary.WshWindowStyle? = Nothing)

            Dim shell As New IWshRuntimeLibrary.IWshShell_Class

            Using DisposableComWrapper.Create(shell)

                Dim shortcut = DirectCast(shell.CreateShortcut(ShortcutPath), IWshRuntimeLibrary.IWshShortcut)
                Using DisposableComWrapper.Create(shortcut)
                    With shortcut
                        If Description IsNot Nothing Then
                            .Description = Description
                        End If
                        If Arguments IsNot Nothing Then
                            .Arguments = Arguments
                        End If
                        If WindowStyle IsNot Nothing Then
                            .WindowStyle = WindowStyle.Value
                        End If
                        If WorkingDirectory IsNot Nothing Then
                            .WorkingDirectory = WorkingDirectory
                        End If
                        If IconLocation IsNot Nothing Then
                            .IconLocation = IconLocation
                        End If
                        If Hotkey IsNot Nothing Then
                            .Hotkey = Hotkey
                        End If
                        If TargetPath IsNot Nothing Then
                            .TargetPath = TargetPath
                        End If
                        .Save()
                    End With
                End Using
            End Using

        End Sub

    End Class

End Namespace
