using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ObservableCollection<GroupList>))]
[JsonSerializable(typeof(GroupList))]

internal partial class AppJsonContext : JsonSerializerContext
{
}