using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Review {
  [BsonId] 
  public ObjectId Id { get; set; }

  [BsonElement("customerId"), BsonRepresentation(BsonType.ObjectId)]
  public string CustomerId { get; set; }

  [BsonElement("rating")]
  public int Rating { get; set; }

  [BsonElement("comment")]
  public string Comment { get; set; }
}


