import pandas as pd
import random
from faker import Faker
from datetime import datetime
from bson import ObjectId
from dotenv import load_dotenv
from pymongo import MongoClient
import os

load_dotenv()  # Loads variables from .env into the environment

api_key = os.getenv("MONGODB_URI")


# Connect to your MongoDB (adjust the URL!)
client = MongoClient('mongodb+srv://todos:UKtyzyOQNBKr0g3y@cluster0.l0tqu1t.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0')  # or your Atlas URL
db = client['R2E']  
restaurantes_collection = db['Restaurants']

# Fetch all existing restaurant IDs
restaurant_ids = [r['_id'] for r in restaurantes_collection.find({}, {'_id': 1})]


# Initialize Faker for realistic fake data
fake = Faker()

# Helper to generate random card info
def random_card():
    return {
        'Cvv': random.randint(100, 999),
        'Numeracion': random.randint(4000000000000000, 4999999999999999),
        'Expiración': fake.date_between(start_date='+1y', end_date='+5y')
    }

# Generate 50 random users
data = []
for _ in range(50):
    user = {
        "_id": str(ObjectId()),
        'Nombre_y_Apellido': fake.name(),
        'Correo': fake.email(),
        'Dirección_entrega': [fake.address() for _ in range(random.randint(1, 3))],
        'Tarjeta': random_card()
    }
    data.append(user)

# Create DataFrame
df = pd.DataFrame(data)


# df.to_json('users.json', orient='records', indent=4, date_format='iso')





# Helper to generate restaurant data


estilos_list = [
    "Italian", "Japanese", "Mexican", "Indian", "Chinese", 
    "Mediterranean", "American", "French", "Thai", "Vegan", 
    "Spanish", "Sushi", "Barbecue", "Seafood", "Lebanese"
]


# Random latitude between -90 and 90
def random_latitude():
    return random.uniform(-90.0, 90.0)

# Random longitude between -180 and 180
def random_longitude():
    return random.uniform(-180.0, 180)
def random_restaurant():
    return {
        "_id": str(ObjectId()),
        'Nombre': fake.company(),
        'List_Reseña': [],  # Random number of reviews
        'Calificación': random.randint(1, 5),  # Random average rating (this can be calculated later)
        "address": fake.address(),
        'Ubicación': { "type": "point",
            "coordinates":[random_longitude(), random_latitude()]},
        'Foto_ubicación': None,  # Assuming you will reference an image ID from GridFS later
        'Foto_referencia': None,  # Assuming you will reference an image ID from GridFS later
        'Estilo': [random.choice(estilos_list) for _ in range(random.randint(1, 2))],  # Random cuisine types
        'Hora_apertura': "06:00:00",
        'Hora_cierre': "23:00:00"
    }

# Generate 50 random restaurants
restaurants = []
for _ in range(50):
    restaurant = random_restaurant()
    restaurants.append(restaurant)

# Create a DataFrame
df_restaurants = pd.DataFrame(restaurants)


# Save to JSON
# df_restaurants.to_json('restaurants.json', orient='records', indent=4, date_format='iso')




# Some example food types
tipos = ["Pizza", "Hamburguesa", "Taco", "Pasta", "Sushi", "Ensalada", "Postre", "Sandwich"]

# Some example ingredients
ingredientes_lista = ["Queso", "Tomate", "Lechuga", "Pollo", "Carne", "Cebolla", "Salsa", "Aguacate", "Pescado", "Pan", "Pepinillos"]

# Let's assume you already have restaurant IDs


# Function to generate a random item
def random_item():
    precio_base = round(random.uniform(5.0, 50.0), 2)  # Base price
    descuento = round(random.uniform(0.0, 0.3), 2)    # Discount between 0% and 30%
    precio_total = round(precio_base * (1 - descuento), 2)

    return {
        "_id": str(ObjectId()),
        "Nombre": fake.word().capitalize() + " " + random.choice(tipos),
        "Precio_Base": precio_base,
        "Precio_Total": precio_total,
        "Ingredientes": random.sample(ingredientes_lista, random.randint(2, 5)),
        "Calificación": random.randint(1, 5),
        "Descripción": fake.sentence(nb_words=10),
        "Descuento": descuento,
        "Temporada": random.choice([True, False]),
        "Foto_articulo": fake.image_url(),
        "Tipo": random.choice(tipos),
        "Restaurante": str(random.choice(restaurant_ids))
    }

# Generate 50 random items
items = [random_item() for _ in range(50)]


df = pd.DataFrame(items)

# Save to JSON
df.to_json('products.json', orient='records', indent=4, force_ascii=False)


users_collection = db['Users']
articulos_collection = db['Products']

# Fetch all IDs
user_ids = [u['_id'] for u in users_collection.find({}, {'_id': 1})]
articulo_ids = [a['_id'] for a in articulos_collection.find({}, {'_id': 1})]


# Define order status
estados = {
    0: "No ordenado",
    1: "Ordenado",
    2: "En camino",
    3: "Entregado"
}

# Generate data
data = []
for i in range(1, 51):  # 50 orders
    carrito = random.sample(articulo_ids, random.randint(1, 5))  # 1 to 5 articles

    total_pagar = 0

    # To calculate total based on articulo price (if needed later)
    # (Optional) Sum prices if you want to be realistic:
    for art_id in carrito:
        art = articulos_collection.find_one({'_id': art_id})
        total_pagar += art.get('Precio_Total', 10)  # fallback to 10 if not found
    # But for now, random:
    # total_pagar = round(random.uniform(10.0, 250.0), 2)

    order = {
        "_id": str(ObjectId()),
        "No_orden": i,
        "Timestamp": fake.date_time_this_year(),
        "Total_a_pagar": total_pagar,
        "Carrito": [(str(c)) for c in carrito],
        "Estado": random.choice(list(estados.keys())),
        "Cliente": str(random.choice(user_ids)),
        "Notas": fake.sentence(nb_words=10)
    }
    data.append(order)

# Create DataFrame
df = pd.DataFrame(data)

# Save to JSON
df.to_json('ordenes.json', orient='records', indent=4, force_ascii=False, )



reseñas_data = []

for _ in range(100):  # let's create 100 reviews
    reseña = {
        "_id": str(ObjectId()),
        "Restaurante": str(random.choice(restaurant_ids)),  # convert ObjectId to string
        "Cliente": str(random.choice(user_ids)),
        "Calificación": random.randint(1, 5),
        "Comentario": fake.sentence(nb_words=12)
    }
    reseñas_data.append(reseña)

# Create DataFrame
reseñas_df = pd.DataFrame(reseñas_data)

# Save to JSON
reseñas_df.to_json('review.json', orient='records', indent=4, force_ascii=False)
