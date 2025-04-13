using System.Collections.Concurrent;

namespace ChatApp.Application.Services;

public interface IBackgroundTaskQueue
{
    Func<Task>? Dequeue();
    void Enqueue(Func<Task> task);
}

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly ConcurrentQueue<Func<Task>> _queue = new();

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