using System.Collections.ObjectModel;
using System.Linq;
using Avalonia_EventHub;
using App1.Events;
using System;
using App1.ViewModels;

public class Store : ModelBase
{
    private ObservableCollection<GroupList> _lists = new();

    public string? SelectedListName { get; private set; }
    public GroupList? SelectedList { get; private set; }
    public BaseTask? SelectedTask { get; set; }
    public ArchivedList ArchiveLists { get; set; }
    public string WelcomeText { get; set; } = "Welcome";
    public bool Initialized { get; set; }
    public required MainList MainLists { get; set; }
    public string? TopbarText { get; set; }

    public Store(IEventHub events) : base (events)
    {
        MainLists = new MainList(_events);
        ArchiveLists = new ArchivedList(_events);

    }

    public ObservableCollection<GroupList> FilteredLists =>
        new(_lists.Where(g => g.ListName != GlobalVariables.Quick));

    public void ArchiveListsChange(ArchivedList? archiveLists)
    {
        if (archiveLists == null) return;
        ArchiveLists = archiveLists;
        _events.Publish(new ArchiveListsChangedEvent(archiveLists));
    }

    public void MainListsChange(MainList? mainLists)
    {
        if (mainLists == null) return;
        MainLists = mainLists;
        _events.Publish(new MainListsChangedEvent(mainLists));
    }

    public void SelectList(GroupList? list)
    {
        if (list == null) return;
        SelectedList = list;
        SelectedListName = list.ListName;
        _events.Publish(new SelectedListChangedEvent(SelectedList, SelectedListName));
    }

    public void SelectTask(BaseTask? task)
    {
        if (task == null) return;
        SelectedTask = task;
        _events.Publish(new SelectedTaskChangedEvent(SelectedTask));
    }

    public void EditTopBarText(string? text)
    {
        if (text == null) return;
        TopbarText = text;
        _events.Publish(new TopbarTextChangedEvent(text));
    }

    public void SetTaskImportant(bool? value)
    {
        if (SelectedTask == null) return;
        SelectedTask.IsImportant = value;
    }

    public void SetTaskDone(bool? value)
    {
        if (SelectedTask == null) return;
        SelectedTask.IsDone = value;
    }

    public void UpdateImportantList()
    {
        if (SelectedListName == null) return;
        _events.Publish(new ChangeImportantListEvent(SelectedListName));
    }

    public void OnChangeListName(bool value)
    {
        _events.Publish(new ChangeListNameEvent(value));
    }

}