namespace App1.ViewModels;

public class WellcomeViewModel : ViewModelBase
{
    private readonly MainViewModel _mainWViewModel;
    public string Text { get; set; }
    public WellcomeViewModel(MainViewModel main, string text)
    {
        _mainWViewModel = main;
        Text = text;
    }
}
