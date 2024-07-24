using System.Text.Json;

namespace Sulos.Decomission.B2CUsers.Services.Exceptions;

public class GraphServiceException : Exception
{
    public GraphServiceException(string message)
        : base(message)
    {
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}