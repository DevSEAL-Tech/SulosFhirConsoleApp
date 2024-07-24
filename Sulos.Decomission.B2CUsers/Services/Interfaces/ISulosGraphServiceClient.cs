using Microsoft.Graph.Models;

namespace Sulos.Decomission.B2CUsers.Services.Interfaces;

public interface ISulosGraphServiceClient
{
    Task<User> GetUserByOrganizationAndFhirId(string organizationId,string fhirId, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllUsersInCurrentOrganisation(string organizationId);
    Task DeleteUserByOrganizationAndFhirId(string organizationId, string fhirId, CancellationToken cancellationToken);
    Task DeleteUserByUserId(string userId, CancellationToken cancellationToken);
}
