namespace newApp.ViewModels;
using Avalonia.Interactivity;
using System;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
}
