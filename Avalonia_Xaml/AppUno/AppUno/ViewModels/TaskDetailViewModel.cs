using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public class TaskDetailViewModel: ViewModelBase
{
    private Store _store;
    public TaskDetailViewModel(Store store)
    {
        _store = store;
    }
}
