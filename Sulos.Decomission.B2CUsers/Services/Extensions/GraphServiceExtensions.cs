namespace Sulos.Decomission.B2CUsers.Services.Extensions;

public class GraphServiceExtensions
{
    private readonly string _extensionApplicationId;

    public GraphServiceExtensions(string extensionApplicationId)
    {
        _extensionApplicationId = extensionApplicationId.Replace("-", string.Empty);
    }

    public string this[GraphServiceExtensionAttributes index] => $"extension_{_extensionApplicationId}_{index}";
}
