using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User {
  [BsonId]
  public ObjectId Id { get; set; }

  [BsonElement("fullName")]
  public string FullName { get; set; }

  [BsonElement("email")]
  public string Email { get; set; }

  [BsonElement("deliveryAddresses")]
  public List<string> DeliveryAddresses { get; set; }

  [BsonElement("card")]
  public Card Card { get; set; }
}
