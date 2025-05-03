<template>
  <v-container class="fill-height" max-width="900">
    <div v-if="page == '' || page == 'home'"> 
      <strong>{{ page }}</strong>
    </div>
    <div v-else-if="page == 'restaurants'">
      Mostrando todos los restaurantes
    </div>
    <div v-else-if="page == 'sales'">
      Mostrando todos los sales
    </div>
    <div v-else-if="page == 'orders'">
      Mostrando todos los orders
    </div>
    <div v-else-if="page == 'profile'">
      Mostrando todos los profile
    </div>
  </v-container>
</template>

<script setup>
import { useRoute } from 'vue-router'
import { computed } from 'vue'

const route = useRoute()

const page = computed(() => route.query.page)
</script>
