using DotNetEnv;
using MongoDB.Bson;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Env.Load();

// Create a new client and connect to the server
var mongoClient = MongoDBConnection.Initialize(Env.GetString("MONGODB_URI"));

//Get database
var db = mongoClient.GetDatabase("R2E");

// Collections
var restaurantsCollection = db.GetCollection<Restaurant>("Restaurants");
var ordersCollection = db.GetCollection<Order>("Orders");
var productsCollection = db.GetCollection<MenuItem>("Products");
var reviewCollection = db.GetCollection<Review>("Review");
var usersCollection = db.GetCollection<User>("Users");

app.MapGet("/", () => "Hello World!");


//ENDPOINTS:
//CREATE:
//- Crear un pedido
app.MapPost("/orders", async (Order newOrder) =>
{
    await ordersCollection.InsertOneAsync(newOrder);
    return Results.Created($"/orders/{newOrder.Id}", newOrder);
});

//- Crear reseña asociada 
app.MapPost("/restaurants/{restaurantId}/reviews", async (string restaurantId, Review newReview) =>
{
    // Generar nuevo Id para la reseña
    newReview.Id = ObjectId.GenerateNewId();
    // Filtro por restaurante
    var filter = Builders<Restaurant>.Filter.Eq(r => r.Id, ObjectId.Parse(restaurantId));
    // Agregar la reseña al arreglo
    var update = Builders<Restaurant>.Update.Push(r => r.Reviews, newReview);
    var result = await restaurantsCollection.UpdateOneAsync(filter, update);
    
    if (result.ModifiedCount == 0)
        return Results.NotFound($"Restaurant: {restaurantId} not found");

    return Results.Created($"/restaurants/{restaurantId}/reviews/{newReview.Id}", newReview);
});

// Crear un Restaurante
app.MapPost("/restaurants", async (Restaurant newRestaurant) => 
{
    await restaurantsCollection.InsertOneAsync(newRestaurant);
    return Results.Created($"/restaurants/{newRestaurant.Id}", newRestaurant);
});

//READ:
// Obtener lista de restaurantes
app.MapGet("/restaurants",async () => 
{
    var restaurants = await restaurantsCollection.Find(_ => true).ToListAsync();

    return Results.Ok(restaurants);
});

//- Obtener todos los usuarios
app.MapGet("/users", async () =>
{
    var users = await usersCollection.Find(_ => true).ToListAsync();
    return Results.Ok(users);
});

//- Obtener todas las ordenes
app.MapGet("/orders", async () =>
{
    var orders = await ordersCollection.Find(_ => true).ToListAsync();
    return Results.Ok(orders);
});


//- Obtener restaurantes (Nombre, Foto referencia, Calificación, size, page)
//- Obtener los diferentes tipos de comida de los restaurantes (sin repeticiones)
//- Obtener ofertas (Nombre del artículo, Precio total, precio base, nomnre de restaurante, descuento, Foto de artículo, size, page)
//- Obtener restaurante por nombre (Nombre, Foto referencia, Calificación)
//- Obtener los elementos del carrito de ordenes que esten en estado "no ordenado"
//- Obtener ordenes para un cliente en estado ordenado y en camino (no se obtiene: Cliente)
//- Obtener ordenes para un cliente en estado entregado (no se obtiene: Cliente)
//- Obtener los datos del cliente
//UPDATE:
//- Actualizar restaurante
app.MapPatch("/restaurants/{name}", async (string name, Restaurant updatedRestaurant) => 
{
    var filter = Builders<Restaurant>.Filter.Eq(r => r.Name, name);

    var updates = new List<UpdateDefinition<Restaurant>>();

    if (updatedRestaurant.Name != null)
        updates.Add(Builders<Restaurant>.Update.Set(r => r.Name, updatedRestaurant.Name));
    if (updatedRestaurant.AverageRating != 0)
        updates.Add(Builders<Restaurant>.Update.Set(r => r.AverageRating, updatedRestaurant.AverageRating));
    if (updatedRestaurant.Location != null)
        updates.Add(Builders<Restaurant>.Update.Set(r => r.Location, updatedRestaurant.Location));
    if (updatedRestaurant.OpeningTime != null)
        updates.Add(Builders<Restaurant>.Update.Set(r => r.OpeningTime, updatedRestaurant.OpeningTime));
    if (updatedRestaurant.ClosingTime != null)
        updates.Add(Builders<Restaurant>.Update.Set(r => r.ClosingTime, updatedRestaurant.ClosingTime));
    if (updatedRestaurant.Styles != null)
        updates.Add(Builders<Restaurant>.Update.Set(r => r.Styles, updatedRestaurant.Styles));
    if (updatedRestaurant.PhotoLocationId != null)
        updates.Add(Builders<Restaurant>.Update.Set(r => r.PhotoLocationId, updatedRestaurant.PhotoLocationId));
    if (updatedRestaurant.PhotoReferenceId != null)
        updates.Add(Builders<Restaurant>.Update.Set(r => r.PhotoReferenceId, updatedRestaurant.PhotoReferenceId));
    if (updatedRestaurant.Ubication?.Coordinates != null && updatedRestaurant.Ubication.Coordinates.Length == 2)
        updates.Add(Builders<Restaurant>.Update.Set(r => r.Ubication, updatedRestaurant.Ubication));

    if (!updates.Any())
        return Results.BadRequest("NO FIELDS TO UPDATE");

    var updateDef = Builders<Restaurant>.Update.Combine(updates);

    var result = await restaurantsCollection.UpdateOneAsync(filter, updateDef);

    if (result.MatchedCount == 0)
        return Results.NotFound($"Restaurant: {name} not found.");

    return Results.Ok($"Restaurant: {name}, updated.");
});

//- Añadir tarjeta a un usuario
//- Añadir artículo a carrito (actualizar el total a pagar)
//- Cambiar estado de la orden
//DELETE:
//- Borrar restaurante
app.MapDelete("restaurants/{name}", async (string name) => 
{
    var filter = Builders<Restaurant>.Filter.Eq(r => r.Name, name);

    var result = await restaurantsCollection.DeleteOneAsync(filter);

    if (result.DeletedCount == 0)
        return Results.NotFound("COULDN'T DELETE RESTAURANT (NOT FOUND)");

    return Results.Ok($"Restaurant: '{name}' deleted");
});

//- Quitar tarjeta
app.MapDelete("user/{userId}/card", async (string userId) =>
{
    var filter = Builders<User>.Filter.Eq("_id", userId);
    var update = Builders<User>.Update.Unset("Tarjeta");

    var result = await usersCollection.UpdateOneAsync(filter, update);

    if (result.ModifiedCount == 0)
        return Results.NotFound($"User with id {userId} not found or did not have a card.");
    
    return Results.Ok($"Card deleted for user {userId}.");
});

//- Eliminar una orden
app.MapDelete("/orders/{orderId}", async (string orderId) =>
{
    var filter = Builders<Order>.Filter.Eq("_id", orderId);

    var result = await ordersCollection.DeleteOneAsync(filter);

    if (result.DeletedCount == 0)
        return Results.NotFound($"Order with id {orderId} not found.");

    return Results.Ok($"Order {orderId} successfully deleted.");
});

app.Run();
