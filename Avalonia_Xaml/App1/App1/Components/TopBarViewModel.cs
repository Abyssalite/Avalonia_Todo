using System;
using System.Windows.Input;
using App1.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App1.Components;

public partial class TopBarViewModel : ObservableObject
{
    private readonly Store _store;
    public string? BarName { get; set; }
    public bool IsNotMainList { get; } = true;
    public bool IsNotInArchive { get; set; } = true;
    public bool CanEditBarName { get; set; } = false;
    private bool _isInEditMode = false;
    public bool IsInEditMode
    {
        get => _isInEditMode;
        set
        {
            _isInEditMode = value;
            CanEditBarName = !CheckMainList() && _isInEditMode;
            OnPropertyChanged(nameof(IsInEditMode));
            OnPropertyChanged(nameof(CanEditBarName));
        }
    }
    public ICommand? ArchiveCommand { get; }
    public ICommand? RestoreOrEditCommand { get; }
    public ICommand? BackOrDrawerCommand { get; }
    public ICommand? DeleteCommand { get; }
    public RelayCommand RunAfterLoadedCommand { get; }
    public Action<ViewModelBase?>? OnSetParent { get; set; }


    public TopBarViewModel(Store store, ViewModelBase? parent, String? barname = null)
    {
        _store = store;

        if (store.SelectedList != null)
        {
            if (barname != null) BarName = barname;
            else BarName = store.SelectedListName;
            IsNotMainList = !CheckMainList();
            IsNotInArchive = !store.SelectedList.IsArchived;
            OnPropertyChanged(nameof(IsNotInArchive));
            OnPropertyChanged(nameof(IsNotMainList));
        }

        _store.PropertyChanged += (_, e) =>
        {
            if (store.SelectedList == null) return;

            if (e.PropertyName == nameof(GroupList.IsArchived))
            {
                IsNotInArchive = !store.SelectedList.IsArchived;
                OnPropertyChanged(nameof(IsNotInArchive));
            }
        };

        if (parent is TaskGroupViewModel tg) {
            ArchiveCommand = new AsyncRelayCommand(() => tg.ArchiveCommand.ExecuteAsync(null));
            RestoreOrEditCommand = new AsyncRelayCommand(() => tg.RestoreOrEditCommand.ExecuteAsync(null));
            DeleteCommand = new AsyncRelayCommand(() => tg.DeleteCommand.ExecuteAsync(null));
        }

        RunAfterLoadedCommand = new RelayCommand(() => OnSetParent?.Invoke(parent));
    }

    private bool CheckMainList() => BarName == "Quick" || BarName == "Important";
}
