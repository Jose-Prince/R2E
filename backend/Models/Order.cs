using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Order {
  [BsonId]
  public ObjectId Id { get; set; }

  [BsonElement("orderNumber")]
  public int OrderNumber { get; set; }

  [BsonElement("timestamp")]
  public DateTime Timestamp { get; set; }

  [BsonElement("totalAmount")]
  public double TotalAmount { get; set; }

  [BsonElement("items"), BsonRepresentation(BsonType.ObjectId)]
  public List<string> Items { get; set; }

  [BsonElement("status")]
  public int Status { get; set; }

  [BsonElement("customerId"), BsonRepresentation(BsonType.ObjectId)]
  public string CustomerId { get; set; }

  [BsonElement("notes")]
  public string Notes { get; set; }
}
