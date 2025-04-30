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
        return Results.NotFound($"Restaurante con id {restaurantId} no encontrado.");

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
    if (updatedRestaurant.AverageRating != 0) // Cambiar si puede ser 0 válido
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
        return Results.BadRequest("No hay campos válidos para actualizar.");

    var updateDef = Builders<Restaurant>.Update.Combine(updates);

    var result = await restaurantsCollection.UpdateOneAsync(filter, updateDef);

    if (result.MatchedCount == 0)
        return Results.NotFound($"Restaurante con id {name} no encontrado.");

    return Results.Ok($"Restaurante con id {name} actualizado correctamente.");
});

//- Añadir tarjeta a un usuario
//- Añadir artículo a carrito (actualizar el total a pagar)
//- Cambiar estado de la orden
//DELETE:
//- Quitar trajeta
//- Eliminar una orden
//
app.Run();
