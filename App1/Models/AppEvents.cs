using System.Collections.ObjectModel;

namespace App1.Events;


public sealed record TaskIsDoneChangedEvent(BaseTask Task, bool? IsDone);
public sealed record TaskIsImportantChangedEvent(BaseTask Task, bool? IsImportant);

public sealed record GroupListIsArchiveStateChangedEvent(GroupList List, bool IsArchived);
public sealed record GroupListChangedEvent(ObservableCollection<TaskGroup> Groups);

public sealed record TaskGroupChangedEvent(ObservableCollection<BaseTask> Tasks);

public sealed record ArchiveListsChangedEvent(ArchivedList Lists);
public sealed record FilteredListsChangedEvent(ObservableCollection<GroupList> Lists);
public sealed record MainListsChangedEvent(MainList Lists);

public sealed record SelectedListChangedEvent(GroupList? SelectedList, string SelectedListName);
public sealed record SelectedTaskChangedEvent(BaseTask? SelectedTask);
public sealed record TopbarTextChangedEvent(string Text);
public sealed record ChangeListNameEvent(bool value);

public sealed record ChangeImportantListEvent();
