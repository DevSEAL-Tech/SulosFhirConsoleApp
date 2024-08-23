using Hl7.Fhir.Model;
using Sulos.Hospice.Care.Core.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Fhir.Searching;

namespace Sulos.Decomission.FhirUsers.Services.Interfaces
{
    public interface ISulosGetPatients
    {
        Task<Patient[]> GetPatients(SulosFhirClient client);
    }

    public class SulosGetPatients : ISulosGetPatients
    {
        public async Task<Patient[]> GetPatients(SulosFhirClient client)
        {
            var searchParams = new FhirSearchParamsBuilder()
            //.Where("active", "true")
            .Build();

            var patientBundle = await client.SearchAsync<Patient>(searchParams).ConfigureAwait(false);

            return patientBundle.GetAllResources<Patient>().ToArray();
        }
    }
}
