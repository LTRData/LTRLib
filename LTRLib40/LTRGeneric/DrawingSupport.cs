using System;

namespace LTRLib.Extensions;

public static class DrawingSupport
{
    public static string PngImageToBase64Url(byte[] img, int offset, int length)
    {
        if (img is null || img.Length == 0)
        {
            return null!;
        }

        var imgstr = Convert.ToBase64String(img, offset, length);

        return $"data:image/png;base64,{imgstr}";
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public static string PngImageToBase64Url(ReadOnlySpan<byte> img)
    {
        if (img.IsEmpty)
        {
            return null!;
        }

        var imgstr = Convert.ToBase64String(img);

        return $"data:image/png;base64,{imgstr}";
    }
#endif
}
