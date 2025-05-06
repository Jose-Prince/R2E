using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()  
            .AllowAnyMethod()  
            .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
Env.Load();

// Create a new client and connect to the server
var mongoClient = MongoDBConnection.Initialize(Env.GetString("MONGODB_URI"));

//Get database
var db = mongoClient.GetDatabase("R2E");
var gridFS = new GridFSBucket(db);

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
app.MapPost("/orders", async (Order request) =>
{
    var newOrder = new Order
    {
        Id = ObjectId.GenerateNewId().ToString(),
        NoOrden = new Random().Next(100000, 999999), // Puedes ajustar cómo generas el número de orden
        Timestamp = DateTime.UtcNow,
        TotalAPagar = request.TotalAPagar,
        Carrito = request.Carrito,
        Estado = 1,
        ClienteId = request.ClienteId,
        Notas = string.Empty // Puedes cambiarlo si deseas un valor por defecto distinto
    };

    await ordersCollection.InsertOneAsync(newOrder);
    return Results.Created($"/orders/{newOrder.Id}", newOrder);
});

app.MapPost("/upload", async (HttpRequest request) =>
{
    var form = await request.ReadFormAsync();
    var file = form.Files["file"];

    if (file == null || file.Length == 0)
        return Results.BadRequest("File is missing or empty.");

    using var stream = file.OpenReadStream();
    var fileId = await gridFS.UploadFromStreamAsync(file.FileName, stream);

    return Results.Ok(new { id = fileId.ToString(), filename = file.FileName });
});


app.MapGet("/file/{id}", async (string id) =>
{
    if (!ObjectId.TryParse(id, out var objectId))
        return Results.BadRequest("Invalid file ID.");

    try
    {
        var stream = await gridFS.OpenDownloadStreamAsync(objectId);
        var contentType = "application/octet-stream";

        // Optional: try to infer content type from filename
        if (stream.FileInfo.Filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            contentType = "image/jpeg";
        else if (stream.FileInfo.Filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            contentType = "image/png";

        return Results.File(stream, contentType, stream.FileInfo.Filename);
    }
    catch (GridFSFileNotFoundException)
    {
        return Results.NotFound("File not found in GridFS.");
    }
});



// Utiliza Bulkwrite para insertar un documento a review y por otro lado utiliza un update para arreglar la Calificación del restaurante
app.MapPost("/reviews/{restaurantId}", async (string restaurantId, HttpRequest request) =>
{
    // Now you have the `restaurantId` from the URL path
    if (string.IsNullOrEmpty(restaurantId))
    {
        return Results.BadRequest("Missing restaurantId.");
    }

    // Read the body of the request as a JSON array of reviews
    var newReviews = await request.ReadFromJsonAsync<List<Review>>();
    if (newReviews == null || newReviews.Count == 0)
    {
        return Results.BadRequest("No reviews provided.");
    }

    // You can use the `restaurantId` directly now in the query
    var restaurant = await restaurantsCollection.Find(r => r.Id == restaurantId).FirstOrDefaultAsync();
    if (restaurant == null)
    {
        return Results.NotFound("Restaurant not found.");
    }

    // Generate new IDs and add the restaurantId to the reviews
    foreach (var review in newReviews)
    {
        review.Id = ObjectId.GenerateNewId().ToString();
        review.RestaurantId = restaurantId;
    }

    // Insert the reviews
    await reviewCollection.InsertManyAsync(newReviews);

    // Append the new reviews to the embedded list in the restaurant
    restaurant.Reviews ??= new List<Review>();
    restaurant.Reviews.AddRange(newReviews);

    // Recalculate the average rating
    restaurant.AverageRating = (int)Math.Round(restaurant.Reviews.Average(r => r.Calificación));

    // Prepare to update the restaurant document in bulk
    var updateModel = new ReplaceOneModel<Restaurant>(
        Builders<Restaurant>.Filter.Eq("Id", restaurantId),
        restaurant
    );

    var result = await restaurantsCollection.BulkWriteAsync(new[] { updateModel });

    return Results.Ok(new
    {
        success = result.ModifiedCount > 0,
        newAverageRating = restaurant.AverageRating,
        reviews = newReviews
    });
});

//BULK WRITE PARA AGREGAR PRODUCTOS A LA COLECCIÓN Productos
app.MapPost("/products/", async (HttpRequest request) =>
{
    var products = await request.ReadFromJsonAsync<List<MenuItem>>();
    if (products == null || products.Count == 0)
    {
        return Results.BadRequest("No products provided.");
    }

    // Assign ObjectIds and create InsertOneModel for each product
    var writeModels = new List<WriteModel<MenuItem>>();
    foreach (var product in products)
    {
        product.Id = ObjectId.GenerateNewId().ToString();
        writeModels.Add(new InsertOneModel<MenuItem>(product));
    }

    await productsCollection.BulkWriteAsync(writeModels);

    return Results.Ok(new
    {
        inserted = products.Count,
        products
    });
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

//- Obtener usuario por id
app.MapGet("/user/{id}", async (string id) =>
{
    var filter = Builders<User>.Filter.Eq(u => u.Id, id);
    var user = await usersCollection.Find(filter).FirstOrDefaultAsync();

    if (user is null)
        return Results.NotFound($"Usuario con ID '{id}' no encontrado.");

    return Results.Ok(user);
});

//- Obtener reviews
app.MapGet("/reviews", async () =>
{
    var reviews = await reviewCollection.Find(_ => true).ToListAsync();
    return Results.Ok(reviews);
});

//- Obtener reviews por Restaurante
app.MapGet("/reviews/restaurant/{restaurantId}", async (string restaurantId) =>
{
    if (string.IsNullOrEmpty(restaurantId))
    {
        return Results.BadRequest("Missing restaurantId.");
    }

    var reviews = await reviewCollection.Find(r => r.RestaurantId == restaurantId).ToListAsync();

    if (reviews == null || reviews.Count == 0)
    {
        return Results.NotFound($"No reviews found for restaurant with id {restaurantId}.");
    }

    return Results.Ok(reviews);
});

// Obtener los diferentes tipos de comida sin repeticiones
app.MapGet("/restaurants/estilos", async () =>
{
    var pipeline = new[]
    {
        new BsonDocument("$unwind", "$Estilo"),
        new BsonDocument("$group", new BsonDocument("_id", "$Estilo")),
        new BsonDocument("$sort", new BsonDocument("_id", 1))
    };

    var result = await restaurantsCollection.AggregateAsync<BsonDocument>(pipeline);

    var estilos = await result.ToListAsync();

    var lista = estilos.Select(e => e["_id"].AsString).ToList();

    return Results.Ok(lista);
});

//- Obtener ofertas (Nombre del artículo, Precio total, precio base, nomnre de restaurante, descuento, Foto de artículo, size, page)
app.MapGet("/sales", async () =>
{
    // Define un filtro para encontrar documentos donde Descuento es mayor que 0
    var filter = Builders<MenuItem>.Filter.Gt(x => x.Discount, 0);

    // Ejecuta la consulta con el filtro
    var discountedProducts = await productsCollection.Find(filter).ToListAsync();

    return Results.Ok(discountedProducts);
});

// Obtener nombre restaurante por Id
app.MapGet("/restaurants/id/{id}", async (string id) =>
{
    var restaurant = await restaurantsCollection.Find(r => r.Id == id).FirstOrDefaultAsync();

    if (restaurant == null)
    {
        return Results.NotFound($"No se encontró ningún restaurante con el ID: {id}");
    }

    return Results.Ok(new { Nombre = restaurant.Name });
});

// Obtener restaurante por nombre (Nombre, Foto_referencia, Calificación)
app.MapGet("/restaurants/nombre/{nombre}", async (string nombre) =>
{
    var filter = Builders<Restaurant>.Filter.Eq("Nombre", nombre);

    var projection = Builders<Restaurant>.Projection
        .Include("Nombre")
        .Include("Foto_referencia")
        .Include("Calificación")
        .Exclude("_id");

    var bson = await restaurantsCollection
        .Find(filter)
        .Project<BsonDocument>(projection)
        .FirstOrDefaultAsync();

    if (bson == null)
        return Results.NotFound($"No se encontró restaurante con nombre: {nombre}");

    var dict = bson.ToDictionary();

    return Results.Ok(dict);
});

// Obtener productos del carrito de órdenes no ordenadas (Estado = 0)
// no pude

//- Obtener todas las ordenes segun estado
app.MapGet("/orders", async (HttpRequest request) =>
{
    var estados = request.Query["estado"].Select(e => int.TryParse(e, out var val) ? val : (int?)null)
                                        .Where(e => e.HasValue)
                                        .Select(e => e!.Value)
                                        .ToList();

    FilterDefinition<Order> filter;

    if (estados.Any())
    {
        filter = Builders<Order>.Filter.In(o => o.Estado, estados);
    }
    else
    {
        filter = Builders<Order>.Filter.Empty;
    }

    var orders = await ordersCollection.Find(filter).ToListAsync();
    return Results.Ok(orders);
});


//  To get la cuenta de los estados ordenados
app.MapGet("/orders/count/{estado:int}", async (int estado) =>
{
    var pipeline = new BsonDocument[]
    {
        new BsonDocument("$match", new BsonDocument("Estado", estado)),
        new BsonDocument("$count", "Count")
    };

    var result = await ordersCollection.AggregateAsync<BsonDocument>(pipeline);
    var document = await result.FirstOrDefaultAsync();

    var count = document?["Count"].AsInt32 ?? 0;

    return Results.Ok(new { Estado = estado, Count = count });
});



// To get los ingredientes y que sean unicos.
app.MapGet("/products/ingredients", async () =>
{
    var pipeline = new BsonDocument[]
    {
        new BsonDocument("$unwind", "$Ingredientes"),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", BsonNull.Value },
            { "allIngredientes", new BsonDocument("$addToSet", "$Ingredientes") } // Collect unique ingredients
        }),
        new BsonDocument("$project", new BsonDocument
        {
            { "ingredientes_", "$allIngredientes" }, // Rename 'allIngredientes' to 'ingredientes_'
            { "_id", 0 } // Exclude _id from the final result
        })
    };

    var result = await productsCollection.AggregateAsync<BsonDocument>(pipeline);
    var document = await result.FirstOrDefaultAsync();

    var ingredientes = document?["ingredientes_"].AsBsonArray.Select(x => x.AsString).ToList() ?? new List<string>();

    return Results.Ok(new { Ingredientes = ingredientes });
});


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
// Añadir o actualizar tarjeta de un usuario
app.MapPatch("/user/{userId}/card", async (string userId, Card nuevaTarjeta) =>
{
    var filter = Builders<User>.Filter.Eq("_id", userId);
    var update = Builders<User>.Update.Set("Tarjeta", nuevaTarjeta);

    var result = await usersCollection.UpdateOneAsync(filter, update);

    if (result.MatchedCount == 0)
        return Results.NotFound($"User with id {userId} not found.");

    return Results.Ok($"Card updated for user {userId}.");
});

//- Eliminar artículo a carrito (actualizar el total a pagar)
app.MapDelete("/user/{userId}", async (string userId) =>
{
    var filter = Builders<User>.Filter.Eq("_id", userId);
    var result = await usersCollection.DeleteOneAsync(filter);

    if (result.DeletedCount == 0)
        return Results.NotFound($"User with id {userId} not found.");

    return Results.Ok($"User with id {userId} deleted.");
});


//- Eliminar USUARIOS

app.MapDelete("/users/batch", async (HttpRequest request) =>
{
    var userIds = await request.ReadFromJsonAsync<List<string>>();
    if (userIds == null || userIds.Count == 0)
        return Results.BadRequest("No user IDs provided.");

    // Filter to match the user IDs as strings
    var filter = Builders<User>.Filter.In("_id", userIds);

    var result = await usersCollection.DeleteManyAsync(filter);

    return Results.Ok($"{result.DeletedCount} users deleted.");
});


// Cambiar estado de una orden
app.MapPatch("/orders/{orderId}/status", async (string orderId, EstadoWrapper body) =>
{
    var filter = Builders<Order>.Filter.Eq("_id", orderId);
    var update = Builders<Order>.Update.Set("Estado", body.Estado);

    var result = await ordersCollection.UpdateOneAsync(filter, update);

    if (result.MatchedCount == 0)
        return Results.NotFound($"Order with id {orderId} not found.");

    return Results.Ok($"Estado de la orden {orderId} actualizado a {body.Estado}.");
});

    app.MapPatch("/review/{reviewId}", async (string reviewId, Review update) =>
    {
        var filter = Builders<Review>.Filter.Eq("_id", ObjectId.Parse(reviewId));
        var updateDef = Builders<Review>.Update
            .Set("Calificación", update.Calificación)
            .Set("Comentario", update.Comentario);
        var result = await reviewCollection.UpdateOneAsync(filter, updateDef);

        if (result.MatchedCount == 0)
            return Results.NotFound($"Review with id {reviewId} not found.");

        return Results.Ok($"Review {reviewId} updated with new Calificación and Comentario.");
    });

app.MapPatch("/reviews", async ([FromBody] List<Review> reviews) =>
{
    // Check if the body is null or empty
    if (reviews == null || !reviews.Any())
    {
        return Results.BadRequest("Invalid input. Please provide a list of reviews.");
    }

    var updateRequests = new List<WriteModel<Review>>();

    // Iterate through the list of reviews to prepare the bulk write
    foreach (var review in reviews)
    {
        if (string.IsNullOrEmpty(review.Id))
            continue; // Skip invalid or missing IDs

        var filter = Builders<Review>.Filter.Eq(r => r.Id, review.Id);

        var updateDef = Builders<Review>.Update
            .Set(r => r.Calificación, review.Calificación)
            .Set(r => r.Comentario, review.Comentario);

        updateRequests.Add(new UpdateOneModel<Review>(filter, updateDef));
    }

    // If no valid reviews to update
    if (!updateRequests.Any())
        return Results.BadRequest("No valid reviews to update.");

    // Perform the bulk update operation
    var result = await reviewCollection.BulkWriteAsync(updateRequests);

    // Return the result
    return Results.Ok($"{result.ModifiedCount} reviews updated.");
});




// Cambiar valores de usuario
app.MapPatch("/users/{id}", async (string id, User updatedUser) =>
{
    var filter = Builders<User>.Filter.Eq(u => u.Id, id);

    var updates = new List<UpdateDefinition<User>>();

    if (!string.IsNullOrEmpty(updatedUser.Name))
        updates.Add(Builders<User>.Update.Set(u => u.Name, updatedUser.Name));
    if (!string.IsNullOrEmpty(updatedUser.Email))
        updates.Add(Builders<User>.Update.Set(u => u.Email, updatedUser.Email));
    if (updatedUser.Address != null && updatedUser.Address.Any())
        updates.Add(Builders<User>.Update.Set(u => u.Address, updatedUser.Address));
    if (updatedUser.Tarjeta != null)
        updates.Add(Builders<User>.Update.Set(u => u.Tarjeta, updatedUser.Tarjeta));

    if (!updates.Any())
        return Results.BadRequest("NO FIELDS TO UPDATE");

    var updateDef = Builders<User>.Update.Combine(updates);

    var result = await usersCollection.UpdateOneAsync(filter, updateDef);

    if (result.MatchedCount == 0)
        return Results.NotFound($"Usuario con ID '{id}' no encontrado.");

    return Results.Ok($"Usuario con ID '{id}' actualizado correctamente.");
});

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



//Elimina el review
app.MapDelete("review/{id}", async (string id) => 
{
    var filter = Builders<Review>.Filter.Eq(r => r.Id, id);

    var result = await reviewCollection.DeleteOneAsync(filter);

    if (result.DeletedCount == 0)
        return Results.NotFound("COULDN'T DELETE RESTAURANT (NOT FOUND)");

    return Results.Ok($"Review: '{id}' deleted");
});


// Elimina múltiples reviews
app.MapDelete("reviews", async ( [FromBody] List<string> ids) =>
{
    // Create a filter to match documents where the 'Id' field is in the list of ids
    var filter = Builders<Review>.Filter.In(r => r.Id, ids);

    // Perform the delete operation
    var result = await reviewCollection.DeleteManyAsync(filter);

    if (result.DeletedCount == 0)
        return Results.NotFound("No reviews found to delete.");

    return Results.Ok($"{result.DeletedCount} reviews deleted.");
});
// ["6810073f6aa7cd474293f35e", "6810073f6aa7cd474293f36f"]


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
//Eliminar Varios ordenes
app.MapDelete("/orders/batch", async (HttpRequest request) =>
{
    var orderIds = await request.ReadFromJsonAsync<List<string>>();
    if (orderIds == null || orderIds.Count == 0)
        return Results.BadRequest("No order IDs provided.");

    var filter = Builders<Order>.Filter.In("_id", orderIds);
    var result = await ordersCollection.DeleteManyAsync(filter);

    if (result.DeletedCount == 0)
        return Results.NotFound("No matching orders found to delete.");

    return Results.Ok($"{result.DeletedCount} orders successfully deleted.");
});



app.Run();
