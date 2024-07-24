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
    //private readonly IDnsHealthResolver _healthResolver;
    public string HospiceId { get; }
    private int MaxBundleEntryCount { get; }

    public SulosFhirClient(string hospiceId, ClientCredentialsOptions options,
        IOptions<FhirConfigOptions> fhirConfigOptions) : base(options.Resource, CreateSettings())
    {
        //_healthResolver = healthResolver;
        HospiceId = hospiceId;
        MaxBundleEntryCount = fhirConfigOptions.Value.MaxBundleEntryCount;
    }

    //public async Task<HealthCheckResult> HealthCheckAsync()
    //{
    //    var status = await _healthResolver.ResolveStatusAsync(Endpoint.ToString());
    //    return status switch
    //    {
    //        HealthStatus.Healthy => HealthCheckResult.Healthy($"{HospiceId} FHIR service is available"),
    //        _ => HealthCheckResult.Unhealthy($"{HospiceId} FHIR service is unavailable")
    //    };
    //}

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

    public async Task EnsurePatientDoesNotHaveDevice(string patientId)
    {
        var searchParams = new FhirSearchParamsBuilder()
            .WhereReference<Patient>("patient", patientId)
            .Build();

        var deviceBundle = await SearchAsync<Device>(searchParams)
            .ConfigureAwait(false);

        if (deviceBundle.Entry.Count > 0)
        {
            throw new PatientHasDeviceException();
        }
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