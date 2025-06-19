using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class NewTaskOptionViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly GroupListViewModel _groupListViewModel;

    public NewTaskOptionViewModel(MainViewModel main, GroupListViewModel groupList)
    {
        _mainViewModel = main;
        _groupListViewModel = groupList;
    }
}