using Sulos.Hospice.Care.Models.Caregivers;
using Sulos.Hospice.Care.Models.Common;
using Sulos.Hospice.Care.Models.Users;

namespace Sulos.Hospice.Care.Models.Patients;

public record SulosPatientModelV2(
    string Id,
    string FirstName,
    string LastName,
    string Gender,
    SulosAddressModel Address,
    SulosContactModel Contact,
    string? PatientStatus,
    string? CodeStatus,
    string? About,
    DateTime? DateOfBirth,
    DateTime? RecordCreationDate,
    DateTime? OldestRequestDate,
    DateTime? FirstLoginTime,
    DateTime? LastLoginTime,
    SulosCaregiverModel? Caregiver,
    SulosUserModel[] AllAssignedPractitioners
) : SulosPersonModel(Id, FirstName, LastName, PersonType.Patient

    );