using Avalonia.Controls;
using Avalonia;

namespace App1.Views;

public partial class AddTaskView : UserControl
{
    public AddTaskView()
    {
        InitializeComponent();
        AddTaskPanel.Margin = GlobalVariables.IsAndroid ? new Thickness(10) : new Thickness(100, 20, 100, 20);
    }
}