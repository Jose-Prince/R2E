<template>
  <v-container>
    <v-tabs v-model="tab">
      <v-tab value="1">Ordered</v-tab>
      <v-tab value="2">On the way</v-tab>
      <v-tab value="3">Delivered</v-tab>
    </v-tabs>

    <v-window v-model="tab">
      <v-window-item value="1">
        <v-list v-if="tab === '1' && orderedOrders.length > 0">
          <v-list-item
            v-for="order in orderedOrders"
            :key="order.id"
            lines="3"
          >
            <v-list-item-title>Order No: {{ order.noOrden }}</v-list-item-title>
            <v-list-item-subtitle>Total: ${{ order.totalAPagar.toFixed(2) }}</v-list-item-subtitle>
            <v-list-item-subtitle>Status: {{ getStatusText(order.estado) }}</v-list-item-subtitle>
            <v-list-item-subtitle>Timestamp: {{ formatDate(order.timestamp) }}</v-list-item-subtitle>
            <v-list-item-subtitle>Client ID: {{ order.clienteId }}</v-list-item-subtitle>
            <v-list-item-subtitle v-if="order.notas">Notes: {{ order.notas }}</v-list-item-subtitle>
            <v-list-item-subtitle v-if="order.carrito && order.carrito.length > 0">
              Cart Items: {{ order.carrito.length }} item(s)
            </v-list-item-subtitle>
            <v-btn @click="updateOrderState(order.id, 2)">{{order.id}}</v-btn>
          </v-list-item>
        </v-list>
        <div v-if="tab === '1' && loadingOrdered" class="text-center">Loading orders...</div>
        <div v-if="tab === '1' && errorOrdered" class="text-center text-error">Error loading orders.</div>
        <div v-if="tab === '1' && !loadingOrdered && !errorOrdered && orderedOrders.length === 0">No ordered orders.</div>
      </v-window-item>

      <v-window-item value="2">
        <v-list v-if="tab === '2' && onTheWayOrders.length > 0">
          <v-list-item
            v-for="order in onTheWayOrders"
            :key="order.id"
            lines="3"
          >
            <v-list-item-title>Order No: {{ order.noOrden }}</v-list-item-title>
            <v-list-item-subtitle>Total: ${{ order.totalAPagar.toFixed(2) }}</v-list-item-subtitle>
            <v-list-item-subtitle>Status: {{ getStatusText(order.estado) }}</v-list-item-subtitle>
            <v-list-item-subtitle>Timestamp: {{ formatDate(order.timestamp) }}</v-list-item-subtitle>
            <v-list-item-subtitle>Client ID: {{ order.clientid }}</v-list-item-subtitle>
            <v-list-item-subtitle v-if="order.notas">Notes: {{ order.notas }}</v-list-item-subtitle>
            <v-list-item-subtitle v-if="order.carrito && order.carrito.length > 0">
              Cart Items: {{ order.carrito.length }} item(s)
            </v-list-item-subtitle>
          </v-list-item>
        </v-list>
        <div v-if="tab === '2' && loadingOnTheWay" class="text-center">Loading orders...</div>
        <div v-if="tab === '2' && errorOnTheWay" class="text-center text-error">Error loading orders.</div>
        <div v-if="tab === '2' && !loadingOnTheWay && !errorOnTheWay && onTheWayOrders.length === 0">No orders on the way.</div>
      </v-window-item>

      <v-window-item value="3">
        <v-list v-if="tab === '3' && deliveredOrders.length > 0">
          <v-list-item
            v-for="order in deliveredOrders"
            :key="order.id"
            lines="3"
          >
            <v-list-item-title>Order No: {{ order.noOrden }}</v-list-item-title>
            <v-list-item-subtitle>Total: ${{ order.totalAPagar.toFixed(2) }}</v-list-item-subtitle>
            <v-list-item-subtitle>Status: {{ getStatusText(order.estado) }}</v-list-item-subtitle>
            <v-list-item-subtitle>Timestamp: {{ formatDate(order.timestamp) }}</v-list-item-subtitle>
            <v-list-item-subtitle>Client ID: {{ order.clienteId}}</v-list-item-subtitle>
            <v-list-item-subtitle v-if="order.notas">Notes: {{ order.notas }}</v-list-item-subtitle>
            <v-list-item-subtitle v-if="order.carrito && order.carrito.length > 0">
              Cart Items: {{ order.carrito.length }} item(s)
            </v-list-item-subtitle>
          </v-list-item>
        </v-list>
        <div v-if="tab === '3' && loadingDelivered" class="text-center">Loading orders...</div>
        <div v-if="tab === '3' && errorDelivered" class="text-center text-error">Error loading orders.</div>
        <div v-if="tab === '3' && !loadingDelivered && !errorDelivered && deliveredOrders.length === 0">No delivered orders.</div>
      </v-window-item>
    </v-window>
  </v-container>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue';
import { getOrdersByState, updateOrderStatus } from '../controller/controller.js';

const tab = ref('1'); // Establece "Ordered" como la pestaña activa por defecto

const orderedOrders = ref([]);
const onTheWayOrders = ref([]);
const deliveredOrders = ref([]);

const loadingOrdered = ref(false);
const loadingOnTheWay = ref(false);
const loadingDelivered = ref(false);

const errorOrdered = ref(null);
const errorOnTheWay = ref(null);
const errorDelivered = ref(null);

const clientID = ref('')

async function updateOrderState(id, status) {
  try {

    await updateOrderState(id, status)
    switch (state) {
      case '1':
        orderedOrders.value = await getOrdersByState(state);
        break;
      case '2':
        onTheWayOrders.value = await getOrdersByState(state);
        break;
      case '3':
        deliveredOrders.value = await getOrdersByState(state);
        break;
    }

  } catch (error) {
    console.error('Error updatiiing order:', error)
  }
}

async function fetchOrders(state) {
  try {
    switch (state) {
      case '1':
        loadingOrdered.value = true;
        errorOrdered.value = null;
        orderedOrders.value = await getOrdersByState(state);
        break;
      case '2':
        loadingOnTheWay.value = true;
        errorOnTheWay.value = null;
        onTheWayOrders.value = await getOrdersByState(state);
        break;
      case '3':
        loadingDelivered.value = true;
        errorDelivered.value = null;
        deliveredOrders.value = await getOrdersByState(state);
        break;
    }
  } catch (error) {
    console.error('Error fetching orders:', error);
    switch (state) {
      case '1':
        errorOrdered.value = error.message || 'Failed to load orders.';
        break;
      case '2':
        errorOnTheWay.value = error.message || 'Failed to load orders.';
        break;
      case '3':
        errorDelivered.value = error.message || 'Failed to load orders.';
        break;
    }
  } finally {
    loadingOrdered.value = false;
    loadingOnTheWay.value = false;
    loadingDelivered.value = false;
  }
}

watch(tab, (newTab) => {
  fetchOrders(newTab);
});

fetchOrders(tab.value);

const formatDate = (timestamp) => {
  const date = new Date(timestamp * 1000); // Convert Unix timestamp to milliseconds
  return date.toLocaleString();
};

const getStatusText = (estado) => {
  switch (estado) {
    case 1:
      return 'Ordered';
    case 2:
      return 'On the way';
    case 3:
      return 'Delivered';
    default:
      return 'Unknown';
  }
};

onMounted(async () => {
  loadCart()
  try {
    clientID.value = localStorage.getItem('ID')
  } catch (err) {
    console.error('Error al obtener estilos:', err)
  }
})


</script>
