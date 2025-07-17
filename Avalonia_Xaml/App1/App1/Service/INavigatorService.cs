using System.Collections.Generic;
using System.Threading.Tasks;
using App1.Components;
using App1.ViewModels;

public class NavigatorService : INavigatorService
{
    private readonly IViewHost _host;
    private bool isExit = false;
    private readonly Stack<ViewModelBase> _leftHistory = new();
    private readonly Stack<(ViewModelBase, TopBarViewModel)> _rightHistory = new();
    private readonly IChangeStateService _stateService;
    public (ViewModelBase?, TopBarViewModel?) FirstView { set; get; }
    private ViewModelBase? _currentLeft;
    private ViewModelBase? _currentRight;
    private TopBarViewModel? _currentTopBar;

    public NavigatorService(IViewHost host, IChangeStateService stateService)
    {
        _host = host;
        _stateService = stateService;
    }

    public async Task NavigateLeft(ViewModelBase? viewModel)
    {
        isExit = false;
        if (_currentLeft != null)
            _leftHistory.Push(_currentLeft);
        if (viewModel != null)
            _currentLeft = viewModel;
        
        await _host.NavigateLeft(_currentLeft);
    }

    public async Task NavigateRight((ViewModelBase?, TopBarViewModel?) viewModel)
    {
        isExit = false;
        if (_currentRight != null && _currentTopBar != null)
            _rightHistory.Push((_currentRight, _currentTopBar));
        if (viewModel != (null, null))
        {
            (_currentRight, _currentTopBar) = viewModel;
        }
        await _host.ChangeTopBar(_currentTopBar);
        await _host.NavigateRight(_currentRight);
    }

    public bool IsExit()
    {
        if (isExit)
            return true;
        else
        {
            if (_rightHistory.Count == 0) isExit = true;
            return false;
        }
    }

    public void ClearStack()
    {
        if (_rightHistory.Count > 0 && FirstView != (null, null))
        {
            (_currentRight, _currentTopBar) = FirstView;
            _rightHistory.Clear();
        }
    }

    public async Task OpenPrevious()
    {
        if (_stateService.IsInEditMode)
        {
            _stateService.CancelEdit();
            return;
        }
        if (_rightHistory.Count > 0)
        {
            (_currentRight, _currentTopBar) = _rightHistory.Pop();

            await _host.NavigateRight(_currentRight);
            await _host.ChangeTopBar(_currentTopBar);

            if (_rightHistory.Count == 0) _stateService.ClearSelectedList();
    
        }
        if (_leftHistory.Count > 0)
        {
            _currentLeft = _leftHistory.Pop();

            await _host.NavigateLeft(_currentLeft);
        }
    }
}

public interface INavigatorService
{
    Task NavigateLeft(ViewModelBase? viewModel);
    Task NavigateRight((ViewModelBase?, TopBarViewModel?) viewModel);
    void ClearStack();
    bool IsExit();
    (ViewModelBase?, TopBarViewModel?) FirstView { set; get; }
    Task OpenPrevious();
}
