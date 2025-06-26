namespace App1.ViewModels;

public partial class NewTaskOptionViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;

    public NewTaskOptionViewModel(IViewHost host, Store store)
    {
        _store = store;
        _host = host;
    }
}