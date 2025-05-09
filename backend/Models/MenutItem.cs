using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class MenuItem {
  [BsonId]
  public string Id { get; set; }

  [BsonElement("Nombre")]
  public required string Name { get; set; }

  [BsonElement("Precio_Base")]
  public double BasePrice { get; set; }

  [BsonElement("Precio_Total")]
  public double TotalPrice { get; set; }

  [BsonElement("Ingredientes")]
  public List<string>? Ingredients { get; set; }

  [BsonElement("Calificación")]
  public int Rating { get; set; }

  [BsonElement("Descripción")]
  public string? Description { get; set; }

  [BsonElement("Descuento")]
  public double Discount { get; set; }

  [BsonElement("Temporada")]
  public bool InSeason { get; set; }

  [BsonElement("Foto_articulo")]
  public string? PhotoItemId { get; set; }

  [BsonElement("Tipo")]
  public string? Type { get; set; }

  [BsonElement("Restaurante") ]
  public required string RestaurantId { get; set; }
}
