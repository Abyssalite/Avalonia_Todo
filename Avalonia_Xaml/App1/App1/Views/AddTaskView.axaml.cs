using Avalonia.Controls;
using System;
using Avalonia;

namespace App1.Views;

public partial class AddTaskView : UserControl
{
    public AddTaskView()
    {
        InitializeComponent();
        if (OperatingSystem.IsAndroid())
        {
            AddTaskPanel.Margin = new Thickness(10);
        }
        else
        { 
            AddTaskPanel.Margin = new Thickness(60, 20, 60, 20);
        }
    }
    
}