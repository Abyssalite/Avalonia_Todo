using App1.ViewModels;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IDialogService, DialogService>();
        collection.AddSingleton<Store>();
        collection.AddSingleton<IViewHost, ViewHost>();
        collection.AddSingleton<INotificationService, NotificationService>();
        collection.AddSingleton<IPaneService, PaneService>();
        collection.AddTransient<MainViewModel>();
        collection.AddTransient<TaskDetailViewModel>();
        collection.AddTransient<AddTaskViewModel>();
        collection.AddTransient<GroupListViewModel>();
        collection.AddTransient<NewTaskOptionViewModel>();
        collection.AddTransient<WellcomeViewModel>();
        collection.AddTransient<TaskGroupViewModel>();
    }
}