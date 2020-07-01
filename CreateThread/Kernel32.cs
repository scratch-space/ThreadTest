using System;
using System.Runtime.InteropServices;

namespace PInvoke
{
    public static partial class Kernel32
    {
        /// <summary>
        /// Constant for invalid handle value
        /// </summary>
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        /// <summary>
        /// Points to a function that notifies the host that a thread has started to execute.
        /// </summary>
        /// <param name="lpThreadParameter">A pointer to the code that has started executing</param>
        /// <returns>
        /// The return value indicates the success or failure of this function.
        /// The return value should never be set to STILL_ACTIVE (259), as noted in <see cref="GetExitCodeThread(IntPtr, out int)"/>.
        /// </returns>
        /// <remarks>
        /// The function to which <see cref="THREAD_START_ROUTINE"/> points is a callback
        /// function and must be implemented by the writer of the hosting application.
        ///
        /// Do not declare this callback function with a void return type and cast the function pointer to
        /// a pointer to <see cref="THREAD_START_ROUTINE"/> when creating the thread.
        /// Code that does this is common, but it can crash on 64-bit Windows.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate int THREAD_START_ROUTINE([In] IntPtr lpThreadParameter);

        /// <summary>
        /// Creates a thread to execute within the virtual address space of the calling process.
        /// To create a thread that runs in the virtual address space of another process, use the
        /// CreateRemoteThread function.
        /// </summary>
        /// <param name="lpThreadAttributes">
        /// A pointer to a <see cref="SECURITY_ATTRIBUTES"/> structure that determines whether the returned
        /// handle can be inherited by child processes. If <paramref name="lpThreadAttributes"/> is null,
        /// the handle cannot be inherited.
        ///
        /// The <see cref="SECURITY_ATTRIBUTES.lpSecurityDescriptor"/> member of the structure specifies a
        /// security descriptor for the new thread.If <paramref name="lpThreadAttributes"/> is null, the
        /// thread gets a default security descriptor.The ACLs in the default security descriptor for a thread
        /// come from the primary token of the creator.
        /// </param>
        /// <param name="dwStackSize">The initial size of the stack, in bytes. The system rounds
        /// this value to the nearest page. If this parameter is 0 (<see cref="SIZE_T.Zero"/>), the new thread uses the default size
        /// for the executable.</param>
        /// <param name="lpStartAddress">A pointer to the application-defined function to be executed
        /// by the thread. This pointer represents the starting address of the thread. For more
        /// information on the thread function, <see cref="THREAD_START_ROUTINE"/>.</param>
        /// <param name="lpParameter">A pointer to a variable to be passed to the thread.</param>
        /// <param name="dwCreationFlags">
        /// The flags that control the creation of the thread.
        /// <see cref="CreateProcessFlags.None"/>, <see cref="CreateProcessFlags.CREATE_SUSPENDED"/>
        /// and <see cref="CreateProcessFlags.STACK_SIZE_PARAM_IS_A_RESERVATION"/> are the only valid
        /// values for this parameter.
        /// </param>
        /// <param name="lpThreadId">A pointer to a variable that receives the thread identifier. If this
        /// parameter is null, the thread identifier is not returned.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the new thread.
        /// If the function fails, the return value is null.To get extended error information, call
        /// <see cref="GetLastError"/>.
        /// </returns>
        /// <remarks>
        /// Note that <see cref="CreateThread(SECURITY_ATTRIBUTES*, SIZE_T, THREAD_START_ROUTINE, IntPtr, CreateProcessFlags, int*)"/>
        /// may succeed even if <paramref name="lpStartAddress"/> points to data, code, or is not accessible.
        /// If the start address is invalid when the thread runs, an exception occurs, and the thread terminates.
        /// Thread termination due to a invalid start address is handled as an error exit for the thread's process. This behavior
        /// is similar to the asynchronous nature of <see cref="CreateProcessAsUser(IntPtr, string, string, SECURITY_ATTRIBUTES*, SECURITY_ATTRIBUTES*, bool, CreateProcessFlags, void*, string, ref STARTUPINFO, out PROCESS_INFORMATION)"/>,
        /// where the process is created even if it refers to invalid or missing dynamic-link libraries (DLLs).
        /// </remarks>
        [DllImport(nameof(Kernel32), SetLastError = true)]
        public static extern unsafe SafeObjectHandle CreateThread(
            SECURITY_ATTRIBUTES* lpThreadAttributes,
            SIZE_T dwStackSize,
            THREAD_START_ROUTINE lpStartAddress,
            IntPtr lpParameter,
            CreateProcessFlags dwCreationFlags,
            int* lpThreadId);


        /// <summary>
        /// Decrements a thread's suspend count. When the suspend count is decremented to zero, the execution of the thread is resumed.
        /// </summary>
        /// <param name="hThread">
        /// A handle to the thread to be restarted.
        /// This handle must have the THREAD_SUSPEND_RESUME access right. For more information, see Thread Security and Access Rights.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the thread's previous suspend count.
        /// If the function fails, the return value is (DWORD) -1. To get extended error information, call <see cref="GetLastError"/>.
        /// </returns>
        [DllImport(nameof(Kernel32), SetLastError = true)]
        public static extern int ResumeThread(SafeObjectHandle hThread);

        /// <summary>
        /// Waits until the specified object is in the signaled state or the time-out interval elapses.
        /// To enter an alertable wait state, use the WaitForSingleObjectEx function. To wait for multiple objects, use WaitForMultipleObjects.
        /// </summary>
        /// <param name="hHandle">
        /// A handle to the object. For a list of the object types whose handles can be specified, see the following Remarks section.
        /// If this handle is closed while the wait is still pending, the function's behavior is undefined.
        /// The handle must have the SYNCHRONIZE access right. For more information, see Standard Access Rights.
        /// </param>
        /// <param name="dwMilliseconds">
        /// The time-out interval, in milliseconds. If a nonzero value is specified, the function waits until the object is signaled or the interval elapses. If dwMilliseconds is zero, the function does not enter a wait state if the object is not signaled; it always returns immediately. If dwMilliseconds is INFINITE, the function will return only when the object is signaled.
        /// See MSDN docs for more information.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value indicates the event that caused the function to return. It can be one of the following values.
        /// </returns>
        [DllImport(nameof(Kernel32), SetLastError = true)]
        public static extern WaitForSingleObjectResult WaitForSingleObject(
            SafeHandle hHandle,
            int dwMilliseconds);

        /// <summary>
        /// Retrieves the termination status of the specified thread.
        /// </summary>
        /// <param name="hThread">
        /// A handle to the thread. The handle must have the THREAD_QUERY_INFORMATION or THREAD_QUERY_LIMITED_INFORMATION access right.
        /// Windows Server 2003 and Windows XP:  The handle must have the THREAD_QUERY_INFORMATION access right.
        /// </param>
        /// <param name="lpExitCode">A pointer to a variable to receive the thread termination status. For more information, see Remarks.</param>
        /// <returns>If the function is succeeds, the return value is true, else the return value is zero.  To get extended error information, call <see cref="GetLastError"/>.</returns>
        /// <remarks>
        /// <para>
        /// This function returns immediately. If the specified thread has not terminated and the function succeeds, the status returned is STILL_ACTIVE.
        /// If the thread has terminated and the function succeeds, the status returned is one of the following values:
        /// </para>
        /// <list>
        /// <item>The exit value specified in the <see cref="ExitThread"/> or <see cref="TerminateThread"/> function.</item>
        /// <item>The return value from the thread function.</item>
        /// <item>he exit value of the thread's process.</item>
        /// </list>
        /// <para>
        /// Important: The GetExitCodeThread function returns a valid error code defined by the application only after the thread terminates.
        /// Therefore, an application should not use STILL_ACTIVE (259) as an error code.
        /// If a thread returns STILL_ACTIVE (259) as an error code, applications that test for this value could interpret it to mean that the thread is still running
        /// and continue to test for the completion of the thread after the thread has terminated, which could put the application into an infinite loop.
        /// </para>
        /// </remarks>
        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeThread(IntPtr hThread, out int lpExitCode);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">A valid handle to an open object.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.To get extended error information, call <see cref="GetLastError"/>.
        /// </returns>
        [DllImport(nameof(Kernel32), SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
