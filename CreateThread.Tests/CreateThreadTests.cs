using System;
using System.Linq;
using Xunit;
using Xunit.Repeat;
using PInvoke;
using static PInvoke.Kernel32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CreateThread.Tests
{
    public class CreateThreadTests
    {
        [Theory]
        [Repeat(100)]
        public unsafe void CreateManyThreads(int iteration)
        {
            var gcHandle = GCHandle.Alloc(iteration);
            try
            {
                int dwNewThreadId = 0;
                using (var hThread =
                    Kernel32.CreateThread(
                        null,
                        SIZE_T.Zero,
                        new THREAD_START_ROUTINE(CreateThread_Test_ThreadMain),
                        GCHandle.ToIntPtr(gcHandle),    // Pass iteration to the ThreadProc
                        Kernel32.CreateProcessFlags.CREATE_SUSPENDED,
                        &dwNewThreadId))
                {

                    // Cannot use dwNewThreadId directly in the LINQ Query - CS1686; make a copy
                    int threadId = dwNewThreadId;
                    // Search the process for threads matching ID == dwThreadId
                    var thread = (from ProcessThread entry
                                  in Process.GetCurrentProcess().Threads
                                  where entry.Id == threadId
                                  select entry)
                                  .FirstOrDefault();
                    Assert.NotNull(thread);

                    var dwSuspendCount = Kernel32.ResumeThread(new SafeObjectHandle(hThread.DangerousGetHandle(), ownsHandle: false));
                    Assert.Equal(1, dwSuspendCount);

                    Kernel32.WaitForSingleObject(hThread, -1);

                    Kernel32.GetExitCodeThread(hThread.DangerousGetHandle(), out var dwExitCode);
                    Assert.Equal(1, dwExitCode);

                    // The ThredProc incremeted its data by 1. Validate that it worked. 
                    Assert.Equal(iteration + 1, (int)gcHandle.Target);
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        private static int CreateThread_Test_ThreadMain(IntPtr data)
        {
            // Iteration # was passed as data
            // Increase it by 1 
            var gcHandle = GCHandle.FromIntPtr(data);
            gcHandle.Target = (int)gcHandle.Target + 1;

            return 1;
        }
    }
}
