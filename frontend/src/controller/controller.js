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
  try {
    const response = await fetch('http://localhost:5125/sales', {
      method: 'GET',
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`Error in request: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching sales:", error);
    throw error;
  }
}

export async function getRestaurantById(id) {
  try {
    const response = await fetch(`http://localhost:5125/restaurants/id/${id}`, {
      method: 'GET',
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      if (response.status === 404) {
        return null; // Indica que no se encontró el restaurante
      }
      throw new Error(`Error fetching restaurant: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error(`Error fetching restaurant with ID ${id}:`, error);
    throw error;
  }
}

export async function getRestaurants() {
  try {
    const response = await fetch(`http://localhost:5125/restaurants`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' }
    });

    if (!response.ok) {
      throw new Error(`Error getting restaurants: ${response.statusText}`)
    }

    return await response.json()
  } catch (error) {
    console.error(`Error getting styles:`, error);
    throw error;
  }
}

export async function createOrder(newOrder) {
  try {
    const response = await fetch('http://localhost:5125/orders', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(newOrder),
    });

    if (!response.ok) {
      const error = await response.json();
      console.error('Error creating order:', error);
      throw new Error(`Error creating order: ${response.status} - ${JSON.stringify(error)}`);
    }

    return await response.json();
  } catch (error) {
    console.error('Error creating order:', error);
    throw error;
  }
}

export async function getOrdersByState(state) {
  try {
    const response = await fetch(`http://localhost:5125/orders?stado=${state}`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' }
    })

    if (!response.ok) {
      throw new Error(`Error getting orders: ${response.status}`)
    }

    return await response.json()
  } catch (error) {
    console.error(`Error getting orders:`, error)
    throw error
  }
}
