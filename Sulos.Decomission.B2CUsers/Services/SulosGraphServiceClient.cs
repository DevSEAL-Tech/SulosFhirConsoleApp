using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Sulos.Decomission.B2CUsers.Services.Exceptions;
using Sulos.Decomission.B2CUsers.Services.Extensions;
using Sulos.Decomission.B2CUsers.Services.Interfaces;

namespace Sulos.Decomission.B2CUsers.Services;

public class SulosGraphServiceClient : ISulosGraphServiceClient
{
    private readonly GraphServiceClient _client;
    private readonly ILogger<SulosGraphServiceClient> _logger;
    private readonly string _orgIdKey;
    private readonly string _fhirIdKey;
    private readonly string _roleKey;
    private readonly string _roleTypeKey;
    private readonly string _profileKey;

    public SulosGraphServiceClient(GraphServiceClient client,
        string rawExtensionApplicationId,
        ILogger<SulosGraphServiceClient> logger
    )
    {
        var graphServiceExtensions = new GraphServiceExtensions(rawExtensionApplicationId);
        _client = client;
        _logger = logger;

        _orgIdKey = graphServiceExtensions[GraphServiceExtensionAttributes.OrganizationID];
        _fhirIdKey = graphServiceExtensions[GraphServiceExtensionAttributes.FhirID];
        _roleKey = graphServiceExtensions[GraphServiceExtensionAttributes.Role];
        _roleTypeKey = graphServiceExtensions[GraphServiceExtensionAttributes.RoleType];
        _profileKey = graphServiceExtensions[GraphServiceExtensionAttributes.Profile];
    }

    public async Task<IEnumerable<User>> GetAllUsersInCurrentOrganisation(string organizationId)
    {
        var users = new List<User>();
        try
        {
            var userResponse = await _client.Users.GetAsync(config =>
            {
                config.QueryParameters.Filter = BuildFhirOrgFilter(organizationId);
                config.QueryParameters.Select = SelectQueryParameters;
            });

            if (userResponse == null)
            {
                return users;
            }

            var pageIterator = PageIterator<User, UserCollectionResponse>.CreatePageIterator(
                _client,
                userResponse,
                user =>
                {
                    users.Add(user);
                    return true;
                },
                req => req
            );

            await pageIterator.IterateAsync();
        }
        catch (AuthenticationFailedException ex)
        {
            _logger.LogError(ex, "B2C Graph API authentication failure");
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "B2C Graph API failure");
        }

        return users;
    }

    public async Task<User> GetUserByOrganizationAndFhirId(string organizationId,string fhirId, CancellationToken cancellationToken)
    {
        try
        {
            var searchResults = await GetUsersByOrganizationAndFhirId(organizationId,fhirId, cancellationToken);
            if (searchResults is { Value.Count: 1 })
            {
                return searchResults.Value[0];
            }

            _logger.LogError("Could not find single user for id: {FhirId}", fhirId);
            throw new GraphServiceException($"Could not find single user for id: {fhirId}");
        }
        catch (AuthenticationFailedException e)
        {
            _logger.LogError(e, "B2C Graph API authentication failure");
            throw new GraphServiceException(e.Message);
        }
        catch (ServiceException e)
        {
            _logger.LogError(e, "B2C Graph API failure");
            throw new GraphServiceException(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error {Message}", e.Message);
            throw new GraphServiceException(e.Message);
        }
    }

    public async Task DeleteUserByOrganizationAndFhirId(string organizationId, string fhirId, CancellationToken cancellationToken)
    {
        try
        {
            var userToDelete = await GetUserByOrganizationAndFhirId(organizationId, fhirId, cancellationToken);

            await DeleteUserByUserId(userToDelete.Id!, cancellationToken);         
        }
        catch (AuthenticationFailedException e)
        {
            _logger.LogError(e, "B2C Graph API authentication failure");
            throw new GraphServiceException(e.Message);
        }
        catch (ServiceException e)
        {
            _logger.LogError(e, "B2C Graph API failure");
            throw new GraphServiceException(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error {Message}", e.Message);
            throw new GraphServiceException(e.Message);
        }
    }

    public async Task DeleteUserByUserId(string userId, CancellationToken cancellationToken)
    {
        try
        {
            await _client.Users[userId].DeleteAsync(cancellationToken: cancellationToken);
        }
        catch (AuthenticationFailedException e)
        {
            _logger.LogError(e, "B2C Graph API authentication failure");
            throw new GraphServiceException(e.Message);
        }
        catch (ServiceException e)
        {
            _logger.LogError(e, "B2C Graph API failure");
            throw new GraphServiceException(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error {Message}", e.Message);
            throw new GraphServiceException(e.Message);
        }
    }

    private async Task<UserCollectionResponse?> GetUsersByOrganizationAndFhirId(
        string organizationId,
        string fhirId,
        CancellationToken cancellationToken) =>
        await _client.Users.GetAsync(config =>
            {
                config.QueryParameters.Filter = BuildFhirOrgAndFhirIdFilter(organizationId, fhirId);
                config.QueryParameters.Select = SelectQueryParameters;
            },
            cancellationToken
        );

    private string[] SelectQueryParameters =>
    [
        "id",
        "displayName",
        "accountEnabled",
        "identities",
        "givenName",
        "surname",
        _orgIdKey,
        _fhirIdKey,
        _roleKey,
        _roleTypeKey,
        _profileKey
    ];

    private string BuildFhirOrgAndFhirIdFilter(string organizationId,string fhirId) =>
        $"{BuildFhirOrgFilter(organizationId)} and {_fhirIdKey} eq '{fhirId}'";

    private string BuildFhirOrgFilter(string organizationId) =>
        $"{_orgIdKey} eq '{organizationId}'";
}
