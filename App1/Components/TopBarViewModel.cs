using System;
using System.Windows.Input;
using App1.ViewModels;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia_EventHub;
using System.Collections.Generic;
using App1.Events;

namespace App1.Components;
/// <summary>
/// Todo: Maybe implement a more permanent Topbar
/// </summary>
public partial class TopBarViewModel : ObservableObject, IDisposable
{
    private readonly Store _store;
    private readonly ViewModelBase _parent;
    private readonly IEventHub _events;
    private readonly List<IDisposable> _subscriptions = new();
    
    private string _topbarText = "";
    public string TopbarText
    {
        get => _topbarText;
        set
        {
            if (value != null)
            {
                _topbarText = value;
                _store.TopbarText = _topbarText;
                OnPropertyChanged(nameof(TopbarText));                
            }
        }
    }
    private bool? _toggleImportant;
    public bool? ToggleImportant
    {
        get => _toggleImportant;
        set
        {
            if (value != null)
            {
                _toggleImportant = value;
                _parent.GetSetImportant(value);
                OnPropertyChanged(nameof(ToggleImportant));                
            }
        }
    }
    public bool IsNotInArchive { get; set; } = true;
    public bool CanEditBarName { get; set; } = false;

    public ICommand? ToggleArchiveCommand { get; }
    public ICommand? EditCommand { get; }
    public ICommand? BackOrDrawerCommand { get; }
    public ICommand? DeleteCommand { get; }
    public RelayCommand RunAfterLoadedCommand { get; }
    public Action<ViewModelBase>? OnSetParent { get; set; }

    public TopBarViewModel(Store store, ViewModelBase? parent, string text, IEventHub events)
    {
        if (parent == null) throw new NullReferenceException("No Parent View Setted");
        _store = store;
        _parent = parent;
        _events = events;

        if (parent is GroupListViewModel)
            TopbarText = text;
        else
            _topbarText = text;


        if (store.SelectedList != null)
        {
            IsNotInArchive = !store.SelectedList.IsArchived;
            OnPropertyChanged(nameof(IsNotInArchive));
        }

        _subscriptions.Add(_events.Subscribe<ListArchiveStateChangedEvent>(evt =>
        {
            if (_store.SelectedList == null) return;
            if (evt.List.ListName != _store.SelectedList.ListName) return;

            IsNotInArchive = !evt.IsArchived;
            OnPropertyChanged(nameof(IsNotInArchive));
        }));

        _subscriptions.Add(_events.Subscribe<TopbarTextChangedEvent>(evt =>
        {
            _topbarText = evt.Text;
            OnPropertyChanged(nameof(TopbarText));
        }));

        ToggleArchiveCommand = new AsyncRelayCommand<object>(async (param) =>
        {
            if (param is Button button)
                button.Flyout?.Hide();
            await _parent.ToggleArchiveCommand.ExecuteAsync(null);
        });
        DeleteCommand = new AsyncRelayCommand<object>(async (param) => {
            if (param is Button button)
                button.Flyout?.Hide();
            await _parent.DeleteCommand.ExecuteAsync(null);
        });
        EditCommand = new RelayCommand<object>((param) =>
        {
            if (param is Button button)
                button.Flyout?.Hide();
            _parent.EditCommand.Execute(null);

        });
        BackOrDrawerCommand = new AsyncRelayCommand<object>(async (param) => {
            if (param is Button button)
                button.Flyout?.Hide();
            await _parent.BackOrDrawerCommand.ExecuteAsync(null);
        });

        _parent.OnChangeListName = (value) =>
        {
            CanEditBarName = value;
            OnPropertyChanged(nameof(CanEditBarName));
        };
        _toggleImportant = _parent.GetSetImportant(null);
        OnPropertyChanged(nameof(ToggleImportant));
        RunAfterLoadedCommand = new RelayCommand(() => OnSetParent?.Invoke(_parent));
    }
    
        public void Dispose()
    {
        foreach (var sub in _subscriptions)
            sub.Dispose();
    }
}
