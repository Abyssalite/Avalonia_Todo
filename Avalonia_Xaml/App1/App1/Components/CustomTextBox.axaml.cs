using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace App1.Components;

public class CustomTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);
    private Flyout? _flyout;

    public CustomTextBox()
    {
        _flyout = CreateHorizontalFlyout();
        ContextFlyout = _flyout;
    }

    private Flyout CreateHorizontalFlyout()
    {
        var flyout = new Flyout { };

        var cutButton = new MenuItem { Header = "Cut", Padding = new Thickness(8, 0, 8, 0) };
        var copyButton = new MenuItem { Header = "Copy", Padding = new Thickness(8, 0, 8, 0) };
        var pasteButton = new MenuItem { Header = "Paste", Padding = new Thickness(8, 0, 8, 0) };

        this.GetObservable(IsFocusedProperty)
            .Subscribe(isFocused =>
            {
                pasteButton.IsEnabled = CanPaste;
                cutButton.IsEnabled = CanCut;
                copyButton.IsEnabled = CanCopy;
            });
        HighlightButton(cutButton);
        HighlightButton(copyButton);
        HighlightButton(pasteButton);

        cutButton.Click += (_, _) =>
        {
            Cut();
            flyout.Hide();
        };

        copyButton.Click += (_, _) =>
        {
            Copy();
            flyout.Hide();
        };

        pasteButton.Click += (_, _) =>
        {
            Paste();
            flyout.Hide();
        };

        cutButton.IsEnabled = CanCut;

        flyout.Content = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 0,
            Children = { cutButton, copyButton, pasteButton, }
        };

        return flyout;
    }

    private void HighlightButton(MenuItem button)
    {
        button.GetObservable(IsPointerOverProperty)
            .Subscribe(isFocused =>
            {
                button.Background = isFocused ?
                    new SolidColorBrush(new Color(200, 180, 180, 180)) :
                    new SolidColorBrush(new Color(0, 0, 0, 0));
            });
    }
}