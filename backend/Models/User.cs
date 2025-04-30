using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("Nombre_y_Apellido")]
    public string NombreYApellido { get; set; }

    [BsonElement("Correo")]
    public string Correo { get; set; }

    [BsonElement("Dirección_entrega")]
    public List<string> DireccionesEntrega { get; set; }

    [BsonElement("Tarjeta")]
    public Card Tarjeta { get; set; }
}
