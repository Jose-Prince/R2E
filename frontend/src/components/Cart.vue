<template>
  <v-container fluid class="fill-height">
    <v-main>
      <h1>My Cart</h1>
      <v-divider></v-divider>
      <div v-if="cartItems.length > 0">
        <div v-for="(item, index) in cartItems" :key="index" class="mb-4">
          <v-card>
            <v-row class="align-center">
              <v-col cols="12" md="8">
                <v-card-title class="text-h6">{{ item.name }}</v-card-title>
                <v-card-subtitle>
                  Price: ${{ item.totalPrice ? item.totalPrice.toFixed(2) : 'N/A' }}
                </v-card-subtitle>
                <v-card-text>
                  <div><strong>Description:</strong> {{ item.description }}</div>
                  <div><strong>Type:</strong> {{ item.type }}</div>
                  <div v-if="item.ingredients && item.ingredients.length > 0">
                    <strong>Ingredients:</strong> {{ item.ingredients.join(', ') }}
                  </div>
                </v-card-text>
                <v-card-actions>
                  <v-btn color="error" @click="removeFromCart(index)">
                    Remove
                  </v-btn>
                </v-card-actions>
              </v-col>
              <v-col cols="12" md="4" class="d-flex justify-end">
                <v-img
                  v-if="item.photoItemId"
                  :src="item.photoItemId"
                  height="150"
                  contain
                  class="mt-2"
                ></v-img>
              </v-col>
            </v-row>
          </v-card>
        </div>
      </div>
      <div v-else>
        <p>No food selected to order.</p>
      </div>
    </v-main>

    <v-footer fixed app>
      <v-container class="py-2">
        <v-row justify="space-between" align="center">
          <div>
            <strong>Total Items:</strong> {{ cartItems.length }}
            <strong class="ml-4">Total Price:</strong> ${{ calculateTotalPrice().toFixed(2) }}
          </div>
          <v-btn color="primary" @click="placeOrder" :disabled="cartItems.length === 0">
            Order now
          </v-btn>
        </v-row>
      </v-container>
    </v-footer>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { createOrder } from '../controller/controller.js'

const cartItems = ref([]);

onMounted(() => {
  loadCart();
});

function loadCart() {
  const storedCart = localStorage.getItem('Cart');
  if (storedCart) {
    cartItems.value = JSON.parse(storedCart);
  }
}

function removeFromCart(index) {
  cartItems.value.splice(index, 1);
  saveCart();
}

function saveCart() {
  localStorage.setItem('Cart', JSON.stringify(cartItems.value));
}

function calculateTotalPrice() {
  return cartItems.value.reduce((total, item) => {
    return total + (item.totalPrice || 0);
  }, 0);
}

function placeOrder() {

  const newOrder = {
    ClienteId: localStorage.getItem('ID'),
    TotalAPagar: calculateTotalPrice(),
    Carrito: cartItems.value.map(item => item.id)
  }
  if (cartItems.value.length > 0) {
    cartItems.value = [];
    saveCart();
    createOrder(newOrder)
    alert('¡Pedido realizado!'); // Replace with your actual order processing logic
  }
}
</script>
