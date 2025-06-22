namespace AppUno.Models;

public class Entity
{
    public record TaskGroup(Store store, GroupList groupedList);
    public record TaskDetail(Store store, BaseTask task);


}

