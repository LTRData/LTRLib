using System.Security;

#pragma warning disable IDE0130 // Namespace does not match folder structure

#if !NET5_0_OR_GREATER

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}

#endif

#if NETFRAMEWORK && !NET35_OR_GREATER

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class ExtensionAttribute : Attribute
    {
    }
}

namespace System
{
    internal delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);

    internal delegate T2 Func<in T1, out T2>(T1 arg1);
}

#endif

#if NETFRAMEWORK || NETSTANDARD

namespace System.Runtime.Versioning
{
    // 
    // Summary:
    // Indicates that an API is supported for a specified platform or operating system.
    // If a version is specified, the API cannot be called from an earlier version.
    // Multiple attributes can be applied to indicate support on multiple operating
    // systems.
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    internal sealed class SupportedOSPlatformAttribute(string platformName) : Attribute
    {
        public string PlatformName { get; set; } = platformName;
    }
}

#endif

#if NET35_OR_GREATER && !NET45_OR_GREATER

namespace System.Runtime.CompilerServices
{

    // 
    // Summary:
    // Represents an operation that schedules continuations when it completes.
    internal partial interface ICriticalNotifyCompletion
    {
        // 
        // Summary:
        // Schedules the continuation action that's invoked when the instance completes.
        // 
        // Parameters:
        // continuation:
        // The action to invoke when the operation completes.
        // 
        // Exceptions:
        // T:System.ArgumentNullException:
        // The continuation argument is null (Nothing in Visual Basic).
        void OnCompleted(Action continuation);

        // 
        // Summary:
        // Schedules the continuation action that's invoked when the instance completes.
        // 
        // Parameters:
        // continuation:
        // The action to invoke when the operation completes.
        // 
        // Exceptions:
        // T:System.ArgumentNullException:
        // The continuation argument is null (Nothing in Visual Basic).
        [SecurityCritical]
        void UnsafeOnCompleted(Action continuation);
    }

}

#endif

