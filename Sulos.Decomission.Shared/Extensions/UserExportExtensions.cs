using System.Text.Json;
using Sulos.Decomission.Shared.Models;

namespace Sulos.Decomission.Shared.Extensions;

public static class UserExportExtensions
{
    public static string SerializeCollection(IEnumerable<UserExport> users)
    {
        return JsonSerializer.Serialize(users);
    }

    public static IEnumerable<UserExport> DeserializeCollection(string collectionJson)
    {
        var users = JsonSerializer.Deserialize<UserExport[]>(collectionJson) ?? [];
        return users;
    }
}