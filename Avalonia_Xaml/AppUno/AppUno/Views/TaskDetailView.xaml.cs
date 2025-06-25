namespace AppUno.Views;

public sealed partial class TaskDetailView : Page
{
    public TaskDetailView()
    {
        this.InitializeComponent();
        if (OperatingSystem.IsAndroid())
        {
            TaskDetailPanel.Margin = new Thickness(10);
        }
        else
        { 
            TaskDetailPanel.Margin = new Thickness(60, 20, 60, 20);
        }
    }
}
