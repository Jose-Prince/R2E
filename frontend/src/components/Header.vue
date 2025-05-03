<template>
    <v-navigation-drawer
        v-model="drawer"
        temporary
      >
      
      
      <v-list density="compact" nav>
        <v-list-item prepend-icon="mdi-home" title="Home" value="home"></v-list-item>
        <v-list-item prepend-icon="mdi-table-chair" title="Restaurants" value="restaurants"></v-list-item>
        <v-list-item prepend-icon="mdi-sale" title="Sales" value="sales"></v-list-item>
        <v-list-item prepend-icon="mdi-silverware-fork-knife" title="Orders" value="orders"></v-list-item>
        <v-divider></v-divider>
        <v-list-item
        prepend-icon="mdi-account"
        title="Profile"
        value="profile"
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
        <v-icon-btn color="primary" icon="$vuetify" variant="outlined"></v-icon-btn>
      </div>
    </v-app-bar>
</template>

<script>
export default  {
  name: 'Header'
}
</script>

<script setup>
import { ref } from 'vue'

const drawer = ref(null)

</script>