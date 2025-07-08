using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace App1.Components;

public class CustomTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);
    private MenuFlyout? _flyout;

    public CustomTextBox()
    {
        _flyout = CreateHorizontalFlyout();
        ContextFlyout = _flyout;
    }

    private MenuFlyout CreateHorizontalFlyout()
    {
        var flyout = new MenuFlyout { };

        var cutButton = new MenuItem { Header = "Cut", Padding = new Thickness(10) };
        var copyButton = new MenuItem { Header = "Copy", Padding = new Thickness(10) };
        var pasteButton = new MenuItem { Header = "Paste", Padding = new Thickness(10) };

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
        };

        copyButton.Click += (_, _) =>
        {
            Copy();
        };

        pasteButton.Click += (_, _) =>
        {
            Paste();
        };

          flyout.ItemsSource = new[]
        {
            cutButton, copyButton, pasteButton, 
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