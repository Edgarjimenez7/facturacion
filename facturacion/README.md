# Sistema de Facturación para Tienda

Un sistema completo de facturación desarrollado con .NET Core Web API y frontend HTML/CSS/JavaScript.

## Características

- **Gestión de Productos**: Agregar, editar, eliminar y buscar productos
- **Gestión de Clientes**: Registro y administración de clientes
- **Facturación**: Crear y gestionar facturas con múltiples productos
- **Reportes**: Estadísticas de ventas y reportes detallados
- **Base de datos SQLite**: Almacenamiento local sin configuración compleja

## Estructura del Proyecto

```
├── Backend/                 # API .NET Core
│   ├── Controllers/         # Controladores de API
│   ├── Models/             # Modelos de datos
│   ├── Data/               # Contexto de base de datos
│   └── Services/           # Lógica de negocio
├── Frontend/               # Frontend web
│   ├── index.html          # Página principal
│   ├── css/               # Estilos CSS
│   ├── js/                # JavaScript
│   └── pages/             # Páginas adicionales
└── Database/              # Base de datos SQLite
```

## Tecnologías Utilizadas

- **Backend**: .NET Core 6+ Web API
- **Frontend**: HTML5, CSS3, JavaScript (Vanilla)
- **Base de datos**: SQLite con Entity Framework Core
- **Autenticación**: JWT (opcional)

## Instalación y Uso

### Requisitos Previos

- .NET 6.0 SDK o superior
- Navegador web moderno

### Instalación

1. Clonar el repositorio
2. Navegar al directorio Backend
3. Ejecutar `dotnet restore`
4. Ejecutar `dotnet run`
5. Abrir el frontend en un navegador

### API Endpoints

- `GET /api/products` - Listar productos
- `POST /api/products` - Crear producto
- `GET /api/customers` - Listar clientes  
- `POST /api/customers` - Crear cliente
- `GET /api/invoices` - Listar facturas
- `POST /api/invoices` - Crear factura

## Despliegue en Producción

### 🚀 Opciones de Despliegue

#### Heroku (Recomendado para principiantes)
- **Costo**: ~$10/mes
- **Ventajas**: Fácil configuración, PostgreSQL incluida
- Ver: [DEPLOY_HEROKU.md](./DEPLOY_HEROKU.md)

#### Render (Mejor opción gratuita)
- **Costo**: Gratis o $7/mes
- **Ventajas**: Tier gratuito generoso, SSL automático
- Ver: [DEPLOY_RENDER.md](./DEPLOY_RENDER.md)

#### Railway (Más simple)
- **Costo**: $5/mes
- **Ventajas**: Zero-config, excelente rendimiento
- Ver: [DEPLOY_RAILWAY.md](./DEPLOY_RAILWAY.md)

### Archivos de Configuración Incluidos
- `Procfile` - Para Heroku
- `Dockerfile` - Para contenedores
- `package.json` - Configuración Node.js
- Soporte PostgreSQL automático

## Contribuir

1. Fork el proyecto
2. Crear una rama para la funcionalidad
3. Hacer commit de los cambios
4. Push a la rama
5. Crear un Pull Request

## Licencia

Este proyecto está bajo la Licencia MIT.