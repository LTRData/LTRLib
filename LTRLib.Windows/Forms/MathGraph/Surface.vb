Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports LTRLib.MathExpression

#If NET35_OR_GREATER OrElse NETSTANDARD OrElse NETCOREAPP Then

Namespace Forms.MathGraph

    Public Class Surface
        Implements IDisposable

        ' Paths for the current graphs to be drawn when surface picturebox is painted
        Private ReadOnly GraphPath As New GraphicsPath
        Private ReadOnly DerivPath As New GraphicsPath
        Private ReadOnly IntegPath As New GraphicsPath

        ' Cache these values in variable for performance reasons
        Public Xmin As Double
        Public Xmax As Double
        Public Ymin As Double
        Public Ymax As Double

        Public Area As New Rectangle

        Public Sub Clear()
            GraphPath.Reset()
            DerivPath.Reset()
            IntegPath.Reset()
        End Sub

        Public Sub DrawAxis(Graphics As Graphics, Pen As Pen)
            With Graphics
                .DrawLine(Pen, VirtualToScreenX(Xmin), VirtualToScreenY(0), VirtualToScreenX(Xmax), VirtualToScreenY(0))
                .DrawLine(Pen, VirtualToScreenX(0), VirtualToScreenY(Ymin), VirtualToScreenX(0), VirtualToScreenY(Ymax))
            End With
        End Sub

        Public Sub DrawGraph(Graphics As Graphics, Pen As Pen)
            Graphics.DrawPath(Pen, GraphPath)
        End Sub

        Public Sub DrawDerivative(Graphics As Graphics, Pen As Pen)
            Graphics.DrawPath(Pen, DerivPath)
        End Sub

        Public Sub DrawIntegral(Graphics As Graphics, Pen As Pen)
            Graphics.DrawPath(Pen, IntegPath)
        End Sub

        Public Sub Refresh(ScriptControl As ScriptControl)
            ' The (to-be-calculated) Y values as init as Nothing. Later in code that is used
            ' as the value for "undefined Y".
            Dim X As Double?
            Dim Y As Double?
            Dim oldX As Double?
            Dim oldY As Double?

            Dim old_dYdX As Double?
            Dim old_FdX As Double? = 0

            For Tracker = Area.Left To Area.Right
                X = ScreenToVirtualX(Tracker)
                Try
                    Y = ScriptControl.Eval(X.Value, If(Y, 0))

                Catch
                    ' Set Y to 0 in case no Y is defined for this X value
                    Y = Nothing

                End Try

                ' Only go into plotting in case both this Y and the last one was defined.
                ' In that case draw a line from the last point.
                If Y.HasValue AndAlso oldY.HasValue Then
                    ' No line drawing in case curve passed +/- infinity (e.g. y=1/x)
                    If (oldY > Ymin AndAlso oldY < Ymax) OrElse (Y > Ymin AndAlso Y < Ymax) Then
                        GraphPath.StartFigure()
                        Try
                            GraphPath.AddLine(Tracker - 1, VirtualToScreenY(oldY.Value), Tracker, VirtualToScreenY(Y.Value))
                        Catch
                        End Try
                        GraphPath.CloseFigure()
                    End If

                    ' Calcualte derivate by it's definition
                    Dim dYdX = (Y - oldY) / (X - oldX)

                    ' Only plot derivate curve if last derivate point was defined
                    If old_dYdX.HasValue Then
                        ' Same here, don't draw "through infinity"
                        If (old_dYdX > Ymin AndAlso old_dYdX < Ymax) OrElse (dYdX > Ymin AndAlso dYdX < Ymax) Then
                            DerivPath.StartFigure()
                            Try
                                DerivPath.AddLine(Tracker - 1, VirtualToScreenY(old_dYdX.Value), Tracker, VirtualToScreenY(dYdX.Value))
                            Catch
                            End Try
                            DerivPath.CloseFigure()
                        End If
                    End If

                    ' Calculate antiderivative
                    Dim FdX = old_FdX + Y * (X - oldX)
                    If FdX < Ymin Then
                        FdX = Ymax
                    ElseIf FdX > Ymax Then
                        FdX = Ymin
                    ElseIf (old_FdX > Ymin AndAlso old_FdX < Ymax) OrElse (FdX > Ymin AndAlso FdX < Ymax) Then
                        IntegPath.StartFigure()
                        Try
                            IntegPath.AddLine(Tracker - 1, VirtualToScreenY(old_FdX.Value), Tracker, VirtualToScreenY(FdX.Value))
                        Catch
                        End Try
                        IntegPath.CloseFigure()
                    End If

                    old_dYdX = dYdX
                    old_FdX = FdX
                Else
                    old_FdX = 0
                End If

                oldX = X
                oldY = Y
            Next
        End Sub

        Public Function ScreenToVirtualX(X As Integer) As Double
            Return (X - Area.Left) * (Xmax - Xmin) / Area.Width + Xmin
        End Function

        Public Function ScreenToVirtualY(Y As Integer) As Double
            Return (Area.Height - Y + Area.Top) * (Ymax - Ymin) / Area.Height + Ymin
        End Function

        Public Function VirtualToScreenX(X As Double) As Integer
            Dim ScreenCoord = CInt((X - Xmin) * Area.Width / (Xmax - Xmin))
            Select Case ScreenCoord
                Case Is < 0
                    Return Area.Left - 1
                Case Is > Area.Width
                    Return Area.Right + 1
                Case Else
                    Return Area.Left + ScreenCoord
            End Select
        End Function

        Public Function VirtualToScreenY(Y As Double) As Integer
            Dim ScreenCoord = CInt(Area.Height - (Y - Ymin) * Area.Height / (Ymax - Ymin))
            Select Case ScreenCoord
                Case Is < 0
                    Return Area.Top - 1
                Case Is > Area.Height
                    Return Area.Bottom + 1
                Case Else
                    Return Area.Top + ScreenCoord
            End Select
        End Function
#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    GraphPath?.Dispose()
                    DerivPath?.Dispose()
                    IntegPath?.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.

                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        '' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        Protected Overrides Sub Finalize()
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(False)
            MyBase.Finalize()
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class

End Namespace

#End If
