using Avalonia.Controls;
using Avalonia.VisualTree;
using System;
using App2.ViewModels;

namespace App2.Views;

public partial class AddTaskView : UserControl
{
    public AddTaskView()
    {
        InitializeComponent();

        this.DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is AddTaskViewModel vm)
        {
            vm.ShowEmptyNameDialog = async () =>
            {
                var window = this.GetVisualRoot() as Window;

                if (window is not null)
                {
                    var dialog = new CustomDialog();
                    await dialog.ShowDialog(window);
                }
            };
        }
    }
}