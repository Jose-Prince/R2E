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

app.MapGet("/", () => "Hello World!");


//ENDPOINTS:
//CREATE:
//- Crear un pedido
app.MapPost("/orders", async (Order newOrder) =>
{
    var ordersCollection = db.GetCollection<Order>("orders");
    await ordersCollection.InsertOneAsync(newOrder);
    return Results.Created($"/orders/{newOrder.Id}", newOrder);
});

//- Crear reseña asociada 
app.MapPost("/restaurants/{restaurantId}/reviews", async (string restaurantId, Review newReview) =>
{
    var restaurantsCollection = db.GetCollection<Restaurant>("restaurants");
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

//READ:
// Obetener lista de restaurantes
app.MapGet("/restaurants",async () => 
{
    var restaurantsCollection = db.GetCollection<Restaurant>("Restaurants");
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
//- Añadir tarjeta a un usuario
//- Añadir artículo a carrito (actualizar el total a pagar)
//- Cambiar estado de la orden
//DELETE:
//- Quitar trajeta
//- Eliminar una orden
//
app.Run();
