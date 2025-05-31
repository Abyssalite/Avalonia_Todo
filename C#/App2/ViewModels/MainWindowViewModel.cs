using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
namespace App2.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<BaseTask> Tasks { get; } = new();

    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }
    public ICommand AddNoteCommand { get; }
    public ICommand ClearCommand { get; }



    private string InputOrDefault(string? input, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }
    public MainWindowViewModel()
    {
        AddNoteCommand = new RelayCommand(AddNote);
        ClearCommand = new RelayCommand(ClearAllInput);

    }

    private void ClearAllInput()
    {
        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));
    }

    private void AddNote()
    {
        string name = InputOrDefault(NewTaskName, "");
        if (name != "")
            Tasks.Add(new BaseTask
            {
                Name = name,
                IsDone = false,
                Category = InputOrDefault(TaskCatalog, ""),
                Description = InputOrDefault(TaskDesc, "")
            });

        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));

    }
}
