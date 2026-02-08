using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ObservableCollection<GroupList>))]
internal partial class AppJsonContext : JsonSerializerContext
{
}