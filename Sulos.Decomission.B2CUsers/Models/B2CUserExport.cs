namespace Sulos.Decomission.B2CUsers.Models;

public class B2CUserExport
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FhirId { get; set; } = "";
    public string Role { get; set; } = "";
    public string RoleType { get; set; } = "";
    public string Profile { get; set; } = "";
    public string OrganizationId { get; set; } = "";
    public string[] EmailAddresses { get; set; }
}