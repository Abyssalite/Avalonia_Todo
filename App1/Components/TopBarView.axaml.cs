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
                switch (parent.GetType().Name)
                {
                    case "AddTaskViewModel":
                        AddTaskTopBar(); break;

                    case "TaskDetailViewModel":
                        TaskDetailTopBar(); break;

                    case "TaskGroupViewModel":
                        {
                            if (parent is TaskGroupViewModel vm)
                                TaskGroupTopBar(vm.ListName); break;
                        }

                    case "WelcomeViewModel":
                        WelcomeTopBar(); break;

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
        ButtonFlyout.IsVisible = false;
    }

    private void TaskDetailTopBar()
    {
        if (GlobalVariables.IsAndroid)
            ButtonFlyout.IsVisible = false;
        ToggleArchiveButton.IsVisible = false;
    }

    private void TaskGroupTopBar(string listName)
    {
        if (GlobalVariables.IsAndroid)
            BackOrDrawerButton.IsVisible = true;
        else BackOrDrawerButton.IsVisible = false;
        if (listName == GlobalVariables.Important) 
            ButtonFlyout.IsVisible = false;
                            
        ToggleArchiveButton.IsEnabled = !TaskHelpers.IsMainList(listName);
        DeleteButton.IsEnabled = !TaskHelpers.IsMainList(listName);
        StarButton.IsVisible = false;
    }
    
    private void WelcomeTopBar()
    {
        if (GlobalVariables.IsAndroid)
            BackOrDrawerButton.IsVisible = true;
        else BackOrDrawerButton.IsVisible = false;
        StarButton.IsVisible = false;
        ButtonFlyout.IsVisible = false;
    }
}
