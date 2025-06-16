using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System;
using App2.Views;

namespace App2.ViewModels;

public class WellcomeViewModel : ViewModelBase
{
    private readonly MainViewModel _mainWViewModel;
    public string Text { get; set; }
    public WellcomeViewModel(MainViewModel main, string text)
    {
        _mainWViewModel = main;
        Text = text;
    }
}
