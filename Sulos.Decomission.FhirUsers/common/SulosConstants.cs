
namespace Sulos.Hospice.Care.Core.Common;

public static class SulosConstants
{
    public const string SulosSystem = "sulos-system";
    public const string VitalSignsCode = "vital-signs";
    public const string SulosFhirBaseUrl = "https://sulos.org/fhir";
    
    public static class PatientExtensions
    {
        public const string Invited = "invited";
        public const string RecordCreationDate = "recordCreationDate";
        public const string PatientStatus = "patientStatus";
        public const string CodeStatus = "codeStatus";
        public const string FirstLoginTime = "firstLoginTime";
        public const string LastLoginTime = "lastLoginTime";
        public const string About = "about";
    }
    public static class  PatientFamilyMemberExtensions
    {
        public const string FamilyMemberAccessType = "access-type";
        
    }

    public static class PractitionerExtensions
    {
        public const string LastLoginTime = "lastLoginTime";
        public const string RoleExtensionType = "role";
        public const string RoleTypeExtensionType = "roleType";
        public const string ProfileExtensionType = "profile";
        public const string StatusExtensionType = "status";
    }

    public static class RequestExtensions
    {
        public const string FlagStatus = "flagStatus";
    }

    public static class DeviceExtensions
    {
        public const string AccessToken = "accessToken";
        public const string RefreshToken = "refreshToken";
        public const string FitbitSubscriptionId = "fitbitSubscriptionId";
        public const string ConnectionStatus = "connectionStatus";
    }

    public static class QuestionnaireResponseExtensions
    {
        public const string AnswerText = "answerText";
    }

    public static class BasicResourceUsages
    {
        public const string OngoingSymptomSystem = "ongoing-symptom-system";
        public const string SymptomThresholdSystem = "symptom-threshold-system";
        public const string PatientRequestSystem = "patient-request-system";
        public const string ActivityLogSystem = "activity-log-system";
    }

    public static class Notes
    {
        public const string NotesRequestSystem = "notes-request-system";
        public const string NotesText = "notes";
    }

    public static class Scopes
    {
        public const string AdminPortal = "admin_portal";
        public const string Caregiver = "caregiver";
        public const string CareTeam = "care_team";
    }

    public static class Claims
    {
        public const string Subject = "sub";
        public const string Scope = "scp";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
        public const string Name = "name";
        public const string Email = "email";
        public const string OrganizationId = "extension_OrganizationID";
        public const string Role = "extension_Role";
        public const string Permission = "extension_Permission";
        public const string RoleType = "extension_RoleType";
        public const string FhirId = "extension_FhirID";
        public const string Profile = "extension_Profile";

        public static class RoleValues
        {
            public const string Patient = "Patient";
            public const string Executives = "Executives";
            public const string CareTeamMember = "CareTeamMember";
            public const string PatientFamiyMember = "PatientFamilyMember";

            public static readonly string[] InternalUserRoles =
                { Executives, CareTeamMember };

            public static readonly string[] ExternalUserRoles =
                { PatientFamiyMember };
        }       
    }
    
    public static class Defaults
    {
        public const string UnknownRoleTypeDisplayName = "Care Team";
        public const string UnknownRequestCategory = "Unknown Request Type";
        public const string UnknownPatientName = "Patient";
    }
    
    public static class AutoResponder
    {
        public const string FullName = "Auto Responder";
        public const string UserPrefix = "AutoResponder_";                      
        public const string Role = Defaults.UnknownRoleTypeDisplayName;    // Not an actual role, only used in the notifications

        public const string AutoResponseText =
            "Greetings,\r\n\n" +
            "Thank you for your message. If you don't hear back immediately, the team will respond the next business day.\r\n\n" +
            "For emergencies, please contact your provider directly as soon as possible.";
    }

    public static class CaregiverNotificationMessage
    {
        public const string ResolvedSymptomMessageTemplate = "The submitted symptom for {0} {1} has been addressed by the Care Team.";
    }

    public static class Devices
    {
        public const string Fitbit = "fitbit";
        public const string HealthKit = "healthkit";
        public const string Mobile = "mobile";

        public static bool IsSupported(string system)
        {
            string[] supportedDevices = { Fitbit, HealthKit, Mobile };

            return supportedDevices.Any(sys => sys == system);
        }
    }

    public static class ReplyEmail
    {
        public const string CareTeamMember = "Care Team Member";
    }
}