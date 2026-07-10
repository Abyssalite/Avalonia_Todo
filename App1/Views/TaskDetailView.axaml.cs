using Avalonia.Controls;
using Avalonia;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace App1.Views;

public partial class TaskDetailView : UserControl
{
    private CancellationTokenSource? _resizeToken;

    public TaskDetailView()
    {
        InitializeComponent();
        
        this.LayoutUpdated += (_, __) =>
        {
            _resizeToken?.Cancel();
            _resizeToken = new CancellationTokenSource();

            var token = _resizeToken.Token;
            Task.Delay(10, token).ContinueWith(t =>
            {
                if (t.IsCanceled) return;
                Dispatcher.UIThread.InvokeAsync(AdjustBorderSize);
            });
        };
    }

    private void AdjustBorderSize()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var width = topLevel.Bounds.Width;

        if (width < 650)
        {
            TaskDetailPanel.Margin =  new Thickness(10) ;
        }
        else if (width > 650)
        {
            TaskDetailPanel.Margin =  new Thickness(100, 20, 100, 20);
        }
    }
}