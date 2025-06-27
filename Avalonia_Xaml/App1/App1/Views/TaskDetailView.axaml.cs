using Avalonia.Controls;
using System;
using Avalonia;

namespace App1.Views;

public partial class TaskDetailView : UserControl
{
    public TaskDetailView()
    {
        InitializeComponent();
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