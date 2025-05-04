<template>
  <div v-if="user" style="display: flex; flex-direction: column; gap: 10px">
    <h1 @click="isEditingName = true">
      <template v-if="isEditingName">
        <input
          v-model="editName"
          type="text"
          @blur="isEditingName = false"
          @focus="isEditingName = true"
        />
      </template>
      <template v-else>
        {{ editName }}
      </template>
    </h1>

    <h2 @click="isEditingEmail = true">
      Email:
      <template v-if="isEditingEmail">
        <input
          v-model="editEmail"
          type="email"
          @blur="isEditingEmail = false"
          @focus="isEditingEmail = true"
        />
      </template>
      <template v-else>
        {{ editEmail }}
      </template>
    </h2>

    <h2>My Adress:</h2>
    <ul>
      <li v-for="(direccion, index) in myAdress" :key="index" style="display: flex; align-items: center; gap: 10px;">
        <div style="margin-bottom: 8px" class="text-h6">{{myAdress[index]}}</div>
        <v-btn @click="removeDireccion(index)">DELETE</v-btn>
      </li>
    </ul>

    <div style="display: flex; gap: 10px; margin-top: 10px;">
      <v-text-field placeholder="Write new address" variant="outlined" v-model="newDirection" />
      <v-btn style="margin-top: 10px" @click="addAddress">ADD</v-btn>
    </div>

    <h2>Payment:</h2>
    <v-table v-if="!isEditingCard">
      <thead>
        <tr>
          <th class="text-left">Number</th>
          <th class="text-left">Expiration Date</th>
          <th class="text-left">CVV</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>{{ editCardNumber }}</td>
          <td>{{ editCardExp }}</td>
          <td>{{ editCardCVV }}</td>
          <td><v-btn @click="isEditingCard = true">Edit</v-btn></td>
        </tr>
      </tbody>
    </v-table>

    <div v-else style="display: flex; flex-direction: column; gap: 10px;">
      <v-text-field v-model="editCardNumber" label="Card Number" variant="outlined" />
      <v-text-field v-model="editCardExp" label="Expiration Date" variant="outlined" />
      <v-text-field v-model="editCardCVV" label="CVV" variant="outlined" />
      <v-btn @click="isEditingCard = false">Save</v-btn>
    </div>
    <v-btn
      v-if="hasChanges"
      color="primary"
      @click="saveChanges"
    >
      Save changes
    </v-btn>
  </div>

  <div v-else>
    <p>Cargando datos del usuario...</p>
  </div>
</template>

<script setup>
import { getUser, updateUser } from '../controller/controller.js'
import { ref, onMounted, computed } from 'vue'

const user = ref(null)
const isEditingName = ref(false)
const isEditingEmail = ref(false)
const isEditingCard = ref(false)

const originalName = ref('')
const originalEmail = ref('')
const originalAdress = ref([])
const originalCard = ref({ numeracion: '', expiracion: '', cvv: '' })

const editName = ref('')
const editEmail = ref('')
const myAdress = ref([])
const newDirection = ref('')
const editCardNumber = ref('')
const editCardExp = ref('')
const editCardCVV = ref('')

onMounted(async () => {
  const id = localStorage.getItem('ID')
  if (id) {
    try {
      user.value = await getUser(id)

      originalName.value = user.value.name
      originalEmail.value = user.value.email
      originalAdress.value = [...user.value.address]
      if (user.value.tarjeta) {
        originalCard.value = { ...user.value.tarjeta }
      }

      editName.value = user.value.name
      editEmail.value = user.value.email
      myAdress.value = [...user.value.address]

      if (user.value.tarjeta) {
        editCardNumber.value = user.value.tarjeta.numeracion
        editCardExp.value = user.value.tarjeta.expiracion
        editCardCVV.value = user.value.tarjeta.cvv
      }
    } catch (err) {
      console.error('Error al obtener el usuario:', err)
    }
  } else {
    console.warn('No se encontró ID en localStorage')
  }
})

const hasChanges = computed(() => {
  const nameChanged = editName.value !== originalName.value
  const emailChanged = editEmail.value !== originalEmail.value
  const addressChanged = JSON.stringify(myAdress.value) !== JSON.stringify(originalAdress.value)
  const cardChanged =
    editCardNumber.value !== originalCard.value.numeracion ||
    editCardExp.value !== originalCard.value.expiracion ||
    editCardCVV.value !== originalCard.value.cvv

  return nameChanged || emailChanged || addressChanged || cardChanged
})

const addAddress= () => {
  const direccion = newDirection.value.trim()
  if (direccion) {
    myAdress.value.push(direccion)
    newDirection.value = ''
  }
}

const removeDireccion = (index) => {
  myAdress.value.splice(index, 1)
}

const saveChanges = async () => {
  const id = localStorage.getItem('ID')

  const updatedUser = {
    Name: editName.value,
    Email: editEmail.value,
    Address: [...myAdress.value],
    Tarjeta: {
      numeracion: editCardNumber.value,
      expiracion: editCardExp.value,
      cvv: editCardCVV.value
    }
  }

  try {
    await updateUser(id, updatedUser)

    originalName.value = updatedUser.Name
    originalEmail.value = updatedUser.Email
    originalAdress.value = [...updatedUser.Address]
    originalCard.value = { ...updatedUser.Tarjeta }
  } catch (err) {
    console.error("Failure saving user changes:", err)
  }
}
</script>

