using Avalonia_EventHub;

namespace App1.ViewModels;

public partial class ModelBase
{
    protected readonly IEventHub _events;
        protected ModelBase(
        IEventHub events
    ){
        _events = events;
    }
}