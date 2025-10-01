# Heroku Deployment Instructions

## 🚀 Deploying to Heroku

### Prerequisites
1. Create a Heroku account at https://heroku.com
2. Install Heroku CLI: https://devcenter.heroku.com/articles/heroku-cli

### Deployment Steps

1. **Login to Heroku**
   ```bash
   heroku login
   ```

2. **Create Heroku App**
   ```bash
   heroku create tu-sistema-facturacion
   ```

3. **Add PostgreSQL Database**
   ```bash
   heroku addons:create heroku-postgresql:mini
   ```

4. **Configure Buildpacks**
   ```bash
   heroku buildpacks:set heroku/dotnet
   heroku buildpacks:add --index 1 heroku/nodejs
   ```

5. **Deploy Application**
   ```bash
   git add .
   git commit -m "Deploy to Heroku"
   git push heroku main
   ```

6. **Open Application**
   ```bash
   heroku open
   ```

### Environment Variables
The app automatically detects Heroku's DATABASE_URL for PostgreSQL connection.

### Monitoring
- View logs: `heroku logs --tail`
- Check app status: `heroku ps`

### Cost
- **Free Tier**: Limited hours per month
- **Eco Dynos**: $5/month for basic usage
- **PostgreSQL Mini**: $5/month for database

**Total Monthly Cost: ~$10/month**

### Post-Deployment
1. Your API will be available at: `https://tu-sistema-facturacion.herokuapp.com/api`
2. Frontend will be served at: `https://tu-sistema-facturacion.herokuapp.com`
3. Swagger documentation: `https://tu-sistema-facturacion.herokuapp.com/swagger`