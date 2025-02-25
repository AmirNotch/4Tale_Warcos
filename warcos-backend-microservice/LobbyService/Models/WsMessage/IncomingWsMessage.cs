using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Lobby.Models.WsMessage;

[JsonConverter(typeof(IncomingWsMessageConverter))]
public class IncomingWsMessage 
{
    [JsonProperty("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonProperty("eventCategory")]
    public EndpointCategory EventCategory { get; set; }

    [JsonProperty("eventType")]
    public string EventType { get; set; } = "";

    [JsonProperty("data")]
    public string? Data { get; set; }
}

class IncomingWsMessageConverter : JsonConverter {
    public override bool CanConvert(Type objectType) {
        throw new NotImplementedException();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }
        var obj = JObject.Load(reader);
        var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(objectType);
        var incomingWsMessage = existingValue as IncomingWsMessage ?? (IncomingWsMessage)contract.DefaultCreator();

        var data = obj.GetValue("data", StringComparison.OrdinalIgnoreCase).RemoveFromLowestPossibleParent();
        if (data != null)
            incomingWsMessage.Data = data.ToString();

        // Populate the remaining properties.
        using (var subReader = obj.CreateReader())
        {
            serializer.Populate(subReader, incomingWsMessage);
        }
        return incomingWsMessage;
    }

    public override bool CanWrite { get { return false; } }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        throw new NotImplementedException();
    }
}

public static class JsonExtensions
{
    public static JToken? RemoveFromLowestPossibleParent(this JToken? node)
    {
        if (node == null)
        {
            return null;
        }
        var contained = node.AncestorsAndSelf().Where(t => t.Parent is JContainer && t.Parent.Type != JTokenType.Property).FirstOrDefault();
        if (contained != null)
        {
            contained.Remove();
        }
        // Also detach the node from its immediate containing property -- Remove() does not do this even though it seems like it should
        if (node.Parent is JProperty)
        {
            ((JProperty)node.Parent).Value = null;
        }
        return node;
    }
}
