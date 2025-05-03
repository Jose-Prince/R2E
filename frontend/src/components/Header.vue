<template>
    <v-app-bar :elevation="2">
        <v-text-field
            density="compact"
            hide-details
            single-line
            variant="outlined"
            placeholder="Buscar..."
        />    
        
    </v-app-bar>
</template>

<script>
export default  {
  name: 'Header'
}
</script>