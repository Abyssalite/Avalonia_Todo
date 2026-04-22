using System;

public class ChangeStateService : IChangeStateService
{
    public bool IsPaneOpen { set; get; }
    public event Action<bool>? OpenPaneAction;
    public void OpenPane(bool isOpen)
    {
        OpenPaneAction?.Invoke(isOpen);
    }

    public event Action? CancelEditAction;
    public void CancelEdit()
    {
        CancelEditAction?.Invoke();
    }

    public event Action? ClearSelectedListAction;
    public void ClearSelectedList()
    {
        ClearSelectedListAction?.Invoke();
    }

    public event Action? UpdateImportantAction;
    public void UpdateImportant()
    {
        UpdateImportantAction?.Invoke();
    }
}

public interface IChangeStateService
{
    bool IsPaneOpen { set; get; }
    event Action<bool>? OpenPaneAction;
    void OpenPane(bool isOpen);

    event Action? CancelEditAction;
    void CancelEdit();

    event Action? ClearSelectedListAction;
    void ClearSelectedList();

    event Action? UpdateImportantAction;
    void UpdateImportant();
}
