using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;
using Sulos.Hospice.Care.Core.Common.Azure;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public interface IFhirClientOptionsFactory
{
    Task<ClientCredentialsOptions> CreateReaderAsync();
    Task<ClientCredentialsOptions> CreateWriterAsync();
}

public class FhirClientOptionsFactory : IFhirClientOptionsFactory
{
    private readonly IOptions<FhirConfigOptions> _fhirConfigOptions;
    //private readonly SecretClient _secretClient;

    private FhirConfigOptions Options => _fhirConfigOptions.Value;

    //public FhirClientOptionsFactory(IOptions<FhirConfigOptions> fhirConfigOptions, SecretClient secretClient)
    public FhirClientOptionsFactory(IOptions<FhirConfigOptions> fhirConfigOptions)
    {
        _fhirConfigOptions = fhirConfigOptions;
        //_secretClient = secretClient;
    }

    public async Task<ClientCredentialsOptions> CreateReaderAsync()
    {
        var urlSecret =  $"{Options.UrlPostfix}";
        var clientIdSecret =  $"{Options.ReaderClientIdPostfix}";
        var clientSecret =  $"{Options.ReaderClientSecretPostfix}";
        var tenantIdSecret =  $"{Options.TenantIdPostfix}";
        return new ClientCredentialsOptions(
            urlSecret,
            clientIdSecret,
            clientSecret,
            tenantIdSecret);
    }

    public async Task<ClientCredentialsOptions> CreateWriterAsync()
    {
        var urlSecret =  $"{Options.UrlPostfix}";
        var clientIdSecret =  $"{Options.WriterClientIdPostfix}";
        var clientSecret =  $"{Options.WriterClientSecretPostfix}";
        var tenantIdSecret =  $"{Options.TenantIdPostfix}";
        return new ClientCredentialsOptions(
            urlSecret,
            clientIdSecret,
            clientSecret,
            tenantIdSecret
        );
    }
}