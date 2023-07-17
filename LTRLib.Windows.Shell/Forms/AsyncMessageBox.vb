'
' LTRLib
' 
' Copyright (c) Olof Lagerkvist, LTR Data
' http://ltr-data.se   https://github.com/LTRData
'

Imports System.Drawing
Imports System.Windows.Forms
Imports LTRLib.LTRGeneric

Public Class AsyncMessageBox

    Private Shared ReadOnly MaxFont As New Font("Tahoma", 96)

    Private m_ForegroundBrush As Brush
    Private m_BackgroundBrush As Brush
    Private m_TextRectangle As RectangleF

    Private WriteOnly Property ForegroundBrush As Brush
        Set(value As Brush)
            m_ForegroundBrush?.Dispose()
            m_ForegroundBrush = value
        End Set
    End Property

    Private WriteOnly Property BackgroundBrush As Brush
        Set(value As Brush)
            m_BackgroundBrush?.Dispose()
            m_BackgroundBrush = value
        End Set
    End Property

    Public Sub New(message As String)
        Me.New()

        MsgText = message
        MyBase.Show()
        MyBase.Activate()

    End Sub

    Public Sub New(owner As IWin32Window, message As String)
        Me.New()

        MsgText = message
        MyBase.Show(owner)
        MyBase.Activate()

    End Sub

    Public Property MsgText() As String
        Get
            Return m_Text
        End Get
        Set(value As String)
            m_Text = value
            CurrentFont = Nothing
            OnResize(EventArgs.Empty)
        End Set
    End Property

    Private m_Text As String
    Private m_CurrentFont As Font
    Private m_Sized As Boolean

    Private WriteOnly Property CurrentFont As Font
        Set(value As Font)
            m_CurrentFont?.Dispose()
            m_CurrentFont = value
        End Set
    End Property

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        If m_Sized = False Then
            Size = New Size(Screen.FromControl(Me).Bounds.Size.Width \ 4, Screen.FromControl(Me).Bounds.Size.Height \ 8)
            Location = New Point((Screen.FromControl(Me).Bounds.Size.Width - Width) \ 2, (Screen.FromControl(Me).Bounds.Size.Height - Height) \ 2)
            m_Sized = True
        End If

        ForegroundBrush = New SolidBrush(ForeColor)
        BackgroundBrush = New SolidBrush(BackColor)

        OnResize(EventArgs.Empty)

    End Sub

    Public Overloads Sub Show()
        MyBase.Show()
        MyBase.Activate()
        MyBase.Update()
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        Try
            MyBase.OnShown(e)

            OnResize(e)

        Catch

        End Try

        Update()
    End Sub

    Private m_ImageBuffer As Image
    Private WriteOnly Property ImageBuffer As Image
        Set(value As Image)
            m_ImageBuffer?.Dispose()
            m_ImageBuffer = value
        End Set
    End Property

    Protected Overrides Sub OnResize(e As EventArgs)
        Try
            MyBase.OnResize(e)

            If m_ForegroundBrush Is Nothing OrElse m_BackgroundBrush Is Nothing Then
                Exit Try
            End If

            ImageBuffer = New Bitmap(ClientSize.Width, ClientSize.Height)

            m_TextRectangle = New Rectangle(ClientRectangle.Width \ 12, ClientRectangle.Height \ 8, ClientRectangle.Width * 10 \ 12, ClientRectangle.Height * 6 \ 8)

            Using g = Graphics.FromImage(m_ImageBuffer)

                g.PageUnit = GraphicsUnit.Pixel

                g.FillRectangle(m_BackgroundBrush, New Rectangle(Point.Empty, m_ImageBuffer.Size))

                CurrentFont = Nothing

                If String.IsNullOrEmpty(m_Text) Then
                    Return
                End If

                m_CurrentFont = FormsSupport.FindLargestFont(g, MaxFont.FontFamily, MaxFont.Size, MaxFont.Style, MaxFont.Unit, m_TextRectangle, m_Text)

                g.DrawString(m_Text, m_CurrentFont, m_ForegroundBrush, m_TextRectangle, FormsSupport.sftCentered)

            End Using

        Catch

        End Try
    End Sub

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        Try
            If m_ImageBuffer Is Nothing Then
                MyBase.OnPaintBackground(e)
                Return
            End If

            e.Graphics.DrawImage(m_ImageBuffer, Point.Empty)

        Catch

        End Try
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Try
            If m_ImageBuffer Is Nothing Then
                MyBase.OnPaint(e)
                Return
            End If

        Catch

        End Try
    End Sub

    Public Overrides Property ForeColor() As Color
        Get
            Return MyBase.ForeColor
        End Get
        Set(value As Color)
            MyBase.ForeColor = value
            ForegroundBrush = New SolidBrush(value)
        End Set
    End Property

    Public Overrides Property BackColor As Color
        Get
            Return MyBase.BackColor
        End Get
        Set(value As Color)
            MyBase.BackColor = value
            BackgroundBrush = New SolidBrush(value)
        End Set
    End Property

    Public Sub SetProgressMessage(msg As String)
        Invoke(Sub()
                   MsgText = msg
                   Refresh()
               End Sub)
    End Sub

    Private Sub ObjDisposed() Handles Me.Disposed
        ImageBuffer = Nothing
        CurrentFont = Nothing
        ForegroundBrush = Nothing
        BackgroundBrush = Nothing
    End Sub

End Class
