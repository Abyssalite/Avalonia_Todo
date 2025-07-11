using System;
using App1.ViewModels;
using Avalonia.Controls;

namespace App1.Components;

public partial class TopBarView : UserControl
{
    public TopBarView()
    {
        InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is TopBarViewModel vm)
        {
            vm.OnSetParent = (parent) =>
            {
                switch (parent?.GetType().Name)
                {
                    case "AddTaskViewModel":
                        AddTaskTopBar(); break;

                    case "TaskDetailViewModel":
                        TaskDetailTopBar(); break;

                    case "TaskGroupViewModel":
                        TaskGroupTopBar(); break;

                    default:
                        break;
                }
            };
            vm.RunAfterLoadedCommand.Execute(null);
        }
    }

    private void AddTaskTopBar()
    {
        ButtonFlyout.IsVisible = false;
        BackOrDrawerButton.IsVisible = false;
        StarButton.IsVisible = false;
    }

    private void TaskDetailTopBar()
    {
        if (OperatingSystem.IsAndroid())
            ButtonFlyout.IsVisible = false;
        else  StarButton.IsVisible = false;
    }

    private void TaskGroupTopBar()
    {
        BackOrDrawerButton.IsVisible = false;
        StarButton.IsVisible = false;
    }
}
