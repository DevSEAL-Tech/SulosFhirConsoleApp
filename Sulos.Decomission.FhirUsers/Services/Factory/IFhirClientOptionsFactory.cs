using Microsoft.Extensions.Options;
using Sulos.Hospice.Care.Core.Common.Azure;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public interface IFhirClientOptionsFactory
{
    ClientCredentialsOptions CreateReaderAsync(string organization);
    ClientCredentialsOptions CreateWriterAsync(string organization);
}

public class FhirClientOptionsFactory : IFhirClientOptionsFactory
{
    private FhirConfigOptions Options { get; }

    public FhirClientOptionsFactory(IOptions<FhirConfigOptions> fhirConfigOptions)
    {
        Options = fhirConfigOptions.Value;
    }

    public ClientCredentialsOptions CreateReaderAsync(string organization)
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

    public ClientCredentialsOptions CreateWriterAsync(string organization)
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