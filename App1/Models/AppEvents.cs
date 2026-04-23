namespace App1.Events;

public sealed record ListsChangedEvent;
public sealed record ArchiveListsChangedEvent;
public sealed record SelectedListChangedEvent(GroupList? SelectedList, string SelectedListName);
public sealed record TopbarTextChangedEvent(string Text);

public sealed record TaskDoneChangedEvent(BaseTask Task, bool IsDone);
public sealed record TaskImportantChangedEvent(BaseTask Task, bool IsImportant);

public sealed record ListArchiveStateChangedEvent(GroupList List, bool IsArchived);
public sealed record ListEditedEvent(GroupList List);
public sealed record TaskGroupsChangedEvent(GroupList List);