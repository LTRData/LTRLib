'
' LTRLib
' 
' Copyright (c) Olof Lagerkvist, LTR Data
' http://ltr-data.se   https://github.com/LTRData
'

Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms

Namespace LTRGeneric

    <ComVisible(False)>
    Public MustInherit Class MessageBoxSupport

#If Not NET35_OR_GREATER Then

        Public Delegate Function Func(Of T, TResult)(param As T) As TResult

#End If

        Private Sub New()

        End Sub

        <Obsolete("Use modern task based routines instead")>
        Public Shared Function DoWorkWithProgressMessage(Of TResult)(owner As IWin32Window,
                                                                     initialMessage As String,
                                                                     work As Func(Of AsyncMessageBox, TResult)) As TResult

            Using AsyncMessageBox As New AsyncMessageBox

                AsyncMessageBox.MsgText = initialMessage

                Dim result As TResult
                Dim ex As Exception = Nothing
                Dim taskfinished As Boolean

                ThreadPool.QueueUserWorkItem(
                    Sub()
                        While Not AsyncMessageBox.IsHandleCreated
                            Thread.Sleep(20)
                        End While

                        Try
                            result = work(AsyncMessageBox)

                        Catch exi As Exception
                            ex = New TargetInvocationException(exi)

                        End Try

                        taskfinished = True
                        AsyncMessageBox.Invoke(Sub() AsyncMessageBox.Close())
                    End Sub)

                AddHandler AsyncMessageBox.FormClosing,
                    Sub(sender, e)
                        If taskfinished = False Then
                            e.Cancel = True
                        End If
                    End Sub

                AsyncMessageBox.ShowDialog(owner)

                If ex Is Nothing Then
                    Return result
                Else
                    Throw ex
                End If

            End Using

        End Function


    End Class

End Namespace
