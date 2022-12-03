'
' LTRLib
' 
' Copyright (c) Olof Lagerkvist, LTR Data
' http://ltr-data.se   https://github.com/LTRData
'

#If HAS_TASK_SCHEDULER Then

Imports TaskScheduler

Namespace IO

    <SupportedOSPlatform("windows")>
    Public MustInherit Class RunSystemTask

        Private Sub New()

        End Sub

        Public Shared Function RunSystemTask(cmdLine As String) As Integer

            Dim ExitCode As Integer
            Dim ex As Exception = Nothing

            Dim TaskName = "syssched-" & Guid.NewGuid().ToString() & "-" & cmdLine.GetHashCode()
            Trace.WriteLine("Task name = " & TaskName)

            Using ScheduledTasks As New ScheduledTasks

                Trace.WriteLine("Deleting existing task...")
                ScheduledTasks.DeleteTask(TaskName)

                Trace.WriteLine("Creating new task...")
                Using Task = ScheduledTasks.CreateTask(TaskName)
                    With Task
                        .ApplicationName = Assembly.GetExecutingAssembly().Location
                        .Flags = TaskFlags.Interactive Or TaskFlags.DeleteWhenDone
                        .MaxRunTime = TimeSpan.FromSeconds(5)
                        .MaxRunTimeLimited = True
                        .Parameters = cmdLine
                        .SetAccountInformation(String.Empty, DirectCast(Nothing, String))

                        Trace.WriteLine("Application name: '" & .ApplicationName & "'")
                        Trace.WriteLine("Parameters: '" & .Parameters & "'")

                        Trace.WriteLine("Saving task...")
                        .Save()
                        Trace.WriteLine("Starting task...")
                        .Run()

                    End With

                    Trace.WriteLine("Reopening task...")
                    Do
                        Using ReopenedTask = ScheduledTasks.OpenTask(TaskName)
                            With ReopenedTask
                                Try
                                    ExitCode = .ExitCode

                                Catch ex
                                    Trace.WriteLine(ex.Message)
                                    Exit Do

                                End Try
                                Dim time = .MostRecentRunTime
                                If time = Nothing Then
                                    Thread.Sleep(200)
                                    Continue Do
                                End If
                                Trace.WriteLine("Exit code = " & ExitCode)
                            End With
                        End Using
                        Exit Do
                    Loop

                End Using

                Trace.WriteLine("Deleting task...")
                ScheduledTasks.DeleteTask(TaskName)

            End Using

            Trace.WriteLine("Done.")

            If ex IsNot Nothing Then
                Throw ex
            End If

            Return ExitCode

        End Function

    End Class

End Namespace

#End If
