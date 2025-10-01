# Instrucciones de Instalación - Sistema de Facturación

## Prerequisitos

### 1. Instalar .NET 6.0 SDK o superior
1. Visitar https://dotnet.microsoft.com/download
2. Descargar e instalar .NET 6.0 SDK o superior
3. Verificar instalación con: `dotnet --version`

### 2. Navegador Web Moderno
- Chrome, Firefox, Edge o Safari actualizado
- Para desarrollo local, se recomienda usar Live Server extension en VS Code

## Instalación del Proyecto

### Backend API
```powershell
# Navegar al directorio del backend
cd Backend

# Restaurar dependencias
dotnet restore

# Crear la base de datos
dotnet build

# Ejecutar el proyecto
dotnet run
```

La API estará disponible en: `https://localhost:7000`
La documentación Swagger en: `https://localhost:7000/swagger`

### Frontend
1. Abrir `Frontend/index.html` con Live Server
2. O servir con cualquier servidor web local
3. Asegurarse de que la API esté ejecutándose

## Uso del Sistema

### 1. Dashboard
- Vista general de estadísticas
- Productos más vendidos
- Productos con poco stock

### 2. Gestión de Productos
- Crear, editar, eliminar productos
- Buscar por nombre, categoría
- Control de stock

### 3. Gestión de Clientes
- Registrar clientes nuevos
- Actualizar información
- Búsqueda avanzada

### 4. Facturación
- Crear facturas con múltiples productos
- Aplicar descuentos
- Seguimiento de estados (Pendiente, Pagado, Cancelado)

### 5. Reportes
- Clientes principales
- Ingresos diarios
- Estadísticas de ventas

## Estructura de la Base de Datos

El sistema usa SQLite con Entity Framework Core:
- **Products**: Información de productos
- **Customers**: Datos de clientes  
- **Invoices**: Cabecera de facturas
- **InvoiceDetails**: Detalles de productos por factura

## API Endpoints

### Productos
- `GET /api/products` - Listar productos
- `POST /api/products` - Crear producto
- `PUT /api/products/{id}` - Actualizar producto
- `DELETE /api/products/{id}` - Eliminar producto

### Clientes
- `GET /api/customers` - Listar clientes
- `POST /api/customers` - Crear cliente
- `PUT /api/customers/{id}` - Actualizar cliente
- `DELETE /api/customers/{id}` - Eliminar cliente

### Facturas
- `GET /api/invoices` - Listar facturas
- `POST /api/invoices` - Crear factura
- `PUT /api/invoices/{id}/status` - Actualizar estado
- `DELETE /api/invoices/{id}` - Eliminar factura

### Reportes
- `GET /api/reports/sales` - Reporte de ventas
- `GET /api/reports/products/low-stock` - Productos con poco stock
- `GET /api/reports/customers/top` - Clientes principales

## Configuración

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../Database/facturacion.db"
  }
}
```

### CORS
El backend está configurado para permitir peticiones desde:
- http://localhost:3000
- http://127.0.0.1:5500
- http://localhost:5500

## Datos de Ejemplo

El sistema incluye datos de muestra:
- 5 productos en diferentes categorías
- 3 clientes de ejemplo
- Base de datos se crea automáticamente al iniciar

## Troubleshooting

### Error de CORS
- Verificar que la URL del frontend esté en la configuración CORS
- Actualizar `API_BASE_URL` en `js/api.js`

### Error de Base de Datos
- Eliminar el archivo `Database/facturacion.db` para recrear la BD
- Verificar permisos de escritura en el directorio

### Error de Conexión API
- Verificar que el backend esté ejecutándose
- Comprobar el puerto correcto en la configuración