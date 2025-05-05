<template>
    <v-navigation-drawer
        v-model="drawer"
        temporary
      >

      <v-list density="compact" nav>
        <v-list-item
          prepend-icon="mdi-home"
          title="Home"
          @click="updateParams('home')"
        ></v-list-item>
        <v-list-item
          prepend-icon="mdi-table-chair"
          title="Restaurants"
          @click="updateParams('restaurants')"
        ></v-list-item>
        <v-list-item
          prepend-icon="mdi-sale"
          title="Sales"
          @click="updateParams('sales')"
        ></v-list-item>
        <v-list-item
          prepend-icon="mdi-silverware-fork-knife"
          title="Orders"
          @click="updateParams('orders')"
        ></v-list-item>
        <v-divider></v-divider>
        <v-list-item
          prepend-icon="mdi-account"
          title="Profile"
          @click="updateParams('profile')"
        ></v-list-item>

      </v-list>
      </v-navigation-drawer>
    <v-app-bar :elevation="2" class="px-4"> <!-- Padding lateral -->
        <template v-slot:prepend>
            <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
        </template>
      <div style="display: flex; width: 100%; gap: 5vw; margin-left: 1rem;">
        <v-text-field
          density="compact"
          hide-details
          single-line
          variant="outlined"
          placeholder="Search restaurant..."
          class="flex-grow-1"
        ></v-text-field>
        <v-icon-btn color="primary" icon="mdi-cart-outline" variant="outlined" @click="updateParams('cart')"></v-icon-btn>
      </div>
    </v-app-bar>
</template>

<script>
export default  {
  name: 'Header'
}
</script>

<script setup>
import { useRouter, useRoute } from 'vue-router'
import { ref } from 'vue'

const drawer = ref(null)

const router = useRouter()
const route = useRoute()

function updateParams(newParam) {
  router.replace({
    query: {
      page: newParam
    }
  })
}

</script>
