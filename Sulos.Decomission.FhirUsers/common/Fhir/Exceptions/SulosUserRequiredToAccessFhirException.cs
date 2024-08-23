using System.Runtime.Serialization;
using System.Text.Json;

namespace Sulos.Hospice.Care.Core.Common.Fhir.Exceptions;

public class SulosUserRequiredToAccessFhirException : Exception
{
    public SulosUserRequiredToAccessFhirException()
        : base("Attempt was made to access fhir without a sulos user.")
    {
        
    }
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}