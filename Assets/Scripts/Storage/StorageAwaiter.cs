using System.Threading.Tasks;

namespace Storage
{
    public static class StorageAwaiter
    {
        private static TaskCompletionSource<bool> tcs = new();

        public static Task WaitReadyAsync() => tcs.Task;

        public static void SignalReady()
        {
            if (!tcs.Task.IsCompleted)
                tcs.SetResult(true);
        }
    }
}