using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Sulos.Hospice.Care.Core.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Fhir.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            .Where("active", "true")
            .Build();

            var patientBundle = await client.SearchAsync<Patient>(searchParams).ConfigureAwait(false);

            return patientBundle.GetAllResources<Patient>().ToArray();
        }
    }
}
