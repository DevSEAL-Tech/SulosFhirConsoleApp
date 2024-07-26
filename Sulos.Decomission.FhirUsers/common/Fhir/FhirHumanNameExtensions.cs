using Hl7.Fhir.Model;
using static Sulos.Hospice.Care.Models.Common.DataReplacements;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public static class FhirHumanNameExtensions
{
    public static string GetFirstName(this HumanName? name) => name?.Given.FirstOrDefault() ?? Missing;

    public static string GetLastName(this HumanName? name) => name?.Family ?? Missing;
}