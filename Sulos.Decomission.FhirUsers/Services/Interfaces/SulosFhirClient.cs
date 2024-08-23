using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Options;
using Sulos.Hospice.Care.Core.Common.Azure;
using Sulos.Hospice.Care.Core.Common.Exceptions;
using Sulos.Hospice.Care.Core.Common.Fhir.Searching;
using Task = System.Threading.Tasks.Task;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public class SulosFhirClient : FhirClient
{

    public string HospiceId { get; }
    private int MaxBundleEntryCount { get; }

    public SulosFhirClient(string hospiceId, ClientCredentialsOptions options, HttpMessageHandler handler,IOptions<FhirConfigOptions> fhirConfigOptions) : base(options.Resource, CreateSettings(), handler)
    {
        HospiceId = hospiceId;
        MaxBundleEntryCount = fhirConfigOptions.Value.MaxBundleEntryCount;
    }

    
    public async Task<T> ReadById<T>(string id) where T : Resource =>
        await ReadAsync<T>($"{typeof(T).Name}/{id}").ConfigureAwait(false);

    public async Task<T[]> ReadAllByReference<T>(ResourceReference[] references)
        where T : Resource
    {
        var ids = references.Where(r => r.Type == typeof(T).Name)
            .Select(r => r.GetId())
            .ToArray();
        return await ReadAllById<T>(ids);
    }

    public async Task<T[]> ReadAllById<T>(string[] ids) where T : Resource
    {
        var tasks = ids.Distinct().Select(ReadById<T>);
        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public async Task<T> EnsureEntityExists<T>(string id)
        where T : Resource
    {
        try
        {
            return await ReadById<T>(id).ConfigureAwait(false);
        }
        catch (FhirOperationException e)
        {
            throw new EntityNotFoundException<T>(id, e);
        }
    }

    public async Task<T?> SearchByIdentifier<T>(string system, string value)
        where T : Resource, new()
    {
        var bundle = await SearchAsync<T>(new FhirSearchParamsBuilder().WhereIdentifier(system, value).Build());
        return bundle.GetResources().Cast<T>().FirstOrDefault();
    }

    

    public async Task<Bundle[]> GetPaginatedQueryResponses(Bundle query)
    {
        var tasks = query.Entry
            .Chunk(MaxBundleEntryCount)
            .Select(chunk =>
            {
                var bundle = new Bundle();
                bundle.Entry.AddRange(chunk);
                bundle.Type = query.Type;
                return TransactionAsync(bundle);
            });

        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private static FhirClientSettings CreateSettings()
    {
        var settings = FhirClientSettings.CreateDefault();
        settings.PreferredFormat = ResourceFormat.Json;
        return settings;
    }
}