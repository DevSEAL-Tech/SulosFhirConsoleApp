using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Support;
using Hl7.Fhir.Utility;
using Task = Hl7.Fhir.Model.Task;
using Sulos.Hospice.Care.Models.Patients;
using static Hl7.Fhir.Model.DataRequirement;

namespace Sulos.Hospice.Care.Core.Common.Fhir.Searching;

public class FhirSearchParamsBuilder
{
    private const int DefaultCount = 1000;
    private SearchParams _params = CreateInitialParams();

    public FhirSearchParamsBuilder Where(string property, string value)
    {
        _params.Where($"{property}={value}");
        return this;
    }

    public FhirSearchParamsBuilder WhereDateRange(string property, DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue)
        {
            _params.Add($"{property}", $"ge{startDate.Value:yyyy-MM-dd}");
        }

        if (endDate.HasValue)
        {
            _params.Add($"{property}", $"le{endDate.Value:yyyy-MM-dd}");
        }

        return this;
    }

    public FhirSearchParamsBuilder WherePropertyNull(string property, string? value)
    {
        _params.Where($"{property}={value}");
        return this;
    }

    public FhirSearchParamsBuilder WhereReference<T>(string property, string id)
        where T : Resource
    {
        return Where(property, FhirReference<T>.Value(id));
    }

    public FhirSearchParamsBuilder WhereIdentifier(string system, string value)
    {
        _params.Where($"identifier={system}|{value}");
        return this;
    }

    public FhirSearchParamsBuilder WhereIdentifiers(Dictionary<string, string> identifiers)
    {
        var paramString = "identifier=";
        var idx = 0;
        foreach ((string system, string value) in identifiers)
        {
            var termination =
                idx++ >= identifiers.Count - 1
                    ? ""
                    : ",";
            paramString += $"{system}|{value}{termination}";
        }

        _params.Where(paramString);
        return this;
    }

    public FhirSearchParamsBuilder WhereLastUpdatedAfter(DateTimeOffset date)
    {
        return Where(FhirConstants.MetadataProperties.LastUpdated, $"ge{date.ToFhirTimestamp()}");
    }

    public FhirSearchParamsBuilder WhereLastUpdatedBefore(DateTimeOffset date)
    {
        return Where(FhirConstants.MetadataProperties.LastUpdated, $"le{date.ToFhirTimestamp()}");
    }

    public FhirSearchParamsBuilder WhereCategory(string category)
    {
        _params.Where($"category={category}");
        return this;
    }

    public FhirSearchParamsBuilder WhereVitalSign(VitalSignKey? key)
    {
        if (key == null)
            return this;

        _params.Where($"combo-code={key}");
        return this;
    }

    public FhirSearchParamsBuilder WhereSubject<T>(string subjectId)
    {
        _params.Where($"subject={typeof(T).Name}/{subjectId}");
        return this;
    }

    public FhirSearchParamsBuilder WhereDateAfter(DateTimeOffset? date)
    {

        return Where("date", $"ge{date.ToFhirDateTime()}");
    }

    public FhirSearchParamsBuilder WhereDateBefore(DateTimeOffset date)
    {
        return Where("date", $"le{date.ToFhirTimestamp()}");
    }

    public FhirSearchParamsBuilder WithCount(int count)
    {
        _params.Count = count;
        return this;
    }

    public FhirSearchParamsBuilder SortByLastUpdated(SortDirection direction = SortDirection.Descending)
    {
        return SortBy(FhirConstants.MetadataProperties.LastUpdated, direction);
    }

    public FhirSearchParamsBuilder FilterByOpenStatus()
    {
        _params.Where($"status={Task.TaskStatus.Requested}");
        _params.Where($"status={Task.TaskStatus.InProgress}");
        return this;
    }

    public FhirSearchParamsBuilder SortByDate(SortDirection direction)
    {
        return SortBy("date", direction);
    }

    public FhirSearchParamsBuilder SortBy(string property, SortDirection direction)
    {
        _params.Sort.Add((property, direction.ToFhirSortOrder()));
        return this;
    }

    public FhirSearchParamsBuilder WithEmail(string email)
    {
        return Where("email", email);
    }

    public FhirSearchParamsBuilder WithNullPhone(string? phone)
    {
        return WherePropertyNull("phone", phone);
    }

    public FhirSearchParamsBuilder WithNullableEmail(string? email)
    {
        return WherePropertyNull("email", email);
    }

    public FhirSearchParamsBuilder WhereGroupType(Group.GroupType type)
    {
        return Where("type", type.ToString().ToLowerInvariant());
    }

    public FhirSearchParamsBuilder WhereGroupMember<T>(T resource)
        where T : DomainResource
    {
        return WhereGroupMember<T>(resource.Id);
    }

    public FhirSearchParamsBuilder WhereGroupMember<T>(string id)
        where T : DomainResource
    {
        return Where("member", new FhirReference<T>(id).Reference);
    }

    public FhirSearchParamsBuilder WhereUrl(string url)
    {
        return Where("url", url);
    }

    public FhirSearchParamsBuilder WhereTypes(IEnumerable<ResourceType> types)
    {
        _params.Add("_type", string.Join(",", types.Select(type => type.GetLiteral())));
        return this;
    }

    public FhirSearchParamsBuilder WhereId(string id)
    {
        _params.Add("_id", id);
        return this;
    }

    public FhirSearchParamsBuilder WhereAuthoredDateRange(DateTime? startDate, DateTime? endDate)
    {
        return WhereDateRange(FhirConstants.MetadataProperties.Authored, startDate, endDate);
    }

    public FhirSearchParamsBuilder WhereAuthoredOnDateRange(DateTime? startDate, DateTime? endDate)
    {
        return WhereDateRange(FhirConstants.MetadataProperties.AuthoredOn, startDate, endDate);
    }

    public FhirSearchParamsBuilder Include(string path)
    {
        _params.Include(path);
        return this;
    }

    public SearchParams Build()
    {
        try
        {
            return _params;
        }
        finally
        {
            _params = CreateInitialParams();
        }
    }

    private static SearchParams CreateInitialParams()
    {
        return new SearchParams
        {
            Count = DefaultCount
        };
    }
}