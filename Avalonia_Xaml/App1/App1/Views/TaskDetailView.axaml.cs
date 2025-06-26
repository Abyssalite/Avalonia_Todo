using Avalonia.Controls;
using Avalonia.VisualTree;
using System;
using App1.ViewModels;
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
        this.DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is TaskDetailViewModel vm)
        {
            vm.ShowDeleteDialog = async () =>
            {
                var window = this.GetVisualRoot() as Window;

                if (window is not null)
                {
                    var dialog = new CustomDialog("Do you want to Delete!", true, "Delete");
                    bool confirmed = await dialog.ShowDialog<bool>(window);
                    if (confirmed) await vm.DeleteTaskAsync();
                }
            };
        }
    }
}