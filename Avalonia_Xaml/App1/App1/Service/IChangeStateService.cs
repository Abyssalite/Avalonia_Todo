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
                EditModeChanged?.Invoke(value);
            }
        }
    }
    public event Action<bool>? EditModeChanged;

    public event Action? SelectedListCleared;
    public void ClearSelectedList()
    {
        SelectedListCleared?.Invoke();
    }
}

public interface IChangeStateService
{
    void OpenPane(bool isOpen);
    bool IsPaneOpen { set; get; }
    event Action<bool>? PaneChanged;

    bool IsInEditMode { get; set; }
    event Action<bool>? EditModeChanged;

    void ClearSelectedList();
}
