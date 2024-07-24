using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;
using Sulos.Hospice.Care.Core.Common.Azure;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public interface IFhirClientOptionsFactory
{
    Task<ClientCredentialsOptions> CreateReaderAsync(string hospiceId);
    Task<ClientCredentialsOptions> CreateWriterAsync(string hospiceId);
}

public class FhirClientOptionsFactory : IFhirClientOptionsFactory
{
    private readonly IOptions<FhirConfigOptions> _fhirConfigOptions;
    private readonly SecretClient _secretClient;

    private FhirConfigOptions Options => _fhirConfigOptions.Value;

    public FhirClientOptionsFactory(IOptions<FhirConfigOptions> fhirConfigOptions, SecretClient secretClient)
    {
        _fhirConfigOptions = fhirConfigOptions;
        _secretClient = secretClient;
    }

    public async Task<ClientCredentialsOptions> CreateReaderAsync(string hospiceId)
    {
        var urlSecret = await _secretClient.GetSecretAsync($"{hospiceId}{Options.UrlPostfix}");
        var clientIdSecret = await _secretClient.GetSecretAsync($"{hospiceId}{Options.ReaderClientIdPostfix}");
        var clientSecret = await _secretClient.GetSecretAsync($"{hospiceId}{Options.ReaderClientSecretPostfix}");
        var tenantIdSecret = await _secretClient.GetSecretAsync($"{hospiceId}{Options.TenantIdPostfix}");
        return new ClientCredentialsOptions(
            urlSecret.Value.Value,
            clientIdSecret.Value.Value,
            clientSecret.Value.Value,
            tenantIdSecret.Value.Value);
    }

    public async Task<ClientCredentialsOptions> CreateWriterAsync(string hospiceId)
    {
        var urlSecret = await _secretClient.GetSecretAsync($"{hospiceId}{Options.UrlPostfix}");
        var clientIdSecret = await _secretClient.GetSecretAsync($"{hospiceId}{Options.WriterClientIdPostfix}");
        var clientSecret = await _secretClient.GetSecretAsync($"{hospiceId}{Options.WriterClientSecretPostfix}");
        var tenantIdSecret = await _secretClient.GetSecretAsync($"{hospiceId}{Options.TenantIdPostfix}");
        return new ClientCredentialsOptions(
            urlSecret.Value.Value,
            clientIdSecret.Value.Value,
            clientSecret.Value.Value,
            tenantIdSecret.Value.Value
        );
    }
}