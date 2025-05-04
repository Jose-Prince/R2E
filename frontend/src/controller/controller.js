export async function getUser(id) {
  try {
    const response = await fetch(`http://localhost:5125/user/${id}`, {
      method: 'GET',
      headers: { "Content-Type": "application/json" },
    })

    if (!response.ok) {
      throw new Error(`Error in request: ${response.status}`)
    }

    return await response.json()
  } catch (error) {
    console.error("Error in data", error)
    throw error
  }
}

export async function updateUser(id, updatedUser) {
  try {
    const response = await fetch(`http://localhost:5125/users/${id}`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updatedUser)
    })

    if (!response.ok) {
      throw new Error(`Error updating: ${response.statusText}`)
    }

    return await response.json()
  } catch (error) {
    console.error("Error updating user:", error)
    throw error
  }
}

export async function getStyles() {
  try {
    const response = await fetch(`http://localhost:5125/restaurants/estilos`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' }
    });

    if (!response.ok) {
      throw new Error(`Error getting styles: ${response.statusText}`);
    }

    return await response.json();
  } catch (error) {
    console.error(`Error getting styles:`, error);
    throw error;
  }
}

export async function getSales() {

}

export async function getRestaurants() {

}
