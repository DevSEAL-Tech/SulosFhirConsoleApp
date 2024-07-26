using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace Sulos.Hospice.Care.Core.Common.Fhir.Exceptions;

public class SulosFhirOperationException : Exception
{
    public SulosFhirOperationException(Base value)
        : base(FormatOutcome(value))
    {
        
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string FormatOutcome(Base value)
    {
        return new StringBuilder()
            .AppendLine("Fhir operation failed with:")
            .AppendLine(value.ToJson())
            .ToString();
    }
}