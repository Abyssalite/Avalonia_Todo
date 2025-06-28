using Avalonia.Controls;
using System;
using Avalonia;

namespace App1.Views;

public partial class AddTaskView : UserControl
{
    public AddTaskView()
    {
        InitializeComponent();
        AddTaskPanel.Margin = OperatingSystem.IsAndroid() ? new Thickness(10) : new Thickness(100, 30, 100, 20);
    }
}