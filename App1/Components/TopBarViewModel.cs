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
    
    private string _topbarText;
    public string TopbarText
    {
        get => _topbarText;
        set
        {
            if (value == null) return;

            _topbarText = value;
            _store.EditTopBarText(_topbarText);
            OnPropertyChanged(nameof(TopbarText));                
        }
    }
    private bool? _toggleImportant;
    public bool? ToggleImportant
    {
        get => _toggleImportant;
        set
        {
            _toggleImportant = value;
            _store.SetTaskImportant(_toggleImportant);
            OnPropertyChanged(nameof(ToggleImportant));                
        }
    }
    public bool IsNotInArchive { get; set; } = true;
    public bool CanEditBarName { get; set; } = false;
    public bool IsInEditMode { get; set; } = false;

    public ICommand? SetArchiveCommand { get; }
    public ICommand? EditCommand { get; }
    public ICommand? BackOrDrawerCommand { get; }
    public ICommand? DeleteCommand { get; }
    public RelayCommand RunAfterLoadedCommand { get; }
    public Action<ViewModelBase>? OnSetParent { get; set; }

    public TopBarViewModel(Store store, ViewModelBase? parent, IEventHub events, string? text = null)
    {
        if (parent == null) throw new NullReferenceException("No Parent View Setted");

        _store = store;
        _parent = parent;
        _events = events;
        _topbarText = text ?? "";

        if (_store.SelectedTask != null)
            _toggleImportant = _store.SelectedTask.IsImportant;

        if (_store.SelectedList != null)
        {
            IsNotInArchive = !_store.SelectedList.IsArchived;
            _subscriptions.Add(_events.Subscribe<GroupListIsArchiveStateChangedEvent>(evt =>
            {
                if (evt.List.ListName != _store.SelectedList.ListName) return;

                IsNotInArchive = !evt.IsArchived;
                OnPropertyChanged(nameof(IsNotInArchive));
            }));
        }


        _subscriptions.Add(_events.Subscribe<ChangeListNameEvent>(evt =>
        {
            IsInEditMode = evt.value;
            CanEditBarName = IsInEditMode && !TaskHelpers.IsQuickList(evt.name);
            OnPropertyChanged(nameof(CanEditBarName));
            OnPropertyChanged(nameof(IsInEditMode));
        }));
        

        SetArchiveCommand = new RelayCommand<object>((param) =>
        {
            if (param is Button button)
                button.Flyout?.Hide();
            _parent.SetArchiveCommand.Execute(null);
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

        RunAfterLoadedCommand = new RelayCommand(() => OnSetParent?.Invoke(_parent));
    }
    
        public void Dispose()
    {
        foreach (var sub in _subscriptions)
            sub.Dispose();
    }
}
