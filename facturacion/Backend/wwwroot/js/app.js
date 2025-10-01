// Global app state
let currentSection = 'dashboard';
let products = [];
let customers = [];
let invoices = [];
let invoiceProducts = [];

// DOM Elements
const navItems = document.querySelectorAll('.nav-item');
const sections = document.querySelectorAll('.content-section');

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    initializeNavigation();
    loadDashboard();
    setupEventListeners();
});

// Navigation
function initializeNavigation() {
    navItems.forEach(item => {
        item.addEventListener('click', function(e) {
            e.preventDefault();
            const section = this.getAttribute('data-section');
            showSection(section);
        });
    });
}

function showSection(sectionName) {
    // Update navigation
    navItems.forEach(item => {
        item.classList.remove('active');
        if (item.getAttribute('data-section') === sectionName) {
            item.classList.add('active');
        }
    });

    // Update sections
    sections.forEach(section => {
        section.classList.remove('active');
        if (section.id === sectionName) {
            section.classList.add('active');
        }
    });

    currentSection = sectionName;

    // Load section-specific data
    switch (sectionName) {
        case 'dashboard':
            loadDashboard();
            break;
        case 'products':
            loadProducts();
            break;
        case 'customers':
            loadCustomers();
            break;
        case 'invoices':
            loadInvoices();
            break;
        case 'reports':
            loadReports();
            break;
    }
}

// Dashboard
async function loadDashboard() {
    try {
        const salesReport = await api.getSalesReport();
        const lowStock = await api.getLowStockProducts();

        // Update stats
        document.getElementById('total-sales').textContent = formatCurrency(salesReport.totalSales);
        document.getElementById('total-invoices').textContent = salesReport.totalInvoices;
        document.getElementById('total-products').textContent = salesReport.totalProducts;
        document.getElementById('total-customers').textContent = salesReport.totalCustomers;

        // Top products
        const topProductsContainer = document.getElementById('top-products');
        if (salesReport.topProducts && salesReport.topProducts.length > 0) {
            topProductsContainer.innerHTML = salesReport.topProducts
                .map(product => `
                    <div style="display: flex; justify-content: space-between; padding: 0.5rem 0; border-bottom: 1px solid var(--border-color);">
                        <span>${product.productName}</span>
                        <span style="font-weight: 600;">${formatCurrency(product.totalRevenue)}</span>
                    </div>
                `).join('');
        } else {
            topProductsContainer.innerHTML = '<p style="text-align: center; color: var(--text-secondary);">No hay datos de ventas disponibles</p>';
        }

        // Low stock products
        const lowStockContainer = document.getElementById('low-stock');
        if (lowStock && lowStock.length > 0) {
            lowStockContainer.innerHTML = lowStock
                .map(product => `
                    <div style="display: flex; justify-content: space-between; padding: 0.5rem 0; border-bottom: 1px solid var(--border-color);">
                        <span>${product.name}</span>
                        <span style="color: var(--error-color); font-weight: 600;">Stock: ${product.stock}</span>
                    </div>
                `).join('');
        } else {
            lowStockContainer.innerHTML = '<p style="text-align: center; color: var(--text-secondary);">Todos los productos tienen stock suficiente</p>';
        }
    } catch (error) {
        showToast('Error cargando dashboard: ' + error.message, 'error');
        console.error('Dashboard error:', error);
    }
}

// Products
async function loadProducts() {
    const tbody = document.querySelector('#products-table tbody');
    showLoading(tbody);

    try {
        products = await api.getProducts();
        renderProductsTable();
        loadProductCategories();
    } catch (error) {
        showError(tbody, error.message);
        showToast('Error cargando productos: ' + error.message, 'error');
    }
}

function renderProductsTable(data = products) {
    const tbody = document.querySelector('#products-table tbody');

    if (data.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-secondary);">No hay productos disponibles</td></tr>';
        return;
    }

    tbody.innerHTML = data.map(product => `
        <tr>
            <td>${product.id}</td>
            <td>${product.name}</td>
            <td>${product.category}</td>
            <td>${formatCurrency(product.price)}</td>
            <td>
                <span style="color: ${product.stock <= 5 ? 'var(--error-color)' : 'var(--text-primary)'}">
                    ${product.stock}
                </span>
            </td>
            <td>
                <button class="btn btn-sm btn-secondary" onclick="editProduct(${product.id})">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="deleteProduct(${product.id})">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

async function loadProductCategories() {
    const categorySelect = document.getElementById('product-category');
    const categories = [...new Set(products.map(p => p.category).filter(c => c))];

    categorySelect.innerHTML = '<option value="">Todas las categorías</option>' +
        categories.map(cat => `<option value="${cat}">${cat}</option>`).join('');
}

function openProductModal(productId = null) {
    const modal = document.getElementById('product-modal');
    const title = document.getElementById('product-modal-title');
    const form = document.getElementById('product-form');

    // Reset form
    form.reset();
    document.getElementById('product-id').value = '';

    if (productId) {
        title.textContent = 'Editar Producto';
        loadProductForEdit(productId);
    } else {
        title.textContent = 'Nuevo Producto';
    }

    modal.style.display = 'block';
}

async function loadProductForEdit(productId) {
    try {
        const product = await api.getProduct(productId);

        document.getElementById('product-id').value = product.id;
        document.getElementById('product-name').value = product.name;
        document.getElementById('product-description').value = product.description;
        document.getElementById('product-price').value = product.price;
        document.getElementById('product-stock').value = product.stock;
        document.getElementById('product-category-input').value = product.category;
    } catch (error) {
        showToast('Error cargando producto: ' + error.message, 'error');
    }
}

function editProduct(id) {
    openProductModal(id);
}

async function deleteProduct(id) {
    if (!confirm('¿Estás seguro de que quieres eliminar este producto?')) {
        return;
    }

    try {
        await api.deleteProduct(id);
        showToast('Producto eliminado exitosamente', 'success');
        loadProducts();
    } catch (error) {
        showToast('Error eliminando producto: ' + error.message, 'error');
    }
}

// Customers
async function loadCustomers() {
    const tbody = document.querySelector('#customers-table tbody');
    showLoading(tbody);

    try {
        customers = await api.getCustomers();
        renderCustomersTable();
    } catch (error) {
        showError(tbody, error.message);
        showToast('Error cargando clientes: ' + error.message, 'error');
    }
}

function renderCustomersTable(data = customers) {
    const tbody = document.querySelector('#customers-table tbody');

    if (data.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-secondary);">No hay clientes disponibles</td></tr>';
        return;
    }

    tbody.innerHTML = data.map(customer => `
        <tr>
            <td>${customer.id}</td>
            <td>${customer.name}</td>
            <td>${customer.email}</td>
            <td>${customer.phone || '-'}</td>
            <td>${customer.documentType} ${customer.documentNumber}</td>
            <td>
                <button class="btn btn-sm btn-secondary" onclick="editCustomer(${customer.id})">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="deleteCustomer(${customer.id})">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function openCustomerModal(customerId = null) {
    const modal = document.getElementById('customer-modal');
    const title = document.getElementById('customer-modal-title');
    const form = document.getElementById('customer-form');

    // Reset form
    form.reset();
    document.getElementById('customer-id').value = '';

    if (customerId) {
        title.textContent = 'Editar Cliente';
        loadCustomerForEdit(customerId);
    } else {
        title.textContent = 'Nuevo Cliente';
    }

    modal.style.display = 'block';
}

async function loadCustomerForEdit(customerId) {
    try {
        const customer = await api.getCustomer(customerId);

        document.getElementById('customer-id').value = customer.id;
        document.getElementById('customer-name').value = customer.name;
        document.getElementById('customer-email').value = customer.email;
        document.getElementById('customer-phone').value = customer.phone;
        document.getElementById('customer-address').value = customer.address;
        document.getElementById('customer-document-type').value = customer.documentType;
        document.getElementById('customer-document-number').value = customer.documentNumber;
    } catch (error) {
        showToast('Error cargando cliente: ' + error.message, 'error');
    }
}

function editCustomer(id) {
    openCustomerModal(id);
}

async function deleteCustomer(id) {
    if (!confirm('¿Estás seguro de que quieres eliminar este cliente?')) {
        return;
    }

    try {
        await api.deleteCustomer(id);
        showToast('Cliente eliminado exitosamente', 'success');
        loadCustomers();
    } catch (error) {
        showToast('Error eliminando cliente: ' + error.message, 'error');
    }
}

// Invoices
async function loadInvoices() {
    const tbody = document.querySelector('#invoices-table tbody');
    showLoading(tbody);

    try {
        invoices = await api.getInvoices();
        renderInvoicesTable();
    } catch (error) {
        showError(tbody, error.message);
        showToast('Error cargando facturas: ' + error.message, 'error');
    }
}

function renderInvoicesTable(data = invoices) {
    const tbody = document.querySelector('#invoices-table tbody');

    if (data.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-secondary);">No hay facturas disponibles</td></tr>';
        return;
    }

    tbody.innerHTML = data.map(invoice => `
        <tr>
            <td>${invoice.invoiceNumber}</td>
            <td>${invoice.customer?.name || 'N/A'}</td>
            <td>${formatDate(invoice.invoiceDate)}</td>
            <td>${formatCurrency(invoice.total)}</td>
            <td>
                <span class="status-badge status-${invoice.status.toLowerCase()}">
                    ${invoice.status}
                </span>
            </td>
            <td>
                <button class="btn btn-sm btn-secondary" onclick="viewInvoice(${invoice.id})">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-warning" onclick="updateInvoiceStatus(${invoice.id})">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="deleteInvoice(${invoice.id})">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

async function openInvoiceModal() {
    const modal = document.getElementById('invoice-modal');
    const form = document.getElementById('invoice-form');

    // Reset form
    form.reset();
    invoiceProducts = [];
    updateInvoiceTotals();

    // Load customers for dropdown
    try {
        if (customers.length === 0) {
            customers = await api.getCustomers();
        }

        const customerSelect = document.getElementById('invoice-customer');
        customerSelect.innerHTML = '<option value="">Seleccionar cliente...</option>' +
            customers.map(customer => `<option value="${customer.id}">${customer.name}</option>`).join('');

        // Load products for dropdown
        if (products.length === 0) {
            products = await api.getProducts();
        }

        const productSelect = document.getElementById('product-select');
        productSelect.innerHTML = '<option value="">Seleccionar producto...</option>' +
            products.map(product => `<option value="${product.id}">${product.name} - ${formatCurrency(product.price)} (Stock: ${product.stock})</option>`).join('');

        // Set default due date (30 days from now)
        const dueDate = new Date();
        dueDate.setDate(dueDate.getDate() + 30);
        document.getElementById('invoice-due-date').value = dueDate.toISOString().split('T')[0];

        modal.style.display = 'block';
    } catch (error) {
        showToast('Error preparando factura: ' + error.message, 'error');
    }
}

function addProductToInvoice() {
    const productSelect = document.getElementById('product-select');
    const productId = parseInt(productSelect.value);

    if (!productId) {
        showToast('Por favor selecciona un producto', 'warning');
        return;
    }

    const product = products.find(p => p.id === productId);
    if (!product) {
        showToast('Producto no encontrado', 'error');
        return;
    }

    // Check if product is already in the invoice
    if (invoiceProducts.find(p => p.productId === productId)) {
        showToast('Este producto ya está en la factura', 'warning');
        return;
    }

    invoiceProducts.push({
        productId: productId,
        productName: product.name,
        quantity: 1,
        unitPrice: product.price,
        discount: 0,
        total: product.price
    });

    renderInvoiceProductsTable();
    updateInvoiceTotals();
    productSelect.value = '';
}

function renderInvoiceProductsTable() {
    const tbody = document.querySelector('#invoice-products-table tbody');

    tbody.innerHTML = invoiceProducts.map((item, index) => `
        <tr>
            <td>${item.productName}</td>
            <td>
                <input type="number" value="${item.quantity}" min="1" 
                       onchange="updateInvoiceProduct(${index}, 'quantity', this.value)"
                       style="width: 80px; padding: 0.25rem;">
            </td>
            <td>
                <input type="number" value="${item.unitPrice}" step="0.01" min="0"
                       onchange="updateInvoiceProduct(${index}, 'unitPrice', this.value)"
                       style="width: 100px; padding: 0.25rem;">
            </td>
            <td>
                <input type="number" value="${item.discount}" step="0.01" min="0" max="100"
                       onchange="updateInvoiceProduct(${index}, 'discount', this.value)"
                       style="width: 80px; padding: 0.25rem;">
            </td>
            <td>${formatCurrency(item.total)}</td>
            <td>
                <button type="button" class="btn btn-sm btn-danger" onclick="removeInvoiceProduct(${index})">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function updateInvoiceProduct(index, field, value) {
    const item = invoiceProducts[index];
    item[field] = parseFloat(value) || 0;

    // Recalculate total
    const subtotal = item.quantity * item.unitPrice;
    const discountAmount = subtotal * (item.discount / 100);
    item.total = subtotal - discountAmount;

    renderInvoiceProductsTable();
    updateInvoiceTotals();
}

function removeInvoiceProduct(index) {
    invoiceProducts.splice(index, 1);
    renderInvoiceProductsTable();
    updateInvoiceTotals();
}

function updateInvoiceTotals() {
    const subtotal = invoiceProducts.reduce((sum, item) => sum + item.total, 0);
    const tax = subtotal * 0.18; // 18% tax
    const total = subtotal + tax;

    document.getElementById('invoice-subtotal').textContent = formatCurrency(subtotal);
    document.getElementById('invoice-tax').textContent = formatCurrency(tax);
    document.getElementById('invoice-total').textContent = formatCurrency(total);
}

async function viewInvoice(id) {
    try {
        const invoice = await api.getInvoice(id);

        // Create a simple invoice view modal
        const modal = document.createElement('div');
        modal.className = 'modal';
        modal.style.display = 'block';

        modal.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <h3>Factura ${invoice.invoiceNumber}</h3>
                    <span class="close" onclick="this.closest('.modal').remove()">&times;</span>
                </div>
                <div style="padding: 1.5rem;">
                    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 2rem; margin-bottom: 2rem;">
                        <div>
                            <h4>Cliente:</h4>
                            <p>${invoice.customer.name}</p>
                            <p>${invoice.customer.email}</p>
                            <p>${invoice.customer.phone}</p>
                        </div>
                        <div>
                            <h4>Detalles:</h4>
                            <p>Fecha: ${formatDate(invoice.invoiceDate)}</p>
                            <p>Vencimiento: ${formatDate(invoice.dueDate)}</p>
                            <p>Estado: ${invoice.status}</p>
                        </div>
                    </div>
                    
                    <h4>Productos:</h4>
                    <table style="width: 100%; border-collapse: collapse; margin-bottom: 1rem;">
                        <thead>
                            <tr style="background-color: var(--background-color);">
                                <th style="padding: 0.5rem; text-align: left; border-bottom: 1px solid var(--border-color);">Producto</th>
                                <th style="padding: 0.5rem; text-align: right; border-bottom: 1px solid var(--border-color);">Cantidad</th>
                                <th style="padding: 0.5rem; text-align: right; border-bottom: 1px solid var(--border-color);">Precio Unit.</th>
                                <th style="padding: 0.5rem; text-align: right; border-bottom: 1px solid var(--border-color);">Total</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${invoice.invoiceDetails.map(detail => `
                                <tr>
                                    <td style="padding: 0.5rem; border-bottom: 1px solid var(--border-color);">${detail.product.name}</td>
                                    <td style="padding: 0.5rem; text-align: right; border-bottom: 1px solid var(--border-color);">${detail.quantity}</td>
                                    <td style="padding: 0.5rem; text-align: right; border-bottom: 1px solid var(--border-color);">${formatCurrency(detail.unitPrice)}</td>
                                    <td style="padding: 0.5rem; text-align: right; border-bottom: 1px solid var(--border-color);">${formatCurrency(detail.total)}</td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                    
                    <div style="text-align: right;">
                        <p>Subtotal: ${formatCurrency(invoice.subTotal)}</p>
                        <p>Impuesto: ${formatCurrency(invoice.tax)}</p>
                        <h3>Total: ${formatCurrency(invoice.total)}</h3>
                    </div>
                    
                    ${invoice.notes ? `<div style="margin-top: 1rem;"><h4>Notas:</h4><p>${invoice.notes}</p></div>` : ''}
                </div>
            </div>
        `;
        
        document.body.appendChild(modal);
    } catch (error) {
        showToast('Error cargando factura: ' + error.message, 'error');
    }
}

async function updateInvoiceStatus(id) {
    const status = prompt('Ingrese el nuevo estado (Pending, Paid, Cancelled):');
    if (!status) return;
    
    const validStatuses = ['Pending', 'Paid', 'Cancelled'];
    if (!validStatuses.includes(status)) {
        showToast('Estado inválido. Estados válidos: Pending, Paid, Cancelled', 'error');
        return;
    }
    
    try {
        await api.updateInvoiceStatus(id, status);
        showToast('Estado de factura actualizado', 'success');
        loadInvoices();
    } catch (error) {
        showToast('Error actualizando estado: ' + error.message, 'error');
    }
}

async function deleteInvoice(id) {
    if (!confirm('¿Estás seguro de que quieres eliminar esta factura?')) {
        return;
    }
    
    try {
        await api.deleteInvoice(id);
        showToast('Factura eliminada exitosamente', 'success');
        loadInvoices();
    } catch (error) {
        showToast('Error eliminando factura: ' + error.message, 'error');
    }
}

// Reports
async function loadReports() {
    await generateReports();
}

async function generateReports() {
    const startDate = document.getElementById('report-start-date').value;
    const endDate = document.getElementById('report-end-date').value;
    
    try {
        // Load top customers
        const topCustomers = await api.getTopCustomers(startDate, endDate);
        const topCustomersContainer = document.getElementById('top-customers');
        
        if (topCustomers.length > 0) {
            topCustomersContainer.innerHTML = topCustomers.map(customer => `
                <div style="display: flex; justify-content: space-between; padding: 0.5rem 0; border-bottom: 1px solid var(--border-color);">
                    <div>
                        <strong>${customer.customerName}</strong>
                        <br>
                        <small>${customer.invoiceCount} facturas</small>
                    </div>
                    <div style="text-align: right;">
                        <strong>${formatCurrency(customer.totalPurchases)}</strong>
                        <br>
                        <small>Promedio: ${formatCurrency(customer.averageOrderValue)}</small>
                    </div>
                </div>
            `).join('');
        } else {
            topCustomersContainer.innerHTML = '<p style="text-align: center; color: var(--text-secondary);">No hay datos de clientes disponibles</p>';
        }
        
        // Load daily revenue
        const dailyRevenue = await api.getDailyRevenue(startDate, endDate);
        const dailyRevenueContainer = document.getElementById('daily-revenue');
        
        if (dailyRevenue.length > 0) {
            dailyRevenueContainer.innerHTML = dailyRevenue.map(day => `
                <div style="display: flex; justify-content: space-between; padding: 0.5rem 0; border-bottom: 1px solid var(--border-color);">
                    <div>
                        <strong>${formatDate(day.date)}</strong>
                        <br>
                        <small>${day.invoiceCount} facturas</small>
                    </div>
                    <div style="text-align: right;">
                        <strong>${formatCurrency(day.totalRevenue)}</strong>
                        <br>
                        <small>Promedio: ${formatCurrency(day.averageOrderValue)}</small>
                    </div>
                </div>
            `).join('');
        } else {
            dailyRevenueContainer.innerHTML = '<p style="text-align: center; color: var(--text-secondary);">No hay datos de ingresos disponibles</p>';
        }
    } catch (error) {
        showToast('Error cargando reportes: ' + error.message, 'error');
    }
}

// Modal functions
function closeModal(modalId) {
    document.getElementById(modalId).style.display = 'none';
}

// Event Listeners
function setupEventListeners() {
    // Product form
    document.getElementById('product-form').addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const productId = document.getElementById('product-id').value;
        const productData = {
            name: document.getElementById('product-name').value,
            description: document.getElementById('product-description').value,
            price: parseFloat(document.getElementById('product-price').value),
            stock: parseInt(document.getElementById('product-stock').value),
            category: document.getElementById('product-category-input').value
        };
        
        try {
            if (productId) {
                productData.id = parseInt(productId);
                await api.updateProduct(productId, productData);
                showToast('Producto actualizado exitosamente', 'success');
            } else {
                await api.createProduct(productData);
                showToast('Producto creado exitosamente', 'success');
            }
            
            closeModal('product-modal');
            loadProducts();
        } catch (error) {
            showToast('Error guardando producto: ' + error.message, 'error');
        }
    });
    
    // Customer form
    document.getElementById('customer-form').addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const customerId = document.getElementById('customer-id').value;
        const customerData = {
            name: document.getElementById('customer-name').value,
            email: document.getElementById('customer-email').value,
            phone: document.getElementById('customer-phone').value,
            address: document.getElementById('customer-address').value,
            documentType: document.getElementById('customer-document-type').value,
            documentNumber: document.getElementById('customer-document-number').value
        };
        
        try {
            if (customerId) {
                customerData.id = parseInt(customerId);
                await api.updateCustomer(customerId, customerData);
                showToast('Cliente actualizado exitosamente', 'success');
            } else {
                await api.createCustomer(customerData);
                showToast('Cliente creado exitosamente', 'success');
            }
            
            closeModal('customer-modal');
            loadCustomers();
        } catch (error) {
            showToast('Error guardando cliente: ' + error.message, 'error');
        }
    });
    
    // Invoice form
    document.getElementById('invoice-form').addEventListener('submit', async function(e) {
        e.preventDefault();
        
        if (invoiceProducts.length === 0) {
            showToast('Debes agregar al menos un producto', 'warning');
            return;
        }
        
        const invoiceData = {
            customerId: parseInt(document.getElementById('invoice-customer').value),
            dueDate: document.getElementById('invoice-due-date').value,
            notes: document.getElementById('invoice-notes').value,
            details: invoiceProducts.map(item => ({
                productId: item.productId,
                quantity: item.quantity,
                unitPrice: item.unitPrice,
                discount: item.discount
            }))
        };
        
        try {
            await api.createInvoice(invoiceData);
            showToast('Factura creada exitosamente', 'success');
            closeModal('invoice-modal');
            loadInvoices();
        } catch (error) {
            showToast('Error creando factura: ' + error.message, 'error');
        }
    });
    
    // Search functionality
    document.getElementById('product-search').addEventListener('input', function() {
        const term = this.value.toLowerCase();
        const filtered = products.filter(product =>
            product.name.toLowerCase().includes(term) ||
            product.description.toLowerCase().includes(term) ||
            product.category.toLowerCase().includes(term)
        );
        renderProductsTable(filtered);
    });
    
    document.getElementById('customer-search').addEventListener('input', function() {
        const term = this.value.toLowerCase();
        const filtered = customers.filter(customer =>
            customer.name.toLowerCase().includes(term) ||
            customer.email.toLowerCase().includes(term) ||
            customer.documentNumber.toLowerCase().includes(term)
        );
        renderCustomersTable(filtered);
    });
    
    document.getElementById('invoice-search').addEventListener('input', function() {
        const term = this.value.toLowerCase();
        const filtered = invoices.filter(invoice =>
            invoice.invoiceNumber.toLowerCase().includes(term) ||
            invoice.customer?.name?.toLowerCase().includes(term)
        );
        renderInvoicesTable(filtered);
    });
    
    // Category filter
    document.getElementById('product-category').addEventListener('change', function() {
        const category = this.value;
        const filtered = category ? products.filter(p => p.category === category) : products;
        renderProductsTable(filtered);
    });
    
    // Status filter
    document.getElementById('invoice-status').addEventListener('change', function() {
        const status = this.value;
        const filtered = status ? invoices.filter(i => i.status === status) : invoices;
        renderInvoicesTable(filtered);
    });
    
    // Close modals when clicking outside
    window.addEventListener('click', function(e) {
        if (e.target.classList.contains('modal')) {
            e.target.style.display = 'none';
        }
    });
    
    // Set default report dates
    const endDate = new Date();
    const startDate = new Date();
    startDate.setMonth(startDate.getMonth() - 1);
    
    document.getElementById('report-start-date').value = startDate.toISOString().split('T')[0];
    document.getElementById('report-end-date').value = endDate.toISOString().split('T')[0];
}