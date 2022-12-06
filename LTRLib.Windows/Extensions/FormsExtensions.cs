// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security;
using System.Text;
using System.Windows.Forms;

#if NET30_OR_GREATER || NETCOREAPP
using System.Windows.Media.Media3D;
#endif

using LTRLib.IO;

namespace LTRLib.Extensions;

public static class FormsExtensions
{
    public static Form? GetTopMostOwner(this Form? form)
    {
        while (form?.Owner is not null)
        {
            form = form.Owner;
        }

        return form;
    }

    public static string? ToBase64Url(this Image img)
    {
        using var buffer = new MemoryStream();
        img.Save(buffer, ImageFormat.Png);
        return DrawingSupport.PngImageToBase64Url(buffer.ToArray());
    }

    #if NET30_OR_GREATER || NETCOREAPP

    /// <summary>
    /// Calculates the area covered by a size.
    /// </summary>
    public static double Volume(this Size3D Size)
    {
        return Size.X * Size.Y * Size.Z;
    }

    public static double Radius(this Point3D Point)
    {
        return Math.Sqrt(Math.Pow(Math.Sqrt(Point.X * Point.X + Point.Y * Point.Y), 2d) + Point.Z * Point.Z);
    }

    public static double DistanceTo(this Point3D Point1, Point3D Point2)
    {
        return (Point1 - new Vector3D(Point2.X, Point2.Y, Point2.Z)).Radius();
    }

    public static Point3D CenterPoint(this Rect3D Rectangle)
    {
        return new Point3D(Rectangle.X + Rectangle.SizeX / 2d, Rectangle.Y + Rectangle.SizeY / 2d, Rectangle.Z + Rectangle.SizeZ / 2d);
    }

    #endif

    /// <summary>
    /// Calculates the area covered by a size.
    /// </summary>
    public static int Area(this Size Size)
    {
        return Size.Height * Size.Width;
    }

    public static double Radius(this Point Point)
    {
        return Math.Sqrt(Point.X * Point.X + Point.Y * Point.Y);
    }

    /// <summary>
    /// Calculate a reverse color.
    /// </summary>
    /// <param name="OrigColor">Original color</param>
    /// <returns>Reverse color.</returns>
    public static Color InverseColor(this Color OrigColor)
    {
        return Color.FromArgb(~OrigColor.ToArgb());
    }

    /// <summary>
    /// Calculates the area covered by a size.
    /// </summary>
    public static float Area(this SizeF Size)
    {
        return Size.Height * Size.Width;
    }
    public static double Radius(this PointF Point)
    {
        return Math.Sqrt((double)(Point.X * Point.X + Point.Y * Point.Y));
    }

    public static double DistanceTo(this Point Point1, Point Point2)
    {
        return (Point1 - new Size(Point2)).Radius();
    }

    public static double DistanceTo(this PointF Point1, PointF Point2)
    {
        return (Point1 - new SizeF(Point2)).Radius();
    }

    public static double Acos(this PointF Point)
    {
        return Math.Acos((double)(Point.Y / Point.X));
    }

    public static double Asin(this PointF Point)
    {
        return Math.Asin((double)(Point.Y / Point.X));
    }

    public static double Atan(this PointF Point)
    {
        return Math.Atan2((double)Point.Y, (double)Point.X);
    }

    public static Point CenterPoint(this Rectangle Rectangle)
    {
        return new Point(Rectangle.Left + Rectangle.Width / 2, Rectangle.Top + Rectangle.Height / 2);
    }

    public static PointF CenterPoint(this RectangleF Rectangle)
    {
        return new PointF(Rectangle.Left + Rectangle.Width / 2f, Rectangle.Top + Rectangle.Height / 2f);
    }

    #if NETFRAMEWORK && !NET45_OR_GREATER
    /// <summary>
    /// Adjusts a string to a display with specified width by wrapping complete words.
    /// </summary>
    /// <param name="Msg"></param>
    /// <param name="LineWidth">Display width. If omitted, defaults to console window width.</param>
    /// <param name="WordDelimiter">Word separator character.</param>
    /// <param name="FillChar">Fill character.</param>
    [SecuritySafeCritical]
    public static string LineFormat(this string Msg, int? LineWidth = default, char WordDelimiter = ' ', char FillChar = ' ')
    {

        int Width;

        if (LineWidth.HasValue)
        {
            Width = LineWidth.Value;
        }

        else if (IO.Win32API.GetFileType(IO.Win32API.GetStdHandle(NativeConstants.StdHandle.Output)) != NativeConstants.Win32FileType.Char)
        {
            Width = 79;
        }
        else
        {
            Width = Console.WindowWidth - 1;
        }

        var origLines = Msg.Nz().Replace("\r", "").Split('\n');

        var resultLines = new List<string>(origLines.Length);

        foreach (var origLine in origLines)
        {
            var Result = new StringBuilder();

            var Line = new StringBuilder(Width);

            foreach (var Word in origLine.Split(WordDelimiter))
            {
                if (Word.Length >= Width)
                {
                    Result.AppendLine(Word);
                    continue;
                }
                if (Word.Length + Line.Length >= Width)
                {
                    Result.AppendLine(Line.ToString());
                    Line.Length = 0;
                }
                if (Line.Length > 0)
                {
                    Line.Append(WordDelimiter);
                }
                Line.Append(Word);
            }

            if (Line.Length > 0)
            {
                Result.Append(Line);
            }

            resultLines.Add(Result.ToString());
        }

#if NET40_OR_GREATER

        return string.Join(Environment.NewLine, resultLines);

#else

        return string.Join(Environment.NewLine, resultLines.ToArray());

#endif
    }

#endif
}

