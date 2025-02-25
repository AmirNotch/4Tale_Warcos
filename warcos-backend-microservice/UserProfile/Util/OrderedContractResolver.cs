using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace UserProfile.Util;

public class OrderedContractResolver : CamelCasePropertyNamesContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        return base.CreateProperties(type, memberSerialization).OrderBy(p => p.Order).ThenBy(p=>p.PropertyName).ToList();
    }
}