using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Restaurant {
    [BsonId]
    public string Id {get; set;}

    [BsonElement("Nombre")]
    public string Name {get; set;}

    [BsonElement("Calificación")]
    public int AverageRating {get; set;}

    [BsonElement("direccion")]
    public string Location { get; set; }

    [BsonElement("Foto_ubicación"), BsonRepresentation(BsonType.ObjectId)]
    public string PhotoLocationId { get; set; }

    [BsonElement("Foto_referencia"), BsonRepresentation(BsonType.ObjectId)]
    public string PhotoReferenceId { get; set; }

    [BsonElement("Estilo")]
    public List<string> Styles {get; set;}

    [BsonElement("Hora_apertura")]
    public String? OpeningTime {get; set;}

    [BsonElement("Hora_cierre")]
    public String? ClosingTime {get; set;}

    [BsonElement("List_Reseña")]
    public List<Review> Reviews {get; set;}

    [BsonElement("ubicacion")]
    public GeoJsonPoint Ubication {get; set;}
}

public class GeoJsonPoint {
    [BsonElement("type")]
    public string Type { get; set; } = "Point";

    [BsonElement("coordinates")]
    public double[] Coordinates { get; set; } = new double[2];
}
