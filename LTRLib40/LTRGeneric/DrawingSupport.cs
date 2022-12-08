using System;

namespace LTRLib.Extensions;

public static class DrawingSupport
{
    public static string PngImageToBase64Url(byte[] img)
    {
        if (img is null || img.Length == 0)
        {
            return null!;
        }

        var imgstr = Convert.ToBase64String(img);

        return $"data:image/png;base64,{imgstr}";
    }
}
