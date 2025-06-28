using System;
using System.Threading.Tasks;
using App1.Dialogs;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.Options;

public class DialogService : IDialogService
{
    public void ShowNotification(string message, string? pos)
    {
        var vm = new CustomDialogViewModel(message, pos); 
        OverlayDialog.ShowCustomModal<CustomDialog, CustomDialogViewModel, bool>(vm);
        vm.ShowNotification();
    }

    public async Task<bool?> ShowDialogAsync(string message, string? pos)
    {
        var vm = new CustomDialogViewModel(message, pos);
        var tcs = new TaskCompletionSource<bool?>();
        var drawerOptions = new DrawerOptions
        {
            Position = Position.Bottom,
            CanLightDismiss = true,
            IsCloseButtonVisible = false,
            Buttons = DialogButton.None,
            CanResize = false,
        };

        var dialogOptions = new OverlayDialogOptions
        {
            CanLightDismiss = true,
            IsCloseButtonVisible = false,
            Buttons = DialogButton.None,
            CanResize = false,
            CanDragMove = false
        };

        vm.OnClose = async confirm =>
        {
            tcs.SetResult(confirm);
            await Task.CompletedTask;
        };

        if (OperatingSystem.IsAndroid())
        {
            await Drawer.ShowCustomModal<CustomDialog, CustomDialogViewModel, object?>(vm, null, drawerOptions);
        }
        else
        {
            await OverlayDialog.ShowCustomModal<CustomDialog, CustomDialogViewModel, bool>(vm, null, dialogOptions);
        }
        return await tcs.Task;
    } 
}

public interface IDialogService
{
    Task<bool?> ShowDialogAsync(string message, string? pos);
    void ShowNotification(string message, string? pos);
}
