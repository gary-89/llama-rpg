using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.Toolkit.Async;

internal sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
{
    public NotifyTaskCompletion(Task<TResult> task)
    {
        CompletionTask = task;

        if (!task.IsCompleted)
        {
            var _ = WatchTaskAsync(CompletionTask);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Task<TResult> CompletionTask { get; }

    public TResult? Result => CompletionTask.Status == TaskStatus.RanToCompletion ? CompletionTask.Result : default;

    public TaskStatus? Status => CompletionTask.Status;

    public bool? IsCompleted => CompletionTask.IsCompleted;

    public bool? IsNotCompleted => !CompletionTask.IsCompleted;

    public bool? IsSuccessfullyCompleted => CompletionTask.Status == TaskStatus.RanToCompletion;

    public bool? IsCanceled => CompletionTask.IsCanceled;

    public bool? IsFaulted => CompletionTask.IsFaulted;

    public AggregateException? Exception => CompletionTask.Exception;

    public Exception? InnerException => Exception?.InnerException;

    public string? ErrorMessage => InnerException?.Message;

    private async Task WatchTaskAsync(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            // ignored
        }

        var propertyChanged = PropertyChanged;

        if (propertyChanged == null)
        {
            return;
        }

        propertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
        propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
        propertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));

        if (task.IsCanceled)
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
        }
        else if (task.IsFaulted)
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(InnerException)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
        else
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));
        }
    }
}
