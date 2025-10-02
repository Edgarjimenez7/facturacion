// API Base URL - Usar siempre Render para evitar problemas de conectividad
// API Base URL - Usar siempre Render para evitar problemas de conectividad
const API_BASE_URL = 'https://sistema-facturacion-v2.onrender.com/api';

// API Service class
class ApiService {
    constructor() {
        this.baseURL = API_BASE_URL;
    }

    async request(endpoint, options = {}) {
        const url = `${this.baseURL}${endpoint}`;
        const config = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                ...options.headers,
            },
            mode: 'cors',
            credentials: 'omit',
            ...options,
        };

        try {
            const response = await fetch(url, config);

            if (!response.ok) {
                let errorText;
                try {
                    errorText = await response.text();
                } catch {
                    errorText = `HTTP ${response.status} ${response.statusText}`;
                }
                throw new Error(`HTTP ${response.status}: ${errorText}`);
            }

            // Handle empty responses
            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                return await response.json();
            }
            return null;
        } catch (error) {
            if (error.name === 'TypeError' && error.message.includes('fetch')) {
                throw new Error('Error de conectividad: No se puede conectar al servidor');
            }
            console.error(`API Error: ${error.message}`);
            throw error;
        }
    }

    // Products API
    async getProducts() {
        return this.request('/products');
    }

    async getProduct(id) {
        return this.request(`/products/${id}`);
    }

    async searchProducts(term) {
        return this.request(`/products/search/${encodeURIComponent(term)}`);
    }

    async getProductsByCategory(category) {
        return this.request(`/products/category/${encodeURIComponent(category)}`);
    }

    async createProduct(product) {
        return this.request('/products', {
            method: 'POST',
            body: JSON.stringify(product),
        });
    }

    async updateProduct(id, product) {
        return this.request(`/products/${id}`, {
            method: 'PUT',
            body: JSON.stringify(product),
        });
    }

    async deleteProduct(id) {
        return this.request(`/products/${id}`, {
            method: 'DELETE',
        });
    }

    async updateProductStock(id, stock) {
        return this.request(`/products/${id}/stock`, {
            method: 'PUT',
            body: JSON.stringify(stock),
        });
    }

    // Customers API
    async getCustomers() {
        return this.request('/customers');
    }

    async getCustomer(id) {
        return this.request(`/customers/${id}`);
    }

    async searchCustomers(term) {
        return this.request(`/customers/search/${encodeURIComponent(term)}`);
    }

    async createCustomer(customer) {
        return this.request('/customers', {
            method: 'POST',
            body: JSON.stringify(customer),
        });
    }

    async updateCustomer(id, customer) {
        return this.request(`/customers/${id}`, {
            method: 'PUT',
            body: JSON.stringify(customer),
        });
    }

    async deleteCustomer(id) {
        return this.request(`/customers/${id}`, {
            method: 'DELETE',
        });
    }

    // Invoices API
    async getInvoices() {
        return this.request('/invoices');
    }

    async getInvoice(id) {
        return this.request(`/invoices/${id}`);
    }

    async getInvoicesByCustomer(customerId) {
        return this.request(`/invoices/customer/${customerId}`);
    }

    async searchInvoices(term) {
        return this.request(`/invoices/search/${encodeURIComponent(term)}`);
    }

    async createInvoice(invoice) {
        return this.request('/invoices', {
            method: 'POST',
            body: JSON.stringify(invoice),
        });
    }

    async updateInvoiceStatus(id, status) {
        return this.request(`/invoices/${id}/status`, {
            method: 'PUT',
            body: JSON.stringify(status),
        });
    }

    async deleteInvoice(id) {
        return this.request(`/invoices/${id}`, {
            method: 'DELETE',
        });
    }

    // Reports API
    async getSalesReport(startDate = null, endDate = null) {
        let endpoint = '/reports/sales';
        const params = new URLSearchParams();

        if (startDate) params.append('startDate', startDate);
        if (endDate) params.append('endDate', endDate);

        if (params.toString()) {
            endpoint += `?${params.toString()}`;
        }

        return this.request(endpoint);
    }

    async getLowStockProducts(threshold = 5) {
        return this.request(`/reports/products/low-stock?threshold=${threshold}`);
    }

    async getTopCustomers(startDate = null, endDate = null) {
        let endpoint = '/reports/customers/top';
        const params = new URLSearchParams();

        if (startDate) params.append('startDate', startDate);
        if (endDate) params.append('endDate', endDate);

        if (params.toString()) {
            endpoint += `?${params.toString()}`;
        }

        return this.request(endpoint);
    }

    async getDailyRevenue(startDate = null, endDate = null) {
        let endpoint = '/reports/revenue/daily';
        const params = new URLSearchParams();

        if (startDate) params.append('startDate', startDate);
        if (endDate) params.append('endDate', endDate);

        if (params.toString()) {
            endpoint += `?${params.toString()}`;
        }

        return this.request(endpoint);
    }
}

// Utility functions
function formatCurrency(amount) {
    return new Intl.NumberFormat('es-ES', {
        style: 'currency',
        currency: 'EUR',
    }).format(amount);
}

function formatDate(dateString) {
    return new Date(dateString).toLocaleDateString('es-ES');
}

function formatDateTime(dateString) {
    return new Date(dateString).toLocaleString('es-ES');
}

function showToast(message, type = 'info') {
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.textContent = message;

    // Style the toast
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 12px 24px;
        background-color: ${type === 'success' ? '#10b981' : type === 'error' ? '#ef4444' : '#3b82f6'};
        color: white;
        border-radius: 6px;
        box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
        z-index: 9999;
        animation: slideInRight 0.3s ease-out;
    `;

    // Add animation styles
    if (!document.getElementById('toast-styles')) {
        const style = document.createElement('style');
        style.id = 'toast-styles';
        style.textContent = `
            @keyframes slideInRight {
                from {
                    transform: translateX(100%);
                    opacity: 0;
                }
                to {
                    transform: translateX(0);
                    opacity: 1;
                }
            }
            @keyframes slideOutRight {
                from {
                    transform: translateX(0);
                    opacity: 1;
                }
                to {
                    transform: translateX(100%);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }

    // Add to DOM
    document.body.appendChild(toast);

    // Remove after 3 seconds
    setTimeout(() => {
        toast.style.animation = 'slideOutRight 0.3s ease-in';
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }, 3000);
}

function showLoading(container) {
    container.innerHTML = '<div class="loading">Cargando datos...</div>';
}

function showError(container, message) {
    container.innerHTML = `
        <div style="text-align: center; padding: 2rem; color: var(--error-color);">
            <i class="fas fa-exclamation-triangle" style="font-size: 2rem; margin-bottom: 1rem;"></i>
            <p>Error: ${message}</p>
            <button class="btn btn-secondary" onclick="location.reload()">
                <i class="fas fa-refresh"></i> Reintentar
            </button>
        </div>
    `;
}

// Create global API instance
const api = new ApiService();