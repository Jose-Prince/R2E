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

export async function updateOrderStatus(orderId, newState) {
  try {
    const url = `http://localhost:5125/orders/${orderId}/status`;
    const response = await fetch(url, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ estado: newState }), // Assuming 'EstadoWrapper' in C# has a property 'Estado'
    });

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error(`Order with id ${orderId} not found.`);
      } else {
        const errorData = await response.json();
        throw new Error(`Error updating order status: ${response.status} - ${errorData.message || 'Unknown error'}`);
      }
    }

    return await response.text(); // The C# endpoint returns a success message as text
  } catch (error) {
    console.error(`Error updating order ${orderId} status:`, error);
    throw error;
  }
}

export async function addRestaurantReviews(restaurantId, reviews) {
  try {
    const url = `http://localhost:5125/reviews/${restaurantId}`;
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(reviews),
    });

    if (!response.ok) {
      let errorMessage = `Error adding reviews: ${response.status}`;
      try {
        const errorData = await response.json();
        errorMessage += ` - ${errorData.message || 'Unknown error'}`;
      } catch (jsonError) {
        // If JSON parsing fails, just use the basic error message
      }
      if (response.status === 400) {
        throw new Error(errorMessage);
      } else if (response.status === 404) {
        throw new Error(errorMessage);
      } else {
        throw new Error(errorMessage);
      }
    }

    return await response.json();
  } catch (error) {
    console.error(`Error adding reviews for restaurant ${restaurantId}:`, error);
    throw error;
  }
}

export async function getReviewsByRestaurant(id) {
  try {
    const response = fetch(`http://localhost:5125/reviews/restaurant/${id}`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' }
    })

    if (!response.ok) {
      throw new Error(`Error getting orders: ${response.status}`)
    }

    return await response.json()
  } catch (error) {
    console.error(`Error getting reviews`, error)
  }
}
