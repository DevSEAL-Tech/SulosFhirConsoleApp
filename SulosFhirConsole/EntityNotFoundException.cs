using System;
using System.Runtime.Serialization;
using Hl7.Fhir.Rest;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sulos.Hospice.Care.Core.Common.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public SearchParams? SearchParams { get; }

        public EntityNotFoundException(Type type, string id, Exception? innerException)
            : base($"Could not find entity of type {type.Name} with id {id}", innerException)
        {
        }

        public EntityNotFoundException(Type type, SearchParams searchParams)
            : base($"Could not find entity of type {type.Name} with search params")
        {
            SearchParams = searchParams;
        }

        public EntityNotFoundException(Type type)
            : base($"Could not find entity of type {type.Name}")
        {
        }

        // Custom serialization logic
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // Optional: Convert enums to camelCase
            };
            var json = new
            {
                Message = Message,
                SearchParams = SearchParams
            };
            return JsonSerializer.Serialize(json, options);
        }

        // Optional: Implementing deserialization if needed
        public static EntityNotFoundException FromJson(string json)
        {
            var options = new JsonSerializerOptions();
            var obj = JsonSerializer.Deserialize<EntityNotFoundException>(json, options);
            return obj ?? throw new JsonException("Failed to deserialize JSON to EntityNotFoundException.");
        }
    }

    public class EntityNotFoundException<T> : EntityNotFoundException
    {
        public EntityNotFoundException(string id, Exception? innerException = null)
            : base(typeof(T), id, innerException)
        {
        }

        public EntityNotFoundException(string system, string value, Exception? innerException = null)
            : base(typeof(T), $"system: {system}, value: {value}", innerException)
        {
        }

        public EntityNotFoundException(SearchParams searchParams)
            : base(typeof(T), searchParams)
        {
        }

        public EntityNotFoundException()
            : base(typeof(T))
        {
        }
    }
}


namespace Sulos.Hospice.Care.Core.Common.Exceptions
{
    public class PatientHasDeviceException : Exception
    {
        public PatientHasDeviceException() : base("Patient already has device attached.")
        { }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}