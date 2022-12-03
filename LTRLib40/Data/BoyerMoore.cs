/*
 * BoyerMoore
 * Source: https://stackoverflow.com/questions/283456/byte-array-pattern-search
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace LTRLib.Data;

public class BoyerMoore : IEnumerable<int>
{
    private const int ALPHABET_SIZE = 256;

    private readonly byte[] text;
    private readonly byte[] pattern;

    private readonly int offset;
    private readonly int length;

    private readonly int[] last;
    private readonly int[] match;
    private readonly int[] suffix;

    public BoyerMoore(byte[] pattern, byte[] text)
        : this(pattern, text, 0, text.Length)
    {
    }

    public BoyerMoore(byte[] pattern, byte[] text, int offset, int length)
    {
        if (offset < 0 || offset >= text.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (length < 0 || checked(length - offset) > text.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        this.text = text;
        this.pattern = pattern;
        this.offset = offset;
        this.length = length;
        last = new int[ALPHABET_SIZE];
        match = new int[pattern.Length];
        suffix = new int[pattern.Length];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /**
    * Searches the pattern in the text.
    * returns the position of the first occurrence, if found and -1 otherwise.
    */
    public IEnumerator<int> GetEnumerator()
    {
        // Preprocessing
        ComputeLast();
        ComputeMatch();

        // Searching
        var i = pattern.Length - 1;
        var j = i;

        while (i < length)
        {
            if (pattern[j] == text[i + offset])
            {
                if (j == 0)
                {
                    yield return i;
                }
                else
                {
                    j--;
                    i--;

                    continue;
                }
            }

            i += pattern.Length - j - 1 + Math.Max(j - last[text[i + offset]], match[j]);
            j = pattern.Length - 1;
        }

        yield break;
    }

    /**
    * Computes the function last and stores its values in the array last.
    * last(Char ch) = the index of the right-most occurrence of the character ch
    *                                                           in the pattern; 
    *                 -1 if ch does not occur in the pattern.
    */
    private void ComputeLast()
    {
        for (var k = 0; k < last.Length; k++)
        {
            last[k] = -1;
        }

        for (var j = pattern.Length - 1; j >= 0; j--)
        {
            if (last[pattern[j]] < 0)
            {
                last[pattern[j]] = j;
            }
        }
    }

    /*
    * Computes the function match and stores its values in the array match.
    * match(j) = min{ s | 0 < s <= j && p[j-s]!=p[j]
    *                            && p[j-s+1]..p[m-s-1] is suffix of p[j+1]..p[m-1] }, 
    *                                                         if such s exists, else
    *            min{ s | j+1 <= s <= m 
    *                            && p[0]..p[m-s-1] is suffix of p[j+1]..p[m-1] }, 
    *                                                         if such s exists,
    *            m, otherwise,
    * where p is the pattern and m is its length.
    */
    private void ComputeMatch()
    {
        /* Phase 1 */
        for (var j = 0; j < match.Length; j++)
        {
            match[j] = match.Length;
        } //O(m) 

        ComputeSuffix(); //O(m)

        /* Phase 2 */
        //Uses an auxiliary array, backwards version of the KMP failure function.
        //suffix[i] = the smallest j > i s.t. p[j..m-1] is a prefix of p[i..m-1],
        //if there is no such j, suffix[i] = m

        //Compute the smallest shift s, such that 0 < s <= j and
        //p[j-s]!=p[j] and p[j-s+1..m-s-1] is suffix of p[j+1..m-1] or j == m-1}, 
        //                                                         if such s exists,
        for (var i = 0; i < match.Length - 1; i++)
        {
            var j = suffix[i + 1] - 1; // suffix[i+1] <= suffix[i] + 1
            match[j] = suffix[i] > j ? j - i : Math.Min(j - i + match[i], match[j]);
        }

        /* Phase 3 */
        //Uses the suffix array to compute each shift s such that
        //p[0..m-s-1] is a suffix of p[j+1..m-1] with j < s < m
        //and stores the minimum of this shift and the previously computed one.
        if (suffix[0] < pattern.Length)
        {
            for (var j = suffix[0] - 1; j >= 0; j--)
            {
                if (suffix[0] < match[j])
                {
                    match[j] = suffix[0];
                }
            }

            var l = suffix[0];
            for (var k = suffix[l]; k < pattern.Length; k = suffix[k])
            {
                while (l < k)
                {
                    if (match[l] > k)
                    {
                        match[l] = k;
                    }

                    l++;
                }
            }
        }
    }

    /*
    * Computes the values of suffix, which is an auxiliary array, 
    * backwards version of the KMP failure function.
    * 
    * suffix[i] = the smallest j > i s.t. p[j..m-1] is a prefix of p[i..m-1],
    * if there is no such j, suffix[i] = m, i.e. 

    * p[suffix[i]..m-1] is the longest prefix of p[i..m-1], if suffix[i] < m.
    */
    private void ComputeSuffix()
    {
        suffix[suffix.Length - 1] = suffix.Length;
        var j = suffix.Length - 1;
        for (var i = suffix.Length - 2; i >= 0; i--)
        {
            while (j < suffix.Length - 1 && !pattern[j].Equals(pattern[i]))
            {
                j = suffix[j + 1] - 1;
            }

            if (pattern[j] == pattern[i])
            {
                j--;
            }

            suffix[i] = j + 1;
        }
    }

}
