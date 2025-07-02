using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;


namespace App1.Views;

public partial class TaskGroupView : UserControl
{
    public TaskGroupView()
    {
        InitializeComponent();
        ListsStack.IsEnabled = true;
        if (OperatingSystem.IsAndroid())
            QuickAddTaskTextBox.GetObservable(TextBox.IsFocusedProperty)
            .Subscribe(isFocused =>
            {
                ListsStack.IsEnabled = !isFocused;
                if (isFocused)
                {
                    ListsStack.Effect = new BlurEffect
                    {
                        Radius = 10,
                    };
                    Grid.SetRow(QuickAddTaskBorder, 0);
                    QuickAddTaskBorder.Height = 150;
                    QuickAddTaskTextBox.MaxWidth = 450;
                    QuickAddTaskBorder.Background = new SolidColorBrush(new Color(200, 29, 29, 29));
                }
                else
                {
                    ListsStack.Effect = new BlurEffect
                    {
                        Radius = 0,
                    };
                    Grid.SetRow(QuickAddTaskBorder, 1);
                    QuickAddTaskBorder.Height = 90;
                    QuickAddTaskTextBox.MaxWidth = 330;
                    QuickAddTaskBorder.Background = new SolidColorBrush(new Color(0, 0, 0, 0));
                }
            });
        else
        {
            Grid.SetRow(QuickAddTaskBorder, 1);
            QuickAddTaskBorder.Height = 90;
            QuickAddTaskTextBox.MaxWidth = 330;
        }
    }
}