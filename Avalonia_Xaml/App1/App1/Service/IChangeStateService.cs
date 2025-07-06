using System;

public class ChangeStateService : IChangeStateService
{
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
                EditModeChanged?.Invoke(this, value);
            }
        }
    }
    public event EventHandler<bool>? EditModeChanged;

    public event Action? SelectedListCleared;
    public void ClearSelectedList()
    {
        SelectedListCleared?.Invoke();
    }
}

public interface IChangeStateService
{
    void OpenPane(bool isOpen);

    bool IsInEditMode { get; set; }
    event EventHandler<bool>? EditModeChanged;

    void ClearSelectedList();
}
