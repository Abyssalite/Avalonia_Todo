using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public partial class MainViewModel : ViewModelBase
{
    public INavigator _navigator;
    private Store _store = new();

    public MainViewModel(INavigator nav)
    {
        _store.GroupedList = TaskHelpers.Load();
        _navigator = nav;
        InitializeNavigation();

    }
    private async void InitializeNavigation()
    {
        try
        {
            await _navigator.NavigateRouteAsync(this, "/Right/WellcomeView", data: "Wellcome");
            await _navigator.NavigateRouteAsync(this, "/Left/GroupListView", data: _store);
        }
        catch (Exception ex)
        {
                Console.WriteLine($"An error occurred: {ex.Message}");

        }

    }
}
