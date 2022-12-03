// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;

namespace LTRLib.Services.morse;

public static class MorseConversion
{
    private static readonly Dictionary<char, byte[]> _chars_to_bits = new() {
        { ' ', new byte[] {0}},
        { 'a', new byte[] {1, 0, 1, 1, 1}},
        { 'b', new byte[] {1, 1, 1, 0, 1, 0, 1, 0, 1}},
        { 'c', new byte[] { 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1} },
        { 'd', new byte[] { 1, 1, 1, 0, 1, 0, 1} },
        { 'e', new byte[] { 1} },
        { 'f', new byte[] { 1, 0, 1, 0, 1, 1, 1, 0, 1} },
        { 'g', new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 1} },
        { 'h', new byte[] { 1, 0, 1, 0, 1, 0, 1} },
        { 'i', new byte[] { 1, 0, 1} },
        { 'j', new byte[] { 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1} },
        { 'k', new byte[] { 1, 1, 1, 0, 1, 0, 1, 1, 1} },
        { 'l', new byte[] { 1, 0, 1, 1, 1, 0, 1, 0, 1} },
        { 'm', new byte[] { 1, 1, 1, 0, 1, 1, 1} },
        { 'n', new byte[] { 1, 1, 1, 0, 1} },
        { 'o', new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1} },
        { 'p', new byte[] { 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1} },
        { 'q', new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1} },
        { 'r', new byte[] { 1, 0, 1, 1, 1, 0, 1} },
        { 's', new byte[] { 1, 0, 1, 0, 1} },
        { 't', new byte[] { 1, 1, 1} },
        { 'u', new byte[] { 1, 0, 1, 0, 1, 1, 1} },
        { 'v', new byte[] { 1, 0, 1, 0, 1, 0, 1, 1, 1} },
        { 'w', new byte[] { 1, 0, 1, 1, 1, 0, 1, 1, 1} },
        { 'x', new byte[] { 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1} },
        { 'y', new byte[] { 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1} },
        { 'z', new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1} },
        { '0', new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1} },
        { '1', new byte[] { 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1} },
        { '2', new byte[] { 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1} },
        { '3', new byte[] { 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1} },
        { '4', new byte[] { 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1} },
        { '5', new byte[] { 1, 0, 1, 0, 1, 0, 1, 0, 1} },
        { '6', new byte[] { 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1} },
        { '7', new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1} },
        { '8', new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1} },
        { '9', new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1} },
        { '.', new byte[] { 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1} },
        { ',', new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1} },
        { '?', new byte[] { 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1} },
        { '=', new byte[] { 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1} }
    };
    
    public static byte[] TextToMorseCode(string text)
    {
        var num_bits = 0;

        for (int i = 0, loopTo = text.Length - 1; i <= loopTo; i++)
        {
            if (!_chars_to_bits.TryGetValue(char.ToLowerInvariant(text[i]), out var bits))
            {
                continue;
            }

            num_bits += bits.Length;

            if (i < text.Length - 1)
            {
                if (char.IsUpper(text, i) && char.IsUpper(text, i + 1))
                {
                    num_bits += 1;
                }
                else
                {
                    num_bits += 3;
                }
            }
        }

        var bitlist = new byte[num_bits];

        var j = 0;

        for (int i = 0, loopTo1 = text.Length - 1; i <= loopTo1; i++)
        {
            if (!_chars_to_bits.TryGetValue(char.ToLowerInvariant(text[i]), out var bits))
            {
                continue;
            }

            if (bits is null)
            {
                j += 3;

                continue;
            }

            Buffer.BlockCopy(bits, 0, bitlist, j, bits.Length);

            j += bits.Length;

            if (j < bitlist.Length - 3)
            {
                if (char.IsUpper(text, i) && char.IsUpper(text, i + 1))
                {
                    j += 1;
                }
                else
                {
                    j += 3;
                }
            }
        }

        return bitlist;
    }
}

