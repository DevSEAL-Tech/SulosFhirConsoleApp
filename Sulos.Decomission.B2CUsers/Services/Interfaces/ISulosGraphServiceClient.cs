using Microsoft.Graph.Models;

namespace Sulos.Decomission.B2CUsers.Services.Interfaces;

public interface ISulosGraphServiceClient
{
    string OrganizationId { get; }
    Task<User> GetUserByFhirId(string fhirId, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllUsersInCurrentOrganisation();
}
