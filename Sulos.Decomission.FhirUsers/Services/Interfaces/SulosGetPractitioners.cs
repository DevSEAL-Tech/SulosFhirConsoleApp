using Hl7.Fhir.Model;
using Sulos.Hospice.Care.Core.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Fhir.Searching;

namespace Sulos.Decomission.FhirUsers.Services.Interfaces
{
    public interface ISulosGetPractitioners
    {
        Task<Practitioner[]> GetPractitioners(SulosFhirClient client);
    }

    public class SulosGetPractitioners : ISulosGetPractitioners
    {
        public async Task<Practitioner[]> GetPractitioners(SulosFhirClient client)
        {
            var searchParams = new FhirSearchParamsBuilder()
            //.Where("active", "true")
            .Build();

            var bundle = await client.SearchAsync<Practitioner>(searchParams).ConfigureAwait(false);

            return bundle.GetAllResources<Practitioner>().ToArray();
        }
    }
}
