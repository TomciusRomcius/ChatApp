using System.Collections.Concurrent;
using System.Data;

namespace ChatApp.Server.Application.Services
{
    public interface IBackgroundTaskQueue
    {
        Func<Task>? Dequeue();
        void Enqueue(Func<Task> task);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        readonly ConcurrentQueue<Func<Task>> _queue = new ConcurrentQueue<Func<Task>>();

        public void Enqueue(Func<Task> task)
        {
            _queue.Enqueue(task);
        }

        public Func<Task>? Dequeue()
        {
            Func<Task>? result;
            _queue.TryDequeue(out result);

            return result;
        }
    }
}