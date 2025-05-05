using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Review
{
    [BsonId]
    public string Id { get; set; }

    [BsonElement("Restaurante")]
    public string RestaurantId { get; set; }

    [BsonElement("Cliente")]

    public string Cliente { get; set; }

    [BsonElement("Calificación")]
    public int Calificación { get; set; }

    [BsonElement("Comentario")]
    public string Comentario { get; set; }
}


