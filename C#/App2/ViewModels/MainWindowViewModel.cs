using System;
using App2.Views;

namespace App2.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private object _mainViewModel;
    public object MainView
    {
        get => _mainViewModel;
        set => SetProperty(ref _mainViewModel, value);
    }

    public MainWindowViewModel()
    {
        Console.WriteLine("MainWindowViewModel");
        _mainViewModel = new MainViewModel(this);
        
    }
    
}