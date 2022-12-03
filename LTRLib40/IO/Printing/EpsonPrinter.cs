// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.Text;

namespace LTRLib.IO.Printing;

public static class EpsonPrinter
{
    public static string TagToEscape(string Text)
    {
        return Text
            .Replace("<H0>", NormalFont)
            .Replace("<H1>", WideFont)
            .Replace("<H2>", HighFont)
            .Replace("<H3>", LargeFont)
            .Replace("<C1>", PrimaryColor)
            .Replace("<C2>", SecondaryColor);

    }

    public static string EscapeToTag(string Text)
    {
        return Text
            .Replace(NormalFont, "<H0>")
            .Replace(WideFont, "<H1>")
            .Replace(HighFont, "<H2>")
            .Replace(LargeFont, "<H3>")
            .Replace(PrimaryColor, "<C1>")
            .Replace(SecondaryColor, "<C2>");

    }

    public static string GetPrintFlashBitmapControl(byte ImageId, byte Scale) => Encoding.Default.GetString(new[] { (byte)0x1C, (byte)0x70, ImageId, Scale });

    public static string GetPrintFlashBitmapControl(byte ImageId) => Encoding.Default.GetString(new[] { (byte)0x1C, (byte)0x70, ImageId, (byte)0 });

    public static string GetPrintFlashBitmapControl() => Encoding.Default.GetString(new[] { (byte)0x1C, (byte)0x70, (byte)1, (byte)0 });

    public enum InternationalCharacterSet : byte
    {
        USA = 0,
        France = 1,
        Germany = 2,
        UK = 3,
        Denmark_I = 4,
        Sweden = 5,
        Italy = 6,
        Spain_I = 7,
        Japan = 8,
        Norway = 9,
        Denmark_II = 10,
        Spain_II = 11,
        LatinAmerica = 12,
        Korea = 13
    }

    public static byte[] GetInternationalCharacterSetControl(InternationalCharacterSet CharSet) => new byte[] { 0x1B, 0x52, (byte)CharSet };

    public enum CharacterCodeTable : byte
    {

        PC437_USA_StandardEurope = 0,
        Katakana = 1,
        PC850_Multilingual = 2,
        PC860_Portugese = 3,
        PC863_CanadianFrench = 4,
        PC865_Nordic = 5,

        WPC1252 = 16,
        PC866_Cyrillic_II = 17,
        PC852_Latin_II = 18,
        PC858 = 19,

        Thai42 = 20,
        Thai11 = 21,
        Thai13 = 22,
        Thai14 = 23,
        Thai16 = 24,
        Thai17 = 25,
        Thai18 = 26,

        SpacePage = 255

    }

    public static byte[] GetCharacterCodeControl(CharacterCodeTable CodeTable) => new byte[] { 0x1B, 0x74, (byte)CodeTable };

    public enum RealtimeStatusRequest : byte
    {
        PrinterStatus = 1,
        OfflineStatus = 2,
        ErrorStatus = 3,
        PaperRollSensorStatus = 4
    }

    public static byte[] GetTransmitStatusRealtimeControl(RealtimeStatusRequest TransmitStatus) => new byte[] { 0x10, 0x4, (byte)TransmitStatus };

    public enum BufferedStatusRequest : byte
    {
        PaperSensor = 1,
        DrawerKickout = 2
    }

    public static byte[] GetTransmitStatusBufferedControl(BufferedStatusRequest TransmitStatus) => new byte[] { 0x1D, 0x72, (byte)TransmitStatus };

    public enum RealtimePrinterStatusResponse : byte
    {
        DrawerOpen = 4,
        Online = 8
    }

    public enum RealtimeOfflineStatusResponse : byte
    {
        CoverOpen = 4,
        PaperFeed = 8,
        PaperStop = 32
    }

    public enum RealtimePaperRollStatusResponse : byte
    {
        NearEnd = 12,
        PaperEnd = 96
    }

    public enum BufferedPaperSensorStatusResponse : byte
    {
        PaperNearEnd = 3,
        PaperEnd = 12 // ' Not used
    }

    public enum BufferedDrawerOpenStatusResponse : byte
    {
        DrawerOpen = 1
    }

    public enum BarCodeType : byte
    {
        UPC_A = 0x0,
        UPC_B = 0x1,
        EAN13 = 0x2,
        EAN8 = 0x3,
        CODE39 = 0x4,
        I2of5 = 0x5,
        CODABAR = 0x6,
        Code93 = 0x7,
        Code128 = 0x8
    }

    public enum BarCodeHRIPosition : byte
    {
        None = 0,
        Above = 1,
        Below = 2,
        Both = 3
    }

    public static byte[] GetBarCodeControl(BarCodeType BarCodeType, BarCodeHRIPosition BarCodeHRIPosition, byte BarCodeHeight, string Data) => GetBarCodeControl(BarCodeType, BarCodeHRIPosition, BarCodeHeight, Encoding.Default.GetBytes(Data));

    public static byte[] GetBarCodeControl(BarCodeType BarCodeType, BarCodeHRIPosition BarCodeHRIPosition, byte BarCodeHeight, byte[] Data)
    {
        var lst = new List<byte>() { 0x1D, 0x48, (byte)BarCodeHRIPosition, 0x1D, 0x68, BarCodeHeight, 0x1D, 0x6B, (byte)((byte)BarCodeType + 0x41), (byte)Data.Length };
        lst.AddRange(Data);
        return lst.ToArray();
    }

    public static byte[] GetDrawerKickoutRealTimeControl(byte DrawerNumber, short PulseTime) => new byte[] { 0x10, 0x14, 0x1, DrawerNumber, (byte)Math.Round(PulseTime / 100d) };

    public static byte[] GetDrawerKickoutBufferedControl(byte DrawerNumber, short OnTime, short OffTime) => new byte[] { 0x1B, 0x70, DrawerNumber, (byte)Math.Round(OnTime / 2d), (byte)Math.Round(OffTime / 2d) };

    public static byte[] GetLeftMarginControl(ushort value) => new byte[] { 0x1D, 0x4C, (byte)(value & 0xFF), (byte)(value >> 8) };

    public static string CenterReceiptText(string TextRow, int NormalFontMaxChars)
    {

        var TextWidth = default(int);

        var Row = TextRow;
        var FontFactor = 1;
        while (!string.IsNullOrEmpty(Row))
        {
            if (Row.StartsWith(NormalFont))
            {
                FontFactor = 1;
                Row = Row.Substring(NormalFont.Length);
            }
            else if (Row.StartsWith(HighFont))
            {
                FontFactor = 1;
                Row = Row.Substring(HighFont.Length);
            }
            else if (Row.StartsWith(WideFont))
            {
                FontFactor = 2;
                Row = Row.Substring(WideFont.Length);
            }
            else if (Row.StartsWith(LargeFont))
            {
                FontFactor = 2;
                Row = Row.Substring(LargeFont.Length);
            }
            else
            {
                TextWidth += FontFactor;
                Row = Row.Substring(1);
            }
        }

        if (TextWidth == 0 || TextWidth >= NormalFontMaxChars)
        {
            return TextRow;
        }
        else
        {
            return new string(' ', (NormalFontMaxChars - TextWidth) / 2) + TextRow;
        }

    }

    public static readonly string NormalFont = Encoding.Default.GetString(new[] { (byte)0x1B, (byte)0x21, (byte)0x0 });
    public static readonly string HighFont = Encoding.Default.GetString(new[] { (byte)0x1B, (byte)0x21, (byte)0x10 });
    public static readonly string WideFont = Encoding.Default.GetString(new[] { (byte)0x1B, (byte)0x21, (byte)0x20 });
    public static readonly string LargeFont = Encoding.Default.GetString(new[] { (byte)0x1B, (byte)0x21, (byte)0x30 });

    public static readonly string PrimaryColor = Encoding.Default.GetString(new[] { (byte)0x1B, (byte)0x72, (byte)0x0 });
    public static readonly string SecondaryColor = Encoding.Default.GetString(new[] { (byte)0x1B, (byte)0x72, (byte)0x1 });

    public static readonly byte[] SelectPrinter = { 0x1B, 0x3D, 0x1 };
    public static readonly byte[] SelectDisplay = { 0x1B, 0x3D, 0x2 };

    public static readonly byte[] DisplayGoToStart = { 0xD, 0xD };

    public static readonly byte[] PageFeed = { 0xC };

    public static readonly byte[] PaperCut = { 0x1B, 0x6D };

}