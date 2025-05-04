<template>
  <div>
    <div class="style-display">
      <template v-for="style in styles" :key="style">
        <v-btn
          :class="{ 'active-style': selectedStyles.includes(style) }"
          class="style-btn"
          variant="outlined"
          size="small"
          @click="toggleStyle(style)"
        >
          {{ style }}
        </v-btn>
      </template>
    </div>

    <h1>Sales</h1>
    <v-divider></v-divider>

    <h1>Restaurants</h1>
    <v-divider></v-divider>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getStyles } from '../controller/controller.js'

const styles = ref([])
const selectedStyles = ref([])

onMounted(async () => {
  try {
    styles.value = await getStyles()
  } catch (err) {
    console.error('Error al obtener estilos:', err)
  }
})

function toggleStyle(style) {
  const index = selectedStyles.value.indexOf(style)
  if (index === -1) {
    selectedStyles.value.push(style)
  } else {
    selectedStyles.value.splice(index, 1)
  }
}
</script>

<style scoped>
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

/* Estilo para botón activo */
.active-style {
  background-color: #1976d2;
  color: white;
  border-color: #1976d2;
}
</style>

