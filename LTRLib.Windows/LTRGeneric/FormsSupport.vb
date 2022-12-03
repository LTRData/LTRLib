'
' LTRLib
' 
' Copyright (c) Olof Lagerkvist, LTR Data
' http://ltr-data.se   https://github.com/LTRData
'

Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Permissions
Imports System.Text
Imports System.Windows.Forms
Imports LTRLib.IO

#Disable Warning IDE0079 ' Remove unnecessary suppression
#Disable Warning SYSLIB0003 ' Type or member is obsolete

Namespace LTRGeneric

    Public MustInherit Class FormsSupport

        Private Sub New()

        End Sub

        Private Const SPI_GETFOREGROUNDLOCKTIMEOUT As UInteger = &H2000
        Private Const SPI_SETFOREGROUNDLOCKTIMEOUT As UInteger = &H2001
        Private Const SPIF_SENDWININICHANGE As UInteger = &H2
        Private Const SPIF_UPDATEINIFILE As UInteger = &H1

        ''' <summary>
        ''' Returns Form object that is currently active, if that Form is created by current thread. If no Form
        ''' is active or if currently active Form belongs to another thread, Nothing is returned.
        ''' </summary>
        Public Shared Function GetCurrentThreadActiveForm() As Form
            GetCurrentThreadActiveForm = Form.ActiveForm
            If _
              GetCurrentThreadActiveForm IsNot Nothing AndAlso
              GetCurrentThreadActiveForm.InvokeRequired = True Then

                GetCurrentThreadActiveForm = Nothing
            End If
        End Function

        <SecuritySafeCritical>
        <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
        Public Shared Function GetForegroundLockTimeout(ByRef ms As UInteger) As Integer
            If Win32API.GetSystemParametersDWORD(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, ms, 0) <> 0 Then
                Return 0
            Else
                Return Marshal.GetLastWin32Error()
            End If
        End Function

        <SecuritySafeCritical>
        <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
        Public Shared Function SetForegroundLockTimeout(ms As UInteger) As Integer
            If Win32API.SetSystemParametersDWORD(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, CType(ms, IntPtr), SPIF_SENDWININICHANGE Or SPIF_UPDATEINIFILE) <> 0 Then
                Return 0
            Else
                Return Marshal.GetLastWin32Error()
            End If
        End Function

        Public Shared Function FindLargestFont(Graphics As Graphics,
                                               FontFamily As FontFamily,
                                               MaxFontSize As Single,
                                               FontStyle As FontStyle,
                                               FontUnit As GraphicsUnit,
                                               TextRectangle As RectangleF,
                                               Text As String) As Font

            FindLargestFont = Nothing

            For FontSize = MaxFontSize To 1 Step -2
                FindLargestFont = New Font(FontFamily, FontSize, FontStyle, FontUnit)

                Dim RequiredRectSize = Graphics.MeasureString(Text, FindLargestFont, CInt(TextRectangle.Width))

                If RequiredRectSize.Height < TextRectangle.Height Then
                    Exit For
                End If

                FindLargestFont.Dispose()
            Next

        End Function

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

        '' Text alignment formats
        Public Shared ReadOnly sftRightAligned As New StringFormat With {.LineAlignment = StringAlignment.Center, .Alignment = StringAlignment.Far}
        Public Shared ReadOnly sftLeftAligned As New StringFormat With {.LineAlignment = StringAlignment.Center, .Alignment = StringAlignment.Near}
        Public Shared ReadOnly sftTopAligned As New StringFormat With {.LineAlignment = StringAlignment.Near, .Alignment = StringAlignment.Center}
        Public Shared ReadOnly sftBottomAligned As New StringFormat With {.LineAlignment = StringAlignment.Far, .Alignment = StringAlignment.Center}
        Public Shared ReadOnly sftCentered As New StringFormat With {.LineAlignment = StringAlignment.Center, .Alignment = StringAlignment.Center}

    End Class

End Namespace
