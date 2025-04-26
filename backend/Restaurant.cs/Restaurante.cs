using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Restaurant {
    [BsondId]
    public ObjectId Id {get; set;}

    [BsonElement("name")]
    public string Name {get; set;}

    [BsonElement("averageRating")]
    public int AverageRating {get; set;}

    [BsonElement("location")]
    public string Location { get; set; }

    [BsonElement("photoLocationId"), BsonRepresentation(BsonType.ObjectId)]
    public string PhotoLocationId { get; set; }

    [BsonElement("photoReferenceId"), BsonRepresentation(BsonType.ObjectId)]
    public string PhotoReferenceId { get; set; }

    [BsonElement("styles")]
    public List<string> Styles {get; set;}

    [BsonElement("openingTime")]
    public DateTime OpeningTime {get; set;}

    [BsonElement("closingTime")]
    public DateTime ClosingTime {get; set;}

    [BsonElement("reviews")]
    public List<Review> Reviews {get; set;}

}