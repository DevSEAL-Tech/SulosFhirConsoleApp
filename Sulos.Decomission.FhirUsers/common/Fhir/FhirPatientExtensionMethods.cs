using Hl7.Fhir.Model;
using Microsoft.Graph.Models.Security;
using Sulos.Hospice.Care.Core.Common.Azure;
using Sulos.Hospice.Care.Models.Caregivers;
using Sulos.Hospice.Care.Models.Common;
using Sulos.Hospice.Care.Models.Patients;
using Sulos.Hospice.Care.Models.Users;
using System.Reflection;
using Sulos.Hospice.Care.Core.Common;
using static Sulos.Hospice.Care.Core.Common.SulosConstants;


namespace Sulos.Hospice.Care.Core.Common.Fhir;

public static class FhirPatientExtensionMethods
{
    public static string? GetIdentifierValue(this Patient patient, string system)
    {
        var identifier = patient.GetIdentifiersBySystem(system).SingleOrDefault(i => i.System == system);
        return identifier?.Value;
    }

    public static void SetIdentifierValue(this Patient patient, string system, string value)
    {
        var identifier = patient.Identifier.SingleOrDefault(i => i.System == system);
        if (identifier == null)
            patient.Identifier.Add(new Identifier(system, value));
        else
            identifier.Value = value;
    }

    public static string GetFirstName(this Patient patient) => patient.GetUsualName().GetFirstName();

    public static string GetLastName(this Patient patient) => patient.GetUsualName().GetLastName();

    public static HumanName.NameUse? GetNameUse(this Patient patient) =>
        patient.Name.FirstOrDefault()?.Use;


    public static HumanName.NameUse? GetCaregiverNameUse(this Patient patient) => patient.Contact?
        .FirstOrDefault()?.Name.Use;

    public static ContactPoint.ContactPointUse? GetCaregiverPhoneNumberUse(this Patient patient) => patient.Contact?
        .FirstOrDefault()?.Telecom.FirstOrDefault()?.Use;

    public static string? GetGenderAsString(this Patient patient) =>
        patient.Gender == null ? null : Enum.GetName(typeof(AdministrativeGender), patient.Gender);

    public static void SetEmail(this Patient patient, string email) =>
        UpsertTelecomContactPoint(patient, ContactPoint.ContactPointSystem.Email, email);

    public static string? GetEmail(this Patient patient) => patient.Telecom
        .Find(contact => contact.System == ContactPoint.ContactPointSystem.Email)?.Value;

    public static string? GetEmailUse(this Patient patient) => patient.Telecom
        .Find(contact => contact.System == ContactPoint.ContactPointSystem.Email)?.Use.ToString();

    public static string? GetPhoneNumber(this Patient patient) => patient.Telecom
        .Find(contact => contact.System == ContactPoint.ContactPointSystem.Phone)?.Value;

    public static string? GetPhoneNumberUse(this Patient patient) => patient.Telecom
        .Find(contact => contact.System == ContactPoint.ContactPointSystem.Phone)?.Use.ToString();

    public static string? GetStreetAddress1(this Patient patient) => patient.Address
        .FirstOrDefault()?.Line.FirstOrDefault();

    public static string? GetStreetAddress2(this Patient patient)
    {
        return patient.Address.FirstOrDefault()?.LineElement.Count > 1
            ? patient.Address.FirstOrDefault()?.LineElement[1].Value
            : null;
    }

    public static string? GetCity(this Patient patient) => patient.Address
        .FirstOrDefault()?.City;

    public static string? GetState(this Patient patient) => patient.Address
        .FirstOrDefault()?.State;

    public static string? GetPostalCode(this Patient patient) => patient.Address
        .FirstOrDefault()?.PostalCode;




    //public static SulosBasicPatientModel ToSulosBasicPatientModel(this Patient patient, SulosAssignedPractitioner[]? assignedPractitioners = null) =>
    //    SulosBasicPatientModel.FromPerson(
    //        patient.ToSulosPersonModel(),
    //        patient.GetDateOfBirth(),
    //        patient.GetGenderAsStringOrDefault(),
    //        patient.GetAddress(),
    //        patient.GetContactInformation(),
    //        assignedPractitioners
    //    );

    public static SulosPersonModel ToSulosPersonModel(this Patient patient) =>
        new(
            patient.Id,
            patient.GetFirstName(),
            patient.GetLastName(),
            PersonType.Patient
        );

    //public static SulosPatientModelV2 ToSulosPatientModel(
    //    this Patient patient,
    //    IEnumerable<Practitioner>? practitioners = null,
    //    IDictionary<string, SulosAzureAdPermissions>? permissions = null
    //)
    //{
    //    var permissionsOrDefault = permissions ?? new Dictionary<string, SulosAzureAdPermissions>();
    //    var allAssignedPractitioners = ArrayOrDefault(practitioners?.ToSulosUserModels(permissionsOrDefault));
    //    return new SulosPatientModelV2(
    //        Id: patient.Id,
    //        FirstName: patient.GetFirstName(),
    //        LastName: patient.GetLastName(),
    //        Gender: patient.GetGenderAsStringOrDefault(),
    //        DateOfBirth: patient.GetDateOfBirth()
    //    );
    //}

    public static SulosPatientModelV2 ToSulosPatientModel(
       this Patient patient,
       IEnumerable<Device>? devices = null,
       IEnumerable<Condition>? conditions = null,
       IEnumerable<Flag>? flags = null,
       DateTime? oldestRequestDate = null,
       IEnumerable<Practitioner>? practitioners = null,
       IEnumerable<Basic>? thresholds = null,
       IEnumerable<Basic>? notes = null,
       IDictionary<string, SulosAzureAdPermissions>? permissions = null
   )
    {
        
        var permissionsOrDefault = permissions ?? new Dictionary<string, SulosAzureAdPermissions>();
        var allAssignedPractitioners = ArrayOrDefault(practitioners?.ToSulosUserModels(permissionsOrDefault));

        var patientStatusCode = patient.GetPatientStatus();
        var patientStatusDisplay = ConvertToStatusDisplay(patientStatusCode);

        return new SulosPatientModelV2(
            Id: patient.Id,
            FirstName: patient.GetFirstName(),
            LastName: patient.GetLastName(),
            Gender: patient.GetGenderAsStringOrDefault(),
           
            Address: patient.GetAddress(),
            Contact: patient.GetContactInformation(),  
            PatientStatus: patientStatusCode,
            CodeStatus: patient.GetCodeStatus(),
            About: patient.GetAbout(),
            DateOfBirth: patient.GetDateOfBirth(),
            RecordCreationDate: patient.GetRecordCreationDate(),
            OldestRequestDate: oldestRequestDate,
            FirstLoginTime: patient.GetFirstLoginTime(),
            LastLoginTime: patient.GetLastLoginTime(),
            Caregiver: patient.GetCaregiver(),
            AllAssignedPractitioners: allAssignedPractitioners
           
        );
    }

    public static string GetPatientStatus(this Patient patient)
    {
        var fhirPatientStatusExtension = patient.FindExtension(PatientExtensions.PatientStatus);
        var newPatientStatus = fhirPatientStatusExtension?.Value.First().Value;
        return newPatientStatus?.ToString() ?? "";
    }
    private static SulosAddressModel GetAddress(this Patient patient)
    {
        var fhirAddress = patient.Address.FirstOrDefault();
        var addressLines = fhirAddress?.Line?.ToArray() ?? new[] { DataReplacements.Missing };
        return new SulosAddressModel(addressLines, fhirAddress?.City, fhirAddress?.State, fhirAddress?.PostalCode);
    }
    public static DateTime? GetRecordCreationDate(this Patient patient) =>
       patient.GetExtensionDateTime(PatientExtensions.RecordCreationDate);
    public static string? GetAbout(this Patient patient)
    {
        var fhirPatientAboutExtension = patient.FindExtension(PatientExtensions.About);
        var about = fhirPatientAboutExtension?.Value?.First().Value;
        return about?.ToString();
    }
    public static string GetCodeStatus(this Patient patient)
    {
        var fhirPatientStatusExtension = patient.FindExtension(PatientExtensions.CodeStatus);
        var codeStatus = fhirPatientStatusExtension?.Value.First().Value;
        return codeStatus?.ToString() ?? "";
    }
    private static SulosContactModel GetContactInformation(this Patient patient)
    {
        var email = patient.Telecom.Find(contact => contact.System == ContactPoint.ContactPointSystem.Email)
            ?.Value;
        var phone = patient.Telecom.Find(contact => contact.System == ContactPoint.ContactPointSystem.Phone)
            ?.Value;

        return new SulosContactModel(email, phone);
    }
    private static T[] ArrayOrDefault<T>(T[]? array) => array ?? Array.Empty<T>();

    private static string ConvertToStatusDisplay(string patientStatusCode) =>
        patientStatusCode.Replace("Active - ", "");

    private static IEnumerable<Identifier> GetIdentifiersBySystem(this Patient patient, string system) =>
        patient.Identifier.Where(i => i.System == system);

    private static HumanName? GetUsualName(this Patient patient) =>
        patient.Name.Find(n => n.Use == HumanName.NameUse.Usual);

    public static DateTime? GetFirstLoginTime(this Patient patient) => patient.GetExtensionDateTime(PatientExtensions.FirstLoginTime);

    public static DateTime? GetLastLoginTime(this Patient patient) => patient.GetExtensionDateTime(PatientExtensions.LastLoginTime);


    private static DateTime? GetDateOfBirth(this Patient patient) =>
        DateTime.TryParse(patient.BirthDate, out var dateOfBirth) ? dateOfBirth.Date : null;

    private static string GetGenderAsStringOrDefault(this Patient patient) => patient.GetGenderAsString() ?? "N/A";


    public static SulosCaregiverModel GetCaregiver(this Patient patient)
    {
        var caregiver = patient.Contact?.FirstOrDefault();
        return new SulosCaregiverModel(
            caregiver?.Name?.Given?.FirstOrDefault(),
            caregiver?.Name?.Family,
            caregiver?.Telecom?.FirstOrDefault()?.Value);
    }

    private static void UpsertTelecomContactPoint(this Patient patient, ContactPoint.ContactPointSystem system,
        string value)
    {
        var existingContactPoint = patient.Telecom.Find(contact => contact.System == system);
        if (existingContactPoint != null)
        {
            existingContactPoint.Value = value;
        }
        else
        {
            patient.Telecom.Add(new ContactPoint { System = system, Value = value });
        }
    }

    private static DateTime? GetExtensionDateTime(this Patient patient, string extensionName)
    {
        var extension = patient.FindExtension(extensionName);
        var extensionTime = extension?.Value.First().Value;

        if (extensionTime is string extensionDate &&
            DateTime.TryParse(extensionDate, out var recordDate))
        {
            return recordDate;
        }

        return null;
    }


}