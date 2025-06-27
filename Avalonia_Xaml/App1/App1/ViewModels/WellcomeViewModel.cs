namespace App1.ViewModels;

public partial class WellcomeViewModel : ViewModelBase
{
    public string Text { get; }
    public WellcomeViewModel(Store store)
    {
        Text = store.WellcomeText;
    }
}
