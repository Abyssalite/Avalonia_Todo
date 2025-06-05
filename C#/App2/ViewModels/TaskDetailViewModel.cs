using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App2.ViewModels;

public class TaskDetailViewModel : ViewModelBase
{
    public BaseTask Task { get; }

    public TaskDetailViewModel(BaseTask task)
    {
        Task = task;
    }
}
