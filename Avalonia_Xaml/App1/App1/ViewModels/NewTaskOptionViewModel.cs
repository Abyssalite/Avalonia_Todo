namespace App1.ViewModels;

public partial class NewTaskOptionViewModel : ViewModelBase
{
    private readonly Store _store;
    private readonly IViewHost _host;

    public NewTaskOptionViewModel(IViewHost host, Store store)
    {
        _store = store;
        _host = host;
    }
}