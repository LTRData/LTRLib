// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System.IO;
using System.Text;

namespace LTRLib.IO;

/// <summary>
/// Buffered version of the StreamWriter class. Writes to a MemoryStream internally and flushes
/// writes out contents of MemoryStream when WriteTo() or ToArray() are called.
/// </summary>
public class BufferedStreamWriter : StreamWriter
{

    /// <summary>
    /// Creates a new instance of BufferedBinaryWriter.
    /// </summary>
    /// <param name="encoding">Specifies which text encoding to use.</param>
    public BufferedStreamWriter(Encoding encoding) : base(new MemoryStream(), encoding)
    {
    }

    /// <summary>
    /// Creates a new instance of BufferedBinaryWriter using System.Text.Encoding.Unicode text encoding.
    /// </summary>
    public BufferedStreamWriter() : base(new MemoryStream(), Encoding.Unicode)
    {
    }

    /// <summary>
    /// Writes current contents of internal MemoryStream to another stream and resets
    /// this BufferedBinaryWriter to empty state.
    /// </summary>
    /// <param name="stream"></param>
    public void WriteTo(Stream stream)
    {
        Flush();
        {
            var withBlock = (MemoryStream)BaseStream;
            withBlock.WriteTo(stream);
            withBlock.SetLength(0L);
            withBlock.Position = 0L;
        }

        stream.Flush();
    }

    /// <summary>
    /// Extracts current contents of internal MemoryStream to a new byte array and resets
    /// this BufferedBinaryWriter to empty state.
    /// </summary>
    public byte[] ToArray()
    {
        Flush();
        var withBlock = (MemoryStream)BaseStream;
        var ToArrayRet = withBlock.ToArray();
        withBlock.SetLength(0L);
        withBlock.Position = 0L;

        return ToArrayRet;
    }

    /// <summary>
    /// Clears contents of internal MemoryStream.
    /// </summary>
    public void Clear()
    {
        if (m_Disposed == true)
        {
            return;
        }

        {
            var withBlock = BaseStream;
            withBlock.SetLength(0L);
            withBlock.Position = 0L;
        }
    }

    protected bool m_Disposed;

    protected override void Dispose(bool disposing)
    {
        m_Disposed = true;

        base.Dispose(disposing);
    }

}