using Hl7.Fhir.Model;
using Microsoft.Graph.Models;
using Sulos.Hospice.Care.Core.Common.Azure;
using Sulos.Hospice.Care.Models.Common;
using Sulos.Hospice.Care.Models.Users;
using static Sulos.Hospice.Care.Core.Common.SulosConstants;
using static Sulos.Hospice.Care.Core.Common.SulosConstants.PractitionerExtensions;
using PersonType = Sulos.Hospice.Care.Models.Users.PersonType;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public static class FhirPractitionerExtensions
{
    public static SulosPersonModel ToSulosPersonModel(
        this Practitioner practitioner,
        SulosAzureAdPermissions? sulosAzureAdPermissions = null
    )
    {
        return new SulosPersonModel(
            practitioner.Id,
            practitioner.GetFirstName(),
            practitioner.GetLastName(),
            PersonType.InternalUser,
            sulosAzureAdPermissions == null ? null : ToParsedValue<UserRole>(sulosAzureAdPermissions.Role),
            sulosAzureAdPermissions?.RoleType
        );
    }

    public static SulosUserModel[] ToSulosUserModels(
        this IEnumerable<Practitioner> practitioners,
        IDictionary<string, SulosAzureAdPermissions> permissions
    ) => practitioners
        .Select(p =>
        {
            permissions.TryGetValue(p.Id, out var userPermissions);
            return p.ToSulosUserModel(userPermissions);
        })
        .ToArray();

    public static SulosUserModel ToSulosUserModel(
        this Practitioner practitioner,
        SulosAzureAdPermissions? sulosAzureAdPermissions = null
    )
    {
        sulosAzureAdPermissions ??= new SulosAzureAdPermissions(
            false,
            practitioner.GetRole()?.ToString(),
            practitioner.GetRoleType(),
            practitioner.GetProfile()?.ToString()
        );

        return SulosUserModel.FromPersonModel(
            practitioner.ToSulosPersonModel(sulosAzureAdPermissions),
            ToSulosStatus(sulosAzureAdPermissions.Enabled),
            practitioner.GetProfile(),
            practitioner.GetContactModel()
        );
    }
    public static SulosContactModel GetContactModel(this Practitioner practitioner)
    {
        var email = practitioner.Telecom
            .Find(contact => contact.System == ContactPoint.ContactPointSystem.Email)
            ?.Value;

        var phone = practitioner.Telecom
            .Find(contact => contact.System == ContactPoint.ContactPointSystem.Phone)
            ?.Value;

        return new SulosContactModel(email, phone);
    }
    private static UserStatus ToSulosStatus(bool? isEnabled)
    {
        switch (isEnabled)
        {
            case false:
                return UserStatus.Inactive;
            default:
                return UserStatus.Active;
        }
    }
    private static T? ToParsedValue<T>(string? enumValueStr) where T : struct, Enum
    {
        if (Enum.TryParse<T>(enumValueStr, out var enumValue))
        {
            return enumValue;
        }

        return null;
    }
    public static UserRole? GetRole(this Practitioner practitioner)
    {
        return practitioner.GetSulosExtensionEnum<UserRole>(RoleExtensionType);
    }
    public static string? GetRoleType(this Practitioner practitioner)
    {
        return practitioner.GetSulosExtensionValue(RoleTypeExtensionType);
    }

    public static void SetRoleType(this Practitioner practitioner, string? roleType)
    {
        practitioner.SetSulosExtensionValue(RoleTypeExtensionType, roleType);
    }

    public static UserProfile? GetProfile(this Practitioner practitioner)
    {
        return practitioner.GetSulosExtensionEnum<UserProfile>(ProfileExtensionType);
    }

    public static void SetProfile(this Practitioner practitioner, UserProfile profile)
    {
        practitioner.SetSulosExtensionEnum(ProfileExtensionType, profile);
    }
    private static HumanName? GetUsualName(this Practitioner practitioner) =>
        practitioner.Name.Find(n => n.Use == HumanName.NameUse.Usual);

    public static string GetFirstName(this Practitioner practitioner) => practitioner.GetUsualName().GetFirstName();

    public static string GetLastName(this Practitioner practitioner) => practitioner.GetUsualName().GetLastName();

   

    public static void SetPhoneNumber(this Practitioner practitioner, string phoneNumber)
    {
        UpsertTelecomContactPoint(practitioner, ContactPoint.ContactPointSystem.Phone, phoneNumber);
    }


    private static void UpsertTelecomContactPoint(this Practitioner practitioner,
     ContactPoint.ContactPointSystem system, string value)
    {
        var existingContactPoint = practitioner.Telecom.Find(contact =>
            contact.System == system);

        if (existingContactPoint != null)
        {
            existingContactPoint.Value = value;
        }
        else
        {
            practitioner.Telecom.Add(new ContactPoint { System = system, Value = value });
        }
    }

}