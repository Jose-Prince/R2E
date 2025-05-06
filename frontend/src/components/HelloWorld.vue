<template>
  <v-container class="fil-height" style="max-width: none; margin-left: 0; margin-right: 0">
    <div v-if="!page || page == 'home'">
      <Home />
    </div>
    <div v-else-if="page == 'restaurants'">
      <Items />
    </div>
    <div v-else-if="page == 'sales'">
      <Items />
    </div>
    <div v-else-if="page == 'orders'">
      <Orders />
    </div>
    <div v-else-if="page == 'profile'">
      <UserInfo />
    </div>
    <div v-else-if="page == 'cart'">
      <Cart />
    </div>
    <div v-else-if="page == 'admin'">
      <Admin></Admin>
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
import Home from './Home.vue'
import Items from './GridItems.vue'
import Cart from './Cart.vue'
import Orders from './Orders.vue'
const route = useRoute()

const page = computed(() => route.query.page)
</script>
