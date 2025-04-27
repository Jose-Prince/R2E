using MongoDB.Bson;
using MongoDB.Driver;

public static class MongoDBConnection
{
    public static MongoClient Client { get; private set; }

    public static MongoClient Initialize(string connectionUri)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        Client = new MongoClient(settings);

        try
        {
            var result = Client.GetDatabase("R2E").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to MongoDB: {ex.Message}");
        }

        return Client;
    }
}