# Render Deployment Instructions

## 🚀 Deploying to Render

### Prerequisites
1. Create a Render account at https://render.com
2. Connect your GitHub repository

### Deployment Steps

1. **Create Web Service on Render**
   - Go to Render Dashboard
   - Click "New +" → "Web Service"
   - Connect your GitHub repository

2. **Configure Build Settings**
   ```
   Build Command: cd Backend && dotnet restore && dotnet build
   Start Command: cd Backend && dotnet run --urls http://0.0.0.0:$PORT
   ```

3. **Environment Variables**
   Add these in Render dashboard:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ```

4. **Database Setup (Optional)**
   - Create PostgreSQL database on Render
   - Copy DATABASE_URL to your web service environment

### Auto-Deploy Configuration

```yaml
# render.yaml (optional)
services:
  - type: web
    name: sistema-facturacion
    env: docker
    dockerfilePath: ./Dockerfile
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
```

### Alternative: Manual Deploy

1. **Fork/Clone Repository**
   ```bash
   git clone https://github.com/yourusername/facturacion.git
   cd facturacion
   ```

2. **Push to Your Repository**
   ```bash
   git add .
   git commit -m "Ready for Render deployment"
   git push origin main
   ```

3. **Connect to Render**
   - Login to Render
   - Connect GitHub repository
   - Deploy automatically

### Cost
- **Free Tier**: 
  - 750 hours/month
  - Goes to sleep after 15 minutes of inactivity
- **Starter Plan**: $7/month
  - Always available
  - Custom domains

**Total Monthly Cost: Free or $7/month**

### Post-Deployment
1. Your app will be available at: `https://your-app-name.onrender.com`
2. API endpoints: `https://your-app-name.onrender.com/api`
3. Swagger docs: `https://your-app-name.onrender.com/swagger`

### Benefits of Render
- ✅ Free SSL certificates
- ✅ Automatic deploys from Git
- ✅ No credit card required for free tier
- ✅ PostgreSQL database included
- ✅ Easy scaling options