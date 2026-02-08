using System;
using System.Threading.Tasks;
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

        if (GlobalVariables.IsAndroid)
            QuickAddTaskTextBox.GetObservable(IsFocusedProperty)
                .Subscribe(async isFocused =>
                {
                    ListsStack.IsEnabled = !isFocused;
                    if (isFocused)
                    {
                        QuickAddTaskBorder.Height = 150;
                        ListsStack.Effect = new BlurEffect { Radius = 10 };
                        QuickAddTaskTextBox.MaxWidth = 450;
                        QuickAddTaskBorder.Background = new SolidColorBrush(new Color(200, 29, 29, 29));
                        Grid.SetRow(QuickAddTaskBorder, 0);
                    }
                    else
                    {
                        if (QuickAddTaskTextBox.ContextFlyout?.IsOpen == true) return;
                        QuickAddTaskBorder.Height = 90;
                        await Task.Delay(100);
                        ListsStack.Effect = new BlurEffect { Radius = 0 };
                        QuickAddTaskTextBox.MaxWidth = 330;
                        QuickAddTaskBorder.Background = new SolidColorBrush(new Color(0, 0, 0, 0));
                        Grid.SetRow(QuickAddTaskBorder, 1);
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