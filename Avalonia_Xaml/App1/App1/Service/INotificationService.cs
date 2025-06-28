

using System;
using Ursa.Controls;

public class NotificationService : INotificationService
{
    public WindowNotificationManager? NotificationManager { get; set; }
    public WindowToastManager? ToastManager { get; set; }
    public void ShowNotification(string message, string pos = "TopCenter")
    {
        if (NotificationManager is not null)
        {
            Enum.TryParse<Avalonia.Controls.Notifications.NotificationPosition>(pos, out var notificationPosition);
            NotificationManager.Position = notificationPosition;
        }
        NotificationManager?.Show(
            new Notification("Warning", message),
            showIcon: true,
            showClose: false,
            type: Avalonia.Controls.Notifications.NotificationType.Warning,
            classes: ["Light"]);
    }
}

public interface INotificationService
{
    void ShowNotification(string message, string position = "TopCenter");
    public WindowNotificationManager? NotificationManager { get; set; }
    public WindowToastManager? ToastManager { get; set; }
}