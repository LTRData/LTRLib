// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB0003 // Type or member is obsolete
#pragma warning disable SYSLIB0004 // Type or member is obsolete

namespace LTRLib.Services.gsdll;


[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
public class GSInstance : IDisposable
{

    #region gsdll32.dll declarations
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    private static class NativeMethods
    {
        [DllImport("gsdll32.dll", CharSet = CharSet.Ansi)]
        public static extern void gsapi_delete_instance(IntPtr pinstance);
        [DllImport("gsdll32.dll", CharSet = CharSet.Ansi)]
        public static extern int gsapi_new_instance(out SafeGSHandle pinstance, IntPtr handle);
        [DllImport("gsdll32.dll", CharSet = CharSet.Ansi)]
        public static extern int gsapi_init_with_args(SafeGSHandle pinstance, int argc, string[] argv);
        [DllImport("gsdll32.dll", CharSet = CharSet.Ansi)]
        public static extern int gsapi_exit(SafeGSHandle pinstance);
    }
    #endregion

    #region SafeHandle for instances
    /// <summary>
    /// Encapsulates a kernel object handle that is closed by calling CloseHandle function in kernel32.dll.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    [ComVisible(false)]
    public class SafeGSHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Closes contained handle by calling gsapi_delete_instance() API.
        /// </summary>
        /// <returns>Always returns true.</returns>
        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            NativeMethods.gsapi_delete_instance(handle);
            return true;
        }

        /// <summary>
        /// Creates a new empty instance. This constructor is used by native to managed
        /// handle marshaller.
        /// </summary>
        protected SafeGSHandle() : base(ownsHandle: true)
        {
        }

        /// <summary>
        /// Initiates a new instance with an existing open handle.
        /// </summary>
        /// <param name="open_handle">Existing open handle.</param>
        /// <param name="owns_handle">Indicates whether handle should be closed when this
        /// instance is released.</param>
        public SafeGSHandle(IntPtr open_handle, bool owns_handle) : base(owns_handle)
        {
            SetHandle(open_handle);
        }
    }
    #endregion

    public SafeGSHandle SafeHandle { get; private set; }

    #if NETFRAMEWORK

    [SecurityCritical]
    public GSInstance(IWin32Window owner) : this(owner.Handle)
    {
    }

    #endif

    [SecurityCritical]
    public GSInstance() : this(IntPtr.Zero)
    {
    }

    [SecurityCritical]
    public GSInstance(IntPtr ownerWindow)
    {
        var rc = NativeMethods.gsapi_new_instance(out var safeHandle, ownerWindow);
        
        if (rc < 0)
        {
            throw new Exception($"Error initializing GhostScript instance (code {rc})");
        }

        SafeHandle = safeHandle;
    }

    [SecurityCritical]
    public void InitWithArgs(string[] args)
    {

        var rc = NativeMethods.gsapi_init_with_args(SafeHandle, args.Length, args);
        if (rc < 0)
        {
            throw new Exception($"GhostScript error (code {rc})");
        }

    }

    [SecurityCritical]
    public void Exit()
    {

        var rc = NativeMethods.gsapi_exit(SafeHandle);
        if (rc < 0)
        {
            throw new Exception($"GhostScript error (code {rc})");
        }

    }

    [SecurityCritical]
    public void ConvertToPDF(string[] sourcefiles, string targetfile)
    {

        var parameters = new List<string>() { "ps2pdf", "-dNOPAUSE", "-dBATCH", "-dSAFER", "-sDEVICE=pdfwrite", $"-sOutputFile={targetfile}", "-c", ".setpdfwrite", "-f" };

        parameters.AddRange(sourcefiles);

        try
        {
            InitWithArgs(parameters.ToArray());
        }

        finally
        {
            Exit();

        }

    }

    #region IDisposable Support
    private bool disposedValue; // To detect redundant calls

    // IDisposable
    [SecurityCritical]
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            SafeHandle.Dispose();

            // TODO: set large fields to null.
        }

        disposedValue = true;
    }

    // TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    [SecuritySafeCritical]
    ~GSInstance()
    {
        // Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        Dispose(false);
    }

    // This code added by Visual Basic to correctly implement the disposable pattern.
    [SecuritySafeCritical]
    public void Dispose()
    {
        // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

}
