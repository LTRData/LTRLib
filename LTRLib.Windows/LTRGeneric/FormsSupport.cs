// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB0003 // Type or member is obsolete

namespace LTRLib.LTRGeneric;

public static class FormsSupport
{
    private const uint SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000U;
    private const uint SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001U;
    private const uint SPIF_SENDWININICHANGE = 0x2U;
    private const uint SPIF_UPDATEINIFILE = 0x1U;

    /// <summary>
    /// Returns Form object that is currently active, if that Form is created by current thread. If no Form
    /// is active or if currently active Form belongs to another thread, Nothing is returned.
    /// </summary>
    public static Form? GetCurrentThreadActiveForm()
    {
        var GetCurrentThreadActiveFormRet = Form.ActiveForm;

        if (GetCurrentThreadActiveFormRet is not null && GetCurrentThreadActiveFormRet.InvokeRequired == true)
        {
            GetCurrentThreadActiveFormRet = null;
        }

        return GetCurrentThreadActiveFormRet;
    }

    [SecuritySafeCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static int GetForegroundLockTimeout(ref uint ms)
    {
        if (IO.Win32API.GetSystemParametersDWORD(SPI_GETFOREGROUNDLOCKTIMEOUT, 0U, out ms, 0U) != 0)
        {
            return 0;
        }
        else
        {
            return Marshal.GetLastWin32Error();
        }
    }

    [SecuritySafeCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static int SetForegroundLockTimeout(uint ms)
    {
        if (IO.Win32API.SetSystemParametersDWORD(SPI_SETFOREGROUNDLOCKTIMEOUT, 0U, (nint)ms, SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE) != 0)
        {
            return 0;
        }
        else
        {
            return Marshal.GetLastWin32Error();
        }
    }

    public static Font? FindLargestFont(Graphics Graphics, FontFamily FontFamily, float MaxFontSize, FontStyle FontStyle, GraphicsUnit FontUnit, RectangleF TextRectangle, string Text)
    {
        Font? FindLargestFontRet = null;

        for (var FontSize = MaxFontSize; FontSize >= 1f; FontSize += -2)
        {
            FindLargestFontRet = new Font(FontFamily, FontSize, FontStyle, FontUnit);

            var RequiredRectSize = Graphics.MeasureString(Text, FindLargestFontRet, (int)Math.Round(TextRectangle.Width));

            if (RequiredRectSize.Height < TextRectangle.Height)
            {
                break;
            }

            FindLargestFontRet.Dispose();
        }

        return FindLargestFontRet;
    }

    // ' Text alignment formats
    public static readonly StringFormat sftRightAligned = new() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Far };
    public static readonly StringFormat sftLeftAligned = new() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near };
    public static readonly StringFormat sftTopAligned = new() { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Center };
    public static readonly StringFormat sftBottomAligned = new() { LineAlignment = StringAlignment.Far, Alignment = StringAlignment.Center };
    public static readonly StringFormat sftCentered = new() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
}