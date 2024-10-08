﻿namespace Sulos.Decomission.Shared.Models;

public class UserExport
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
