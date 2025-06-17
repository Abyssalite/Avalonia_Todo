namespace App1.ViewModels;

public class WellcomeViewModel : ViewModelBase
{
    public string Text { get; set; }
    public WellcomeViewModel(string text)
    {
        Text = text;
    }
}
