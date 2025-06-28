using System;

public class PaneService : IPaneService
{
    public event Action<bool>? PaneChanged;

    public void OpenPane(bool isOpen)
    {
        PaneChanged?.Invoke(isOpen);
    }
}

public interface IPaneService
{
    void OpenPane(bool isOpen);
}
