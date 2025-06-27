namespace App1.ViewModels;

public partial class NewTaskOptionViewModel : ViewModelBase
{
    private Store _store;
    private IViewHost _host;

    public NewTaskOptionViewModel(IViewHost host, Store store)
    {
        _store = store;
        _host = host;
    }
}