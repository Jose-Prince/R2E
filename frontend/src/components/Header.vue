<template>
    <v-app-bar :elevation="2" class="px-4"> <!-- Padding lateral -->
        <template v-slot:prepend>
            <v-app-bar-title>READY 2 EAT</v-app-bar-title>
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