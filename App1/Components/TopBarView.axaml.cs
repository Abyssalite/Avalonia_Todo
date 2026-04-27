using System;
using App1.ViewModels;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace App1.Components;

public partial class TopBarView : UserControl
{
    public TopBarView()
    {
        InitializeComponent();
        
        this.DataContextChanged += OnDataContextChanged;

        if (GlobalVariables.IsAndroid)
            BackOrDrawerImage.Source = new Bitmap(
                AssetLoader.Open(new Uri("avares://App1/Assets/icons8-menu-100.png"))
            );
        else BackOrDrawerImage.Source = new Bitmap(
                AssetLoader.Open(new Uri("avares://App1/Assets/icons8-back-100.png"))
            );
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is TopBarViewModel vm)
        {
            vm.OnParentViewModelSet = (parent) =>
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
                        DiableTopBar(); break;
                }
            };
            vm.RunAfterViewLoadedCommand.Execute(null);
        }
    }

    private void AddTaskTopBar()
    {
        BackOrDrawerButton.IsVisible = false;
        ButtonFlyout.IsVisible = false;
    }

    private void TaskDetailTopBar()
    {
        ButtonFlyout.IsVisible = false;
        ToggleArchiveButton.IsVisible = false;
    }

    private void TaskGroupTopBar(string listName)
    {
        BackOrDrawerButton.IsVisible = true;
        if (listName == GlobalVariables.Important) 
            ButtonFlyout.IsVisible = false;
                            
        ToggleArchiveButton.IsEnabled = !TaskHelpers.IsQuickList(listName);
        DeleteButton.IsEnabled = !TaskHelpers.IsQuickList(listName);
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

    private void DiableTopBar()
    {
        BackOrDrawerButton.IsVisible = false;
        StarButton.IsVisible = false;
        ButtonFlyout.IsVisible = false;
        DeleteButton.IsVisible = false;
        ToggleArchiveButton.IsVisible = false;
    }
}
