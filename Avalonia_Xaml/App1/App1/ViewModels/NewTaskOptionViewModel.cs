using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class NewTaskOptionViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;

    public NewTaskOptionViewModel(IViewHost host, Store store)
    {
        _store = store;
        _host = host;
    }
}