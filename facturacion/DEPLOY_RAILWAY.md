# Railway Deployment Instructions

## 🚀 Deploying to Railway

### Prerequisites
1. Create a Railway account at https://railway.app
2. Connect your GitHub repository

### Deployment Steps

1. **Create New Project**
   - Go to Railway Dashboard
   - Click "New Project"
   - Select "Deploy from GitHub repo"
   - Choose your repository

2. **Configure Service**
   - Railway will auto-detect .NET application
   - Build and start commands are automatic

3. **Add PostgreSQL Database**
   - In your project dashboard
   - Click "New Service" → "Database" → "PostgreSQL"
   - Railway will automatically set DATABASE_URL

4. **Environment Variables (Optional)**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ```

### Auto-Deploy
Railway automatically deploys on every git push to main branch.

### Cost
- **Starter Plan**: $5/month
- **Developer Plan**: $10/month
- Includes PostgreSQL database

### Post-Deployment
1. Your app will be available at: `https://your-app.railway.app`
2. Custom domains available on paid plans

### Benefits of Railway
- ✅ Zero configuration deployment
- ✅ Automatic SSL
- ✅ Built-in PostgreSQL
- ✅ Excellent performance
- ✅ Simple pricing