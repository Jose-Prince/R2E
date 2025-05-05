<template>
  <v-container>
    <v-tabs v-model="tab">
      <v-tab value="sales">Sales</v-tab>
      <v-tab value="restaurants">Restaurants</v-tab>
    </v-tabs>

    <v-window v-model="tab">
      <v-window-item value="sales">
        <SalesSection />
      </v-window-item>
      <v-window-item value="restaurants">
        <RestaurantSection />
      </v-window-item>
    </v-window>
  </v-container>
</template>

<script setup>
import { ref } from 'vue';
import SalesSection from './SalesSection.vue'; // Importa el componente de la sección de ventas
import RestaurantSection from './RestaurantSection.vue'; // Importa el componente de la sección de restaurantes

const tab = ref('sales'); // Establece la pestaña "Sales" como la activa por defecto
</script>
