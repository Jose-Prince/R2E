<template>
  <div>
    <div v-if="routeQuery === 'sales' && sales.length > 0">
      <h1>Sales</h1>
      <v-divider></v-divider>
      <v-row>
        <v-col
          v-for="sale in sales"
          :key="sale.id.timestamp"
          cols="12"
          sm="6"
          md="4"
          lg="3"
        >
          <v-card>
            <v-card-title class="text-h6">{{ sale.name }}</v-card-title>
            <v-card-subtitle>Price: ${{ sale.totalPrice.toFixed(2) }}</v-card-subtitle>
            <v-card-text>
              <div><strong>Description:</strong> {{ sale.description }}</div>
              <div><strong>Type:</strong> {{ sale.type }}</div>
              <div v-if="sale.ingredients && sale.ingredients.length > 0">
                <strong>Ingredients:</strong> {{ sale.ingredients.join(', ') }}
              </div>
              <div><strong>Rating:</strong> {{ sale.rating }}</div>
              <v-img
                :src="sale.photoItemId"
                height="150"
                cover
                class="mt-2"
              ></v-img>
            </v-card-text>
            <v-card-actions>
              <v-btn @click="addToCart(sale)">
                Add to cart
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </div>
    <div v-else-if="routeQuery === 'restaurants' && restaurants.length > 0">
      <h1>Restaurants</h1>
      <v-divider></v-divider>
      <v-row>
        <v-col
          v-for="restaurant in restaurants"
          :key="restaurant.id.timestamp"
          cols="12"
          sm="6"
          md="4"
          lg="3"
        >
          <v-card>
            <v-card-title class="text-h6">{{ restaurant.name }}</v-card-title>
            <v-card-subtitle>{{ restaurant.location }}</v-card-subtitle>
            <v-card-text>
              <div><strong>Rating:</strong> {{ restaurant.averageRating }}</div>
              <div>
                <strong>Hours:</strong> {{ restaurant.openingTime }} - {{ restaurant.closingTime }}
              </div>
              <div>
                <strong>Styles:</strong>
                <v-chip
                  v-for="style in restaurant.styles"
                  :key="style"
                  class="ma-1"
                  size="small"
                >
                  {{ style }}
                </v-chip>
              </div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </div>
    <div v-else-if="routeQuery === 'sales' && sales.length === 0">
      <h1>Sales</h1>
      <v-divider></v-divider>
      <p>No hay ofertas disponibles.</p>
    </div>
    <div v-else-if="routeQuery === 'restaurants' && restaurants.length === 0">
      <h1>Restaurants</h1>
      <v-divider></v-divider>
      <p>No hay restaurantes disponibles.</p>
    </div>
    <div v-else>
      <h1>Inicio</h1>
      <v-divider></v-divider>
      <p>Bienvenido a la página de inicio. Selecciona 'sales' o 'restaurants' en la URL.</p>
    </div>
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router';
import { ref, onMounted } from 'vue';
import { getRestaurants, getSales } from '../controller/controller.js';

const route = useRoute();
const routeQuery = ref('');

const selectedStyles = ref([]);
const restaurants = ref([]);
const sales = ref([]);
const cart = ref([]);

onMounted(async () => {
  routeQuery.value = route.query.page;
  loadCart(); // Cargar el carrito al montar el componente

  try {
    if (routeQuery.value === 'restaurants') {
      restaurants.value = await getRestaurants();
    } else if (routeQuery.value === 'sales') {
      sales.value = await getSales();
    }
  } catch (err) {
    console.error('Error al obtener datos:', err);
  }
});

function toggleStyle(style) {
  const index = selectedStyles.value.indexOf(style);
  if (index === -1) {
    selectedStyles.value.push(style);
  } else {
    selectedStyles.value.splice(index, 1);
  }
}

function addToCart(product) {
  cart.value.push(product);
  saveCart();
}

function saveCart() {
  localStorage.setItem('Cart', JSON.stringify(cart.value));
}

function loadCart() {
  const storedCart = localStorage.getItem('Cart');
  if (storedCart) {
    cart.value = JSON.parse(storedCart);
  }
}
</script>

<style scoped>
/* Estilos existentes */
.style-display {
  border: 1px solid white;
  border-radius: 15px;
  padding: 10px;
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.style-btn {
  text-transform: none;
  border-radius: 10px;
  transition: 0.3s;
  min-width: auto;
  padding: 4px 12px;
}

.active-style {
  background-color: #1976d2;
  color: white;
  border-color: #1976d2;
}
</style>
