using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Appmilla.RestApiClient
{
    public class HttpGetContractResolver : DefaultContractResolver
    {
        private static JsonConverter guidNullConverter = new NullToDefaultConverter<Guid>();

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == typeof(Guid))
            {
                return guidNullConverter;
            }

            return base.ResolveContractConverter(objectType);
        }
    }
}
