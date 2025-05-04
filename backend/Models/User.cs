using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    public String Id { get; set; }

    [BsonElement("Nombre_y_Apellido")]
    public string Name { get; set; }

    [BsonElement("Correo")]
    public string Email { get; set; }

    [BsonElement("Dirección_entrega")]
    public List<string> Address { get; set; }

    [BsonElement("Tarjeta")]
    public Card Tarjeta { get; set; }
}
