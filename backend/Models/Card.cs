using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Card {
    [BsonElement("number")]
    public string Number {get; set;}

    [BsonElement("cvv")]
    public int Cvv {get; set;}

    [BsonElement("expiration")]
    public DateTime Expiration {get; set;}
}