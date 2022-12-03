'
' LTRLib
' 
' Copyright (c) Olof Lagerkvist, LTR Data
' http://ltr-data.se   https://github.com/LTRData
'

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Security
Imports System.Text
Imports System.Windows.Forms
#If NET30_OR_GREATER OrElse NETCOREAPP Then
Imports System.Windows.Media.Media3D
#End If
Imports LTRLib.IO

Namespace Extensions

    Public Module FormsExtensions

        <Extension>
        Public Function GetTopMostOwner(form As Form) As Form

            While form?.Owner IsNot Nothing
                form = form.Owner
            End While

            Return form

        End Function

        <Extension>
        Public Function ToBase64Url(img As Image) As String

            Using buffer As New MemoryStream()
                img.Save(buffer, ImageFormat.Png)
                Return DrawingSupport.PngImageToBase64Url(buffer.ToArray())
            End Using

        End Function

#If NET30_OR_GREATER OrElse NETCOREAPP Then

        ''' <summary>
        ''' Calculates the area covered by a size.
        ''' </summary>
        <Extension>
        Public Function Volume(Size As Size3D) As Double
            Return Size.X * Size.Y * Size.Z
        End Function

        <Extension>
        Public Function Radius(Point As Point3D) As Double
            Return Math.Sqrt(Math.Pow(Math.Sqrt(Point.X * Point.X + Point.Y * Point.Y), 2) + Point.Z * Point.Z)
        End Function

        <Extension>
        Public Function DistanceTo(Point1 As Point3D, Point2 As Point3D) As Double
            Return (Point1 - New Vector3D(Point2.X, Point2.Y, Point2.Z)).Radius()
        End Function

        <Extension>
        Public Function CenterPoint(Rectangle As Rect3D) As Point3D
            Return New Point3D(Rectangle.X + Rectangle.SizeX / 2, Rectangle.Y + Rectangle.SizeY / 2, Rectangle.Z + Rectangle.SizeZ / 2)
        End Function

#End If

        ''' <summary>
        ''' Calculates the area covered by a size.
        ''' </summary>
        <Extension>
        Public Function Area(Size As Size) As Integer
            Return Size.Height * Size.Width
        End Function

        <Extension>
        Public Function Radius(Point As Point) As Double
            Return Math.Sqrt(Point.X * Point.X + Point.Y * Point.Y)
        End Function

        ''' <summary>
        ''' Calculate a reverse color.
        ''' </summary>
        ''' <param name="OrigColor">Original color</param>
        ''' <returns>Reverse color.</returns>
        <Extension>
        Public Function InverseColor(OrigColor As Color) As Color
            Return Color.FromArgb(Not OrigColor.ToArgb())
        End Function

        ''' <summary>
        ''' Calculates the area covered by a size.
        ''' </summary>
        <Extension>
        Public Function Area(Size As SizeF) As Single
            Return Size.Height * Size.Width
        End Function
        <Extension>
        Public Function Radius(Point As PointF) As Double
            Return Math.Sqrt(Point.X * Point.X + Point.Y * Point.Y)
        End Function

        <Extension>
        Public Function DistanceTo(Point1 As Point, Point2 As Point) As Double
            Return (Point1 - New Size(Point2)).Radius()
        End Function

        <Extension>
        Public Function DistanceTo(Point1 As PointF, Point2 As PointF) As Double
            Return (Point1 - New SizeF(Point2)).Radius()
        End Function

        <Extension>
        Public Function Acos(Point As PointF) As Double
            Return Math.Acos(Point.Y / Point.X)
        End Function

        <Extension>
        Public Function Asin(Point As PointF) As Double
            Return Math.Asin(Point.Y / Point.X)
        End Function

        <Extension>
        Public Function Atan(Point As PointF) As Double
            Return Math.Atan2(Point.Y, Point.X)
        End Function

        <Extension>
        Public Function CenterPoint(Rectangle As Rectangle) As Point
            Return New Point(Rectangle.Left + Rectangle.Width \ 2, Rectangle.Top + Rectangle.Height \ 2)
        End Function

        <Extension>
        Public Function CenterPoint(Rectangle As RectangleF) As PointF
            Return New PointF(Rectangle.Left + Rectangle.Width / 2, Rectangle.Top + Rectangle.Height / 2)
        End Function

#If NETFRAMEWORK AndAlso Not NET45_OR_GREATER Then
        ''' <summary>
        ''' Adjusts a string to a display with specified width by wrapping complete words.
        ''' </summary>
        ''' <param name="Msg"></param>
        ''' <param name="LineWidth">Display width. If omitted, defaults to console window width.</param>
        ''' <param name="WordDelimiter">Word separator character.</param>
        ''' <param name="FillChar">Fill character.</param>
        <Extension, SecuritySafeCritical>
        Public Function LineFormat(Msg As String, Optional LineWidth As Integer? = Nothing, Optional WordDelimiter As Char = " "c, Optional FillChar As Char = " "c) As String

            Dim Width As Integer

            If LineWidth.HasValue Then
                Width = LineWidth.Value
            Else

                If Win32API.GetFileType(Win32API.GetStdHandle(NativeConstants.StdHandle.Output)) <> NativeConstants.Win32FileType.Char Then
                    Width = 79
                Else
                    Width = Console.WindowWidth - 1
                End If

            End If

            Dim origLines = Msg.Nz().Replace(vbCr, "").Split({vbLf(0)})

            Dim resultLines As New List(Of String)(origLines.Length)

            For Each origLine In origLines

                Dim Result As New StringBuilder

                Dim Line As New StringBuilder(Width)

                For Each Word In origLine.Split(WordDelimiter)
                    If Word.Length >= Width Then
                        Result.AppendLine(Word)
                        Continue For
                    End If
                    If Word.Length + Line.Length >= Width Then
                        Result.AppendLine(Line.ToString())
                        Line.Length = 0
                    End If
                    If Line.Length > 0 Then
                        Line.Append(WordDelimiter)
                    End If
                    Line.Append(Word)
                Next

                If Line.Length > 0 Then
                    Result.Append(Line)
                End If

                resultLines.Add(Result.ToString())

            Next

#If NET40_OR_GREATER Then
            Return String.Join(Environment.NewLine, resultLines)
#Else
            Return String.Join(Environment.NewLine, resultLines.ToArray())
#End If

        End Function
#End If

    End Module

End Namespace
