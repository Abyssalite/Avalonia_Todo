using Avalonia.Controls;
using System;
using App1.ViewModels;
namespace App1.Views;

public partial class GroupListView : UserControl
{
    private bool IsTextBoxshow { set; get; } = false;

    public GroupListView()
    {
        InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;
        SaveButton.Content = "Add List";
        NewListBox.IsVisible = IsTextBoxshow;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is GroupListViewModel vm)
        {
            vm.OnSaveAddList = () =>
            {
                SaveButton.Content = IsTextBoxshow ? "Add List" : "Save";
                NewListBox.Text = null;
                IsTextBoxshow = !IsTextBoxshow;
                NewListBox.IsVisible = IsTextBoxshow;
            };
        }
    }
}