using MongoDB.Bson.Serialization.Attributes;

public class Card
{
    [BsonElement("Cvv")]
    public int Cvv { get; set; }

    [BsonElement("Numeracion")]
    public long Numeracion { get; set; }

    [BsonElement("Expiración")]
    public DateTime Expiracion { get; set; }
}
