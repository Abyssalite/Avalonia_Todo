using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;

namespace App1.Components;

public partial class CustomDialogViewModel : ObservableObject, IDialogContext
{
    [ObservableProperty] private string? _owner;
    [ObservableProperty] private string? _target;
    [ObservableProperty] private string _text;
    public event EventHandler<object?>? RequestClose;
    public Action<bool?>? OnClose { get; set; }
    public ICommand OKCommand { get; set; }
    public ICommand CancelCommand { get; set; }

    public CustomDialogViewModel(string message)
    {
        _text = message;
        OKCommand = new RelayCommand(OK);
        CancelCommand = new RelayCommand(Cancel);
    }

    public void Close()
    {
        OnClose?.Invoke(null);
        RequestClose?.Invoke(this, null);
    }

    private void OK()
    {
        OnClose?.Invoke(true);
        RequestClose?.Invoke(this, true);
    }

    private void Cancel()
    {
        OnClose?.Invoke(false);
        RequestClose?.Invoke(this, false);
    }
}