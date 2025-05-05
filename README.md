# R2E
Sistema de Gestión de Pedidos y Reseñas de Restaurantes



Backend Queries to test


POST

http://localhost:5125/reviews/6810073f6aa7cd474293f392
{
  "Cliente": "6810073f6aa7cd474293f377",
  "Calificación": 2,
  "Comentario": "Super Bad Taste in that Food condiment"
}
    // Utiliza BulkWrite (Update en Calificación, Agrega Reseña)
    // Inserta el Review Primero

POST

BULKWRITE Los productos al restaurante
http://localhost:5125/products

[
  {
    "name": "Le Ensalada",
    "basePrice": 42.04,
    "totalPrice": 32.37,
    "ingredients": ["Pollo", "Carne"],
    "rating": 2,
    "description": "American any result pressure continue data.",
    "discount": 0.23,
    "inSeason": true,
    "photoItemId": null,
    "type": "Sushi",
    "restaurantId": "6810073f6aa7cd474293f39e"
  },
  {
    "name": "Tuna Deluxe",
    "basePrice": 25.50,
    "totalPrice": 22.00,
    "ingredients": ["Tuna", "Rice", "Avocado"],
    "rating": 5,
    "description": "Fresh and rich in taste.",
    "discount": 0.10,
    "inSeason": false,
    "photoItemId": null,
    "type": "Sushi",
    "restaurantId": "6810073f6aa7cd474293f39e"
  }
]



