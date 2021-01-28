using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Appmilla.RestApiClient
{
    public class PostContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            // Filter out any properties that are decorated with the [IgnoreForSerialisation] attribute
            properties = properties.Where(p => ValidateProperty(p)).ToList();

            return properties;

            bool ValidateProperty(JsonProperty property)
            {
                try
                {
                    var declaredProperties = type.GetTypeInfo().GetDeclaredProperty(property.PropertyName);
                    if (declaredProperties != null)
                    {
                        var attributeList = declaredProperties.GetCustomAttributes<IgnoreForSerialisation>();
                        return !attributeList.Any();
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                }

                return false;
            }
        }
    }
}
