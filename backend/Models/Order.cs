using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class Order
{
    [BsonId]
    public required string Id { get; set; }

    [BsonElement("No_orden")]
    public int NoOrden { get; set; }

    [BsonElement("Timestamp")]
    public long Timestamp { get; set; }

    [BsonElement("Total_a_pagar")]
    public double TotalAPagar { get; set; }

    [BsonElement("Carrito"), BsonRepresentation(BsonType.ObjectId)]
    public required List<string> Carrito { get; set; }

    [BsonElement("Estado")]
    public int Estado { get; set; }

    [BsonElement("Cliente"), BsonRepresentation(BsonType.ObjectId)]
    public required string ClienteId { get; set; }

    [BsonElement("Notas")]
    public string? Notas { get; set; }
}
