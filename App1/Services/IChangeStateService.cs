using System;

public class ChangeStateService : IChangeStateService
{
    public bool IsPaneOpen { set; get; }
    public event Action<bool>? PaneChanged;
    public void OpenPane(bool isOpen)
    {
        PaneChanged?.Invoke(isOpen);
    }

    private bool _isInEditMode;
    public bool IsInEditMode
    {
        get => _isInEditMode;
        set
        {
            if (_isInEditMode != value)
            {
                _isInEditMode = value;
                //EditModeChanged?.Invoke(value);
            }
        }
    }
    public event Action? EditModeChanged;
    public void CancelEdit()
    {
        EditModeChanged?.Invoke();
    }

    public event Action? SelectedListCleared;
    public void ClearSelectedList()
    {
        SelectedListCleared?.Invoke();
    }

    public event Action? ImportantChanged;
    public void UpdateImportant()
    {
        ImportantChanged?.Invoke();
    }
}

public interface IChangeStateService
{
    bool IsPaneOpen { set; get; }
    event Action<bool>? PaneChanged;
    void OpenPane(bool isOpen);

    bool IsInEditMode { get; set; }
    event Action? EditModeChanged;
    void CancelEdit();

    event Action? SelectedListCleared;
    void ClearSelectedList();

    event Action? ImportantChanged;
    void UpdateImportant();
}
