using Avalonia.Controls;
using System;
using App1.ViewModels;
namespace App1.Views;

public partial class GroupListView : UserControl
{
    private bool _isTextBoxshow = false;

    public GroupListView()
    {
        InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;

        NewListBox.IsVisible = _isTextBoxshow;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is GroupListViewModel vm)
        {
            vm.OnSaveAddList = () =>
            {
                NewListBox.Text = null;
                _isTextBoxshow = !_isTextBoxshow;
                NewListBox.IsVisible = _isTextBoxshow;
            };
        }
    }
}