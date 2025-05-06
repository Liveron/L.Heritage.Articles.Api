using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace L.Heritage.Articles.Infrastructure.Serializers;

internal class JsonElementToBsonSerializer : IBsonSerializer<JsonElement>
{
    public Type ValueType => typeof(JsonElement);

    public JsonElement Deserialize(BsonDeserializationContext context, BsonDeserializationArgs _)
    {
        var bsonDocument = BsonDocumentSerializer.Instance.Deserialize(context);
        var json = bsonDocument.ToJson();
        using var jsonDocument = JsonDocument.Parse(json);
        return jsonDocument.RootElement.Clone();
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs _, JsonElement value)
    {
        var json = value.GetRawText();
        var document = BsonDocument.Parse(json);
        BsonDocumentSerializer.Instance.Serialize(context, document);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value) =>
        Serialize(context, args, (JsonElement)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) =>
        Deserialize(context, args);
}
