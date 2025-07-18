using System;
using System.Windows.Input;
using App1.ViewModels;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App1.Components;
/// <summary>
/// Todo: Maybe implement a more permanent Topbar
/// </summary>
public partial class TopBarViewModel : ObservableObject
{
    private readonly Store _store;
    private readonly ViewModelBase _parent;
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

    public TopBarViewModel(Store store, ViewModelBase? parent, string text)
    {
        if (parent == null) throw new NullReferenceException("No Parent View Setted");
        _store = store;
        _parent = parent;
        if (parent is GroupListViewModel)
            TopbarText = text;
        else _topbarText = text;

        if (store.SelectedList != null)
        {
            IsNotInArchive = !store.SelectedList.IsArchived;
            OnPropertyChanged(nameof(IsNotInArchive));
        }

        _store.PropertyChanged += (_, e) =>
        {
            if (store.SelectedList == null) return;

            // Update GUI after toggle IsArchived
            if (e.PropertyName == nameof(GroupList.IsArchived))
            {
                IsNotInArchive = !store.SelectedList.IsArchived;
                OnPropertyChanged(nameof(IsNotInArchive));
            }
            // Update GUI after change TopbarText
            if (e.PropertyName == nameof(Store.TopbarText))
            {
                _topbarText = _store.TopbarText;
                OnPropertyChanged(nameof(TopbarText));
            }
        };

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
}
