using System;
namespace Sulos.Hospice.Care.Core.Common.KeyVault;

public class KeyVaultOptions
{
    public const string ConfigSection = "KeyVault";
    public string Url { get; set; } = "";
}
