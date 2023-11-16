using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Text;
using static LTRLib.IO.NativeConstants;
using static LTRLib.IO.Win32API;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB0003 // Type or member is obsolete

namespace LTRLib.IO;

[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
[SupportedOSPlatform("windows")]
public class VolumeMountPointEnumerator(string VolumePath) : IEnumerable<string>
{
    public string VolumePath { get; set; } = VolumePath;

    [SecuritySafeCritical]
    public IEnumerator<string> GetEnumerator() => new Enumerator(VolumePath);

    [SecuritySafeCritical]
    private IEnumerator IEnumerable_GetEnumerator() => GetEnumerator();

    [SecuritySafeCritical]
    IEnumerator IEnumerable.GetEnumerator() => IEnumerable_GetEnumerator();

    public sealed class Enumerator(string volumePath) : IEnumerator<string>
    {
        private SafeFindVolumeMountPointHandle? _handle;
        private StringBuilder _sb = new(32767);

        public string Current
        {
            [SecuritySafeCritical]
            get
            {
                if (disposedValue)
                {
                    throw new ObjectDisposedException("VolumeMountPointEnumerator.Enumerator");
                }

                return _sb.ToString();
            }
        }

        private object IEnumerator_Current
        {
            [SecuritySafeCritical]
            get => Current;
        }

        object IEnumerator.Current
        {
            [SecuritySafeCritical]
            get => IEnumerator_Current;
        }

        [SecuritySafeCritical]
        public bool MoveNext()
        {

            if (disposedValue)
            {
                throw new ObjectDisposedException("VolumeMountPointEnumerator.Enumerator");
            }

            if (_handle is null)
            {
                _handle = FindFirstVolumeMountPoint(volumePath, _sb, _sb.Capacity);
                if (!_handle.IsInvalid)
                {
                    return true;
                }
                else if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_FILES)
                {
                    return false;
                }
                else
                {
                    throw new Win32Exception();
                }
            }
            else if (FindNextVolumeMountPoint(_handle, _sb, _sb.Capacity))
            {
                return true;
            }
            else if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_FILES)
            {
                return false;
            }
            else
            {
                throw new Win32Exception();
            }

        }

        [SecuritySafeCritical]
        private void Reset() => throw new NotImplementedException();

        [SecuritySafeCritical]
        void IEnumerator.Reset() => Reset();

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        // IDisposable
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                if (_handle is not null)
                {
                    _handle.Dispose();
                    _handle = null!;
                }

                // TODO: set large fields to null.
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
                _sb.Clear();
#endif
                _sb = null!;
            }

            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        [SecuritySafeCritical]
        ~Enumerator()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
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

}