using Hl7.Fhir.Model;
using Sulos.Hospice.Care.Models.Common;
using PersonType = Sulos.Hospice.Care.Models.Users.PersonType;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public static class FhirPractitionerExtensions
{
    public static SulosPersonModel ToSulosPersonModel(
        this Practitioner practitioner
    )
    {
        return new SulosPersonModel(
            practitioner.Id,
            practitioner.GetFirstName(),
            practitioner.GetLastName(),
            practitioner.GetEmailAddress(),
            PersonType.InternalUser
        );
    }

    private static HumanName? GetUsualName(this Practitioner practitioner) =>
        practitioner.Name.Find(n => n.Use == HumanName.NameUse.Usual);

    public static string GetFirstName(this Practitioner practitioner) => practitioner.GetUsualName().GetFirstName();

    public static string GetLastName(this Practitioner practitioner) => practitioner.GetUsualName().GetLastName();

    public static string GetEmailAddress(this Practitioner practitioner) =>
        practitioner.Telecom
            .Find(contact => contact.System == ContactPoint.ContactPointSystem.Email)
            ?.Value;
}