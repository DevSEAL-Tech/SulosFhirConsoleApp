
namespace Sulos.Hospice.Care.Core.Common.Fhir.Http;

public class FhirAuditingHttpMessageHandler : DelegatingHandler
{
    private const string BaseAuditingHeaderName = "X-MS-AZUREFHIR-AUDIT-";
   

    public FhirAuditingHttpMessageHandler(HttpMessageHandler inner)
        : base(inner)
    {
       
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add($"{BaseAuditingHeaderName}UserId", "mysulos");
        request.Headers.Add($"{BaseAuditingHeaderName}OrganizationId", "mysulos");
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}