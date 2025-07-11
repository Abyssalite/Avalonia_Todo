namespace App1.ViewModels;

public partial class NewTaskOptionViewModel : ViewModelBase
{
    private readonly Store _store;

    public NewTaskOptionViewModel(Store store)
    {
        _store = store;
    }
}