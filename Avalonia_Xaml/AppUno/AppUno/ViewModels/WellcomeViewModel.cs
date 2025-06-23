namespace AppUno.ViewModels;

public class WellcomeViewModel : ViewModelBase
{
    public string Text { get; }
    
    public WellcomeViewModel(string text)
    {
        Text = text;
    }
}
