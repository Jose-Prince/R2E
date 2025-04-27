using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Env.Load();

app.MapGet("/", () => "Hello World!");

// Create a new client and connect to the server
var mongoClient = MongoDBConnection.Initialize(Env.GetString("MONGODB_URI"));

//ENDPOINTS:
//CREATE:
//- Crear un pedido
//- Crear reseña asociada 
//READ:
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
app.Run();
