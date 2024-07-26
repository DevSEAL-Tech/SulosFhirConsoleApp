namespace Sulos.Hospice.Care.Models.Common;

public record SulosAddressModel
(
    string?[] AddressLines,
    string? City,
    string? State,
    string? PostalCode 
);