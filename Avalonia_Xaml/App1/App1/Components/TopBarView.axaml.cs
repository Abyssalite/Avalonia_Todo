using System;
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
                switch (parent.GetType().Name)
                {
                    case "AddTaskViewModel":
                        AddTaskTopBar(); break;

                    case "TaskDetailViewModel":
                        TaskDetailTopBar(); break;

                    case "TaskGroupViewModel":
                        TaskGroupTopBar(); break;

                    case "WellcomeViewModel":
                        WellcomeTopBar(); break;

                    default:
                        break;
                }
            };
            vm.RunAfterLoadedCommand.Execute(null);
        }
    }

    private void AddTaskTopBar()
    {
        BackOrDrawerButton.IsVisible = false;
        StarButton.IsVisible = false;
        ButtonFlyout.IsVisible = false;
    }

    private void TaskDetailTopBar()
    {
        if (OperatingSystem.IsAndroid())
            ButtonFlyout.IsVisible = false;
        ToggleArchiveButton.IsVisible = false;
    }

    private void TaskGroupTopBar()
    {
        if (OperatingSystem.IsAndroid())
            BackOrDrawerButton.IsVisible = true;
        else BackOrDrawerButton.IsVisible = false;
        StarButton.IsVisible = false;
    }
    
    private void WellcomeTopBar()
    {
        if (OperatingSystem.IsAndroid())
            BackOrDrawerButton.IsVisible = true;
        else BackOrDrawerButton.IsVisible = false;
        StarButton.IsVisible = false;
        ButtonFlyout.IsVisible = false;
    }
}
