using App1.ViewModels;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<Store>();
        collection.AddSingleton<IDialogService, DialogService>();
        collection.AddSingleton<INotificationService, NotificationService>();
        collection.AddSingleton<IChangeStateService, ChangeStateService>();
        
        collection.AddTransient<MainViewModel>();
        collection.AddTransient<TaskDetailViewModel>();
        collection.AddTransient<AddTaskViewModel>();
        collection.AddTransient<GroupListViewModel>();
        collection.AddTransient<NewTaskOptionViewModel>();
        collection.AddTransient<WelcomeViewModel>();
        collection.AddTransient<TaskGroupViewModel>();
    }
}