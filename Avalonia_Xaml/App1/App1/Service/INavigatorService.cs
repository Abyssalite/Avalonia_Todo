using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App1.ViewModels;

public class NavigatorService : INavigatorService
{
    private readonly IViewHost _host;
    private bool isExit = false;
    private readonly Stack<ViewModelBase> _leftHistory = new();
    private readonly Stack<ViewModelBase> _rightHistory = new();
    public ViewModelBase? FirstView { set; get; }
    private ViewModelBase? _currentLeft;
    private ViewModelBase? _currentRight;

    public NavigatorService(IViewHost host)
    {
        _host = host;
    }

    public async Task NavigateLeft(ViewModelBase? viewModel)
    {
        isExit = false;
        if (_currentLeft != null)
            _leftHistory.Push(_currentLeft);
        if (viewModel != null)
            _currentLeft = viewModel;
        
        TaskHelpers.print("_leftHistory");
        TaskHelpers.print(_leftHistory);
        await _host.NavigateLeft(_currentLeft);
    }

    public async Task NavigateRight(ViewModelBase? viewModel)
    {
        isExit = false;
        if (_currentRight != null)
            _rightHistory.Push(_currentRight);
        if (viewModel != null)
            _currentRight = viewModel;
    
        TaskHelpers.print("_rightHistory");
        TaskHelpers.print(_rightHistory);
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
        if (_rightHistory.Count > 0 && FirstView != null)
        {
            _currentRight = FirstView;
            _rightHistory.Clear();
        }
    }

    public async Task OpenPrevious()
    {
        if (_rightHistory.Count > 0)
        {
            _currentRight = _rightHistory.Pop();
            TaskHelpers.print("_rightHistory");
            TaskHelpers.print(_rightHistory);
            await _host.NavigateRight(_currentRight);
        }
        if (_leftHistory.Count > 0)
        {
            _currentLeft = _leftHistory.Pop();
            TaskHelpers.print("_leftHistory");
            TaskHelpers.print(_leftHistory);
            await _host.NavigateLeft(_currentLeft);
        }
    }
}

public interface INavigatorService
{
    Task NavigateLeft(ViewModelBase? viewModel);
    Task NavigateRight(ViewModelBase? viewModel);
    public void ClearStack();
    bool IsExit(); 
    ViewModelBase? FirstView { set; get; }
    Task OpenPrevious();   
}
