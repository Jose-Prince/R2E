<template>
  <v-container class="fil-height" max-width="900" style="margin-left: 0; margin-right: 0">
    <div v-if="!page || page == 'home'">
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
      <UserInfo />
    </div>
    <div v-else>
      No carga
    </div>
  </v-container>
</template>

<script setup>
import { useRoute } from 'vue-router'
import { computed } from 'vue'
import UserInfo from './UserInfo.vue'
const route = useRoute()

const page = computed(() => route.query.page)
</script>
