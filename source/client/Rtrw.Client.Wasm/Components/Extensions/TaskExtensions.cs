namespace Rtrw.Client.Wasm.Components.Extensions
{
    public enum TaskOption
    {
        None,
        Safe
    }

    public static class TaskExtensions
    {
        public static async void AndForget(this Task task)
        {
            await task;
        }
        public static async void AndForget(this Task task, TaskOption option)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (option != TaskOption.Safe)
                    throw;

                Console.WriteLine(ex);
            }
        }
        public static async void AndForget(this ValueTask task)
        {
            await task;
        }
        public static async void AndForget(this ValueTask task, TaskOption option)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (option != TaskOption.Safe)
                    throw;

                Console.WriteLine(ex);
            }
        }
    }
}
