using Avalonia.Controls;
using Avalonia.VisualTree;
using System;
using App2.ViewModels;

namespace App2.Views;

public partial class TaskDetailView : UserControl
{
    public TaskDetailView()
    {
        InitializeComponent();
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
                    if (confirmed) vm.DeleteTask();
                }
            };
        }
    }
}