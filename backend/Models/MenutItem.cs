using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class MenuItem {
  [BsonId]
  public ObjectId Id { get; set; }

  [BsonElement("name")]
  public string Name { get; set; }

  [BsonElement("basePrice")]
  public double BasePrice { get; set; }

  [BsonElement("totalPrice")]
  public double TotalPrice { get; set; }

  [BsonElement("ingredients")]
  public List<string> Ingredients { get; set; }

  [BsonElement("rating")]
  public int Rating { get; set; }

  [BsonElement("description")]
  public string Description { get; set; }

  [BsonElement("discount")]
  public double Discount { get; set; }

  [BsonElement("inSeason")]
  public bool InSeason { get; set; }

  [BsonElement("photoItemId"), BsonRepresentation(BsonType.ObjectId)]
  public string PhotoItemId { get; set; }

  [BsonElement("type")]
  public string Type { get; set; }

  [BsonElement("restaurantId"), BsonRepresentation(BsonType.ObjectId)]
  public string RestaurantId { get; set; }
}
