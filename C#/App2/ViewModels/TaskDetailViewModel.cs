namespace App2.ViewModels;

public class TaskDetailViewModel : ViewModelBase
{
    public BaseTask Task { get; }

    public TaskDetailViewModel(BaseTask task)
    {
        Task = task;
    }
}
