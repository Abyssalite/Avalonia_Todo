namespace AppUno.Views;

public sealed partial class TaskGroupView : Page
{
    public TaskGroupView()
    {
        this.InitializeComponent();
        if (OperatingSystem.IsAndroid())
        {
            TaskGroupPanel.Margin = new Thickness(5);
        }
        else
        { 
            TaskGroupPanel.Margin = new Thickness(30, 20, 30, 20);
        }
    }
}

