using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Threading;

namespace Calculator;

public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly IDisposable? _subscribe;
    private bool? _lastCanExecuteFromObservable;

    public RelayCommand(Action<object?> execute)
    {
        _execute = execute;
    }

    public RelayCommand(Action<object?> execute, IObservable<bool> canExecuteObservable)
    {
        _execute = execute;

        _subscribe = canExecuteObservable
            .Catch<bool, Exception>(ex => Observable.Return(false))
            .StartWith(false)
            .DistinctUntilChanged()
            .Replay(1)
            .RefCount()
            .Subscribe(x =>
            {
                _lastCanExecuteFromObservable = x;
                Dispatcher.UIThread.Invoke(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
            });
    }

    public bool CanExecute(object? parameter)
    {
        if (_lastCanExecuteFromObservable.HasValue)
            return _lastCanExecuteFromObservable.Value;
        return true;
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    public event EventHandler? CanExecuteChanged;

    ~RelayCommand()
    {
        _subscribe?.Dispose();
    }
}