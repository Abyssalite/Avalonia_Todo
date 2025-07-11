using Avalonia.Controls;
using System;
using Avalonia;

namespace App1.Views;

public partial class TaskDetailView : UserControl
{
    public TaskDetailView()
    {
        InitializeComponent();
        TaskDetailPanel.Margin = OperatingSystem.IsAndroid() ? new Thickness(10) : new Thickness(100, 20, 100, 20);
    }
}