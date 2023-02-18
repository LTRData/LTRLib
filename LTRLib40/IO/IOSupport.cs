// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace LTRLib.IO;

public static class IOSupport
{
    public static NetworkStream OpenTcpIpStream(string host_and_port)
    {
        var @params = host_and_port.Split(':');
        if (@params.Length != 2)
        {
            throw new ArgumentException("Needs host:port form", nameof(host_and_port));
        }

        var Socket = OpenTcpIpSocket(@params[0], int.Parse(@params[1]));
        Socket.NoDelay = true;
        return new NetworkStream(Socket, ownsSocket: true);
    }

    public static NetworkStream OpenTcpIpStream(string host, int port)
    {
        var Socket = OpenTcpIpSocket(host, port);
        Socket.NoDelay = true;
        return new NetworkStream(Socket, ownsSocket: true);
    }

    public static Socket OpenTcpIpSocket(string host, int port)
    {
        var Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            Socket.Connect(host, port);
        }

        catch (Exception ex)
        {
            Socket.Close();
            throw new IOException("Connection failed", ex);

        }

        return Socket;
    }

    public static NetworkStream OpenTcpIpStream(this EndPoint remoteEP)
    {
        var Socket = OpenTcpIpSocket(remoteEP);
        Socket.NoDelay = true;
        return new NetworkStream(Socket, ownsSocket: true);
    }

    public static Socket OpenTcpIpSocket(this EndPoint remoteEP)
    {
        var Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            Socket.Connect(remoteEP);
        }

        catch (Exception ex)
        {
            Socket.Close();
            throw new IOException("Connection failed", ex);

        }

        return Socket;
    }

    public static NetworkStream OpenTcpIpStream(IPAddress address, int port)
    {
        var Socket = OpenTcpIpSocket(address, port);
        Socket.NoDelay = true;
        return new NetworkStream(Socket, ownsSocket: true);
    }

    public static Socket OpenTcpIpSocket(IPAddress address, int port)
    {
        var Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            Socket.Connect(address, port);
        }

        catch (Exception ex)
        {
            Socket.Close();
            throw new IOException("Connection failed", ex);

        }

        return Socket;
    }

    public static NetworkStream OpenTcpIpStream(IPAddress[] addresses, int port)
    {
        var Socket = OpenTcpIpSocket(addresses, port);
        Socket.NoDelay = true;
        return new NetworkStream(Socket, ownsSocket: true);
    }

    public static Socket OpenTcpIpSocket(IPAddress[] addresses, int port)
    {
        var Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            Socket.Connect(addresses, port);
        }

        catch (Exception ex)
        {
            Socket.Close();
            throw new IOException("Connection failed", ex);

        }

        return Socket;
    }

    public static Stream OpenNetworkStream(this Uri uri, RemoteCertificateValidationCallback userCertificateValidationCallback)
    {
        switch (uri.Scheme)
        {
            case "https":
                {
                    return OpenSslStream(uri, userCertificateValidationCallback);
                }
            case "http":
                {
                    return OpenTcpIpStream(uri.DnsSafeHost, uri.Port);
                }

            default:
                {
                    throw new ArgumentException($"Unsupported scheme: {uri.Scheme}", nameof(uri));
                }

        }

    }

    public static SslStream OpenSslStream(this Uri uri, RemoteCertificateValidationCallback userCertificateValidationCallback)
    {

        var ssl = new SslStream(OpenTcpIpStream(uri.DnsSafeHost, uri.Port), leaveInnerStreamOpen: false, userCertificateValidationCallback: userCertificateValidationCallback);

        try
        {
            ssl.AuthenticateAsClient(uri.DnsSafeHost);
        }

        catch (Exception ex)
        {
            ssl.Close();
            throw new IOException("Connection failed", ex);

        }

        return ssl;

    }

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Copies data from a stream to another stream.
    /// </summary>
    /// <param name="SourceStream">Input stream to copy from.</param>
    /// <param name="TargetStream">Output stream to copy to.</param>
    /// <param name="BufferSize">Size of buffer to use when reading/writing.</param>
    public static void CopyStream(Stream SourceStream, Stream TargetStream, int BufferSize) => SourceStream.CopyTo(TargetStream, BufferSize);

    /// <summary>
    /// Copies data from a stream to another stream.
    /// </summary>
    /// <param name="SourceStream">Input stream to copy from.</param>
    /// <param name="TargetStream">Output stream to copy to.</param>
    public static void CopyStream(Stream SourceStream, Stream TargetStream) => SourceStream.CopyTo(TargetStream);

#else

    /// <summary>
    /// Copies data from a stream to another stream.
    /// </summary>
    /// <param name="SourceStream">Input stream to copy from.</param>
    /// <param name="TargetStream">Output stream to copy to.</param>
    /// <param name="BufferSize">Size of buffer to use when reading/writing.</param>
    public static void CopyStream(Stream SourceStream, Stream TargetStream, int BufferSize)
    {

        var argLength = -1;
        CopyStream(SourceStream, TargetStream, BufferSize, ref argLength);

    }

    /// <summary>
    /// Copies data from a stream to another stream.
    /// </summary>
    /// <param name="SourceStream">Input stream to copy from.</param>
    /// <param name="TargetStream">Output stream to copy to.</param>
    public static void CopyStream(Stream SourceStream, Stream TargetStream)
    {

        var argLength = -1;
        CopyStream(SourceStream, TargetStream, 1048576, ref argLength);

    }

#endif

    /// <summary>
    /// Copies data from a stream to another stream.
    /// </summary>
    /// <param name="SourceStream">Input stream to copy from.</param>
    /// <param name="TargetStream">Output stream to copy to.</param>
    /// <param name="BufferSize">Size of buffer to use when reading/writing.</param>
    /// <param name="Length">In: Maximum total length to copy or -1 for all
    /// of input stream. Out: Number of bytes actually copied.</param>
    public static void CopyStream(Stream SourceStream, Stream TargetStream, int BufferSize, ref int Length)
    {
        if (Length >= 0 && BufferSize > Length)
        {
            BufferSize = Length;
        }

        var Buffer = new byte[BufferSize];

        var WrittenBytes = default(int);

        var Size = BufferSize;

        for(; ;)
        {
            if (Length >= 0 && Size > Length)
            {
                Size = Length;
            }

            Size = SourceStream.Read(Buffer, 0, Size);
            if (Size <= 0)
            {
                break;
            }

            TargetStream.Write(Buffer, 0, Size);

            WrittenBytes += Size;

            if (Length >= 0)
            {
                Length -= Size;
                if (Length == 0)
                {
                    break;
                }
            }
        }

        Length = WrittenBytes;
    }

    /// <summary>
    /// Reads bytes from stream up to a sequence of bytes. The end marking sequence of bytes are
    /// extracted from stream but not included in returned array.
    /// </summary>
    /// <param name="Stream">Stream to read from.</param>
    /// <param name="Endmark">Sequence of bytes that marks end of read operation.</param>
    /// <returns>Array of read bytes without end sequence.</returns>
    [Obsolete("Use modern pipeline based reading instead")]
    public static byte[]? ReadTo(this Stream Stream, byte[] Endmark)
    {
        var Line = new List<byte>();

        for(; ;)
        {
            if (Line.Count >= Endmark.Length)
            {
                var EndMarkFound = true;
                for (int i = 0, loopTo = Endmark.Length - 1; i <= loopTo; i++)
                {
                    if (Line[Line.Count - Endmark.Length + i] != Endmark[i])
                    {
                        EndMarkFound = false;
                        break;
                    }
                }

                if (EndMarkFound == true)
                {
                    Line.RemoveRange(Line.Count - Endmark.Length, Endmark.Length);
                    break;
                }
            }

            var B = Stream.ReadByte();

            if (B < 0)
            {
                if (Line.Count == 0)
                {
                    return null;
                }
                else
                {
                    break;
                }
            }

            Line.Add((byte)B);
        }

        return Line.ToArray();
    }
}
