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
client = MongoClient(api_key)  # or your Atlas URL
db = client['R2E']  
Ordenes = db['Orders']

documents = list(Ordenes.find())

for doc in documents:
    if 'Timestamp' in doc and isinstance(doc['Timestamp'], datetime):
        doc['Timestamp'] = doc['Timestamp'].strftime('%Y-%m-%d %H:%M:%S')

# Convert to DataFrame
df = pd.DataFrame(documents)

# Save to CSV
df.to_csv("ordenes.csv", index=False)


data = Ordenes.find()  # Retrieve all documents