# CAC E-Commerce API

A modern e-commerce API built with .NET 8, following Domain-Driven Design (DDD) principles, featuring authentication, product management, order processing, and more.

## Features

- **Domain-Driven Design (DDD)** - Clean architecture with aggregate roots and domain events
- **Authentication & Authorization** - JWT-based authentication with role-based access control (Admin/Customer)
- **Product Management** - CRUD operations for products and categories with in-memory caching
- **Order Management** - Place, view, and manage orders
- **Pagination & Sorting** - Efficient data retrieval with pagination and sorting support
- **Logging** - Comprehensive logging using Serilog
- **API Documentation** - Swagger/OpenAPI documentation

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code (optional)

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd CAC.E-Commerce
```

### 2. Configure Database Connection

Update the connection string in `CAC.Api/appsettings.json` or `CAC.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(local)\\SQLEXPRESS;Database=CACDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

**Note:** Adjust the server name, database name, and credentials according to your SQL Server setup.

### 3. Run Database Migrations

The application uses Entity Framework Core migrations. Run the following command to apply migrations:

```bash
cd CAC.Infrastrucure
dotnet ef database update --project ../CAC.Infrastrucure/CAC.Infrastrucure.csproj --startup-project ../CAC.Api/CAC.Api.csproj
```

Alternatively, if you have the EF Core tools installed globally:

```bash
dotnet ef database update --project CAC.Infrastrucure/CAC.Infrastrucure.csproj --startup-project CAC.Api/CAC.Api.csproj
```

### 4. Seeding Sample Data

The application automatically seeds sample data on startup if the database is empty. The seeder creates:

- **Users:**
  - Admin: `admin@cac.com` / `Admin@123`
  - Customers: `john.doe@example.com`, `jane.smith@example.com`, `bob.johnson@example.com` / `Customer@123`

- **Categories:** 6 categories (Electronics, Clothing, Books, Home & Garden, Sports & Outdoors, Toys & Games)

- **Products:** 30 sample products across all categories

- **Orders:** 3 sample orders with different statuses

**Note:** Seeding only runs if the database is empty (no users exist). To re-seed, delete all data from the database.

### 5. Run the Application

```bash
cd CAC.Api
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

Swagger UI will be available at: `https://localhost:5001/swagger` (in Development mode)

## Sample API Requests

### Authentication

#### 1. Register a New User

```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john.doe@example.com",
  "role": "Customer"
}
```

#### 2. Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "Customer@123"
}
```

**Response:**
```json
{
  "id": 2,
  "name": "John Doe",
  "email": "john.doe@example.com",
  "role": "Customer",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456..."
}
```

**Note:** Save the `accessToken` for authenticated requests. Include it in the `Authorization` header as: `Bearer {accessToken}`

#### 3. Refresh Token

```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "abc123def456..."
}
```

### Public Endpoints (No Authentication Required)

#### 4. Get All Products (with Pagination & Sorting)

```http
GET /api/products?pageNumber=1&pageSize=10&sortBy=Name&sortDirection=asc
```

**Query Parameters:**
- `includeInactive` (optional): Include inactive products (default: `false`)
- `pageNumber` (optional): Page number (default: `1`)
- `pageSize` (optional): Items per page (default: `10`)
- `sortBy` (optional): Field to sort by (e.g., `Name`, `Price`, `CreationDate`)
- `sortDirection` (optional): `asc` or `desc` (default: `asc`)

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "name": "Smartphone",
      "description": "Latest model smartphone with advanced features",
      "price": 699.99,
      "categoryId": 1,
      "categoryName": "Electronics",
      "stockQuantity": 50,
      "isActive": true,
      "creationDate": "2024-01-01T00:00:00Z",
      "updationDate": null
    }
  ],
  "totalCount": 30,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

#### 5. Get Product by ID

```http
GET /api/products/1
```

#### 6. Search Products

```http
GET /api/products/search?name=smartphone&pageNumber=1&pageSize=10&sortBy=Price&sortDirection=asc
```

### Customer Endpoints (Requires Customer Authentication)

**Note:** Include the JWT token in the Authorization header: `Authorization: Bearer {token}`

#### 7. Get Customer Profile

```http
GET /api/customers/me
Authorization: Bearer {token}
```

#### 8. Update Customer Profile

```http
PUT /api/customers/me
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "John Updated",
  "email": "john.updated@example.com"
}
```

#### 9. Place an Order

```http
POST /api/orders
Authorization: Bearer {token}
Content-Type: application/json

{
  "items": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 5,
      "quantity": 1
    }
  ]
}
```

#### 10. Cancel an Order

```http
POST /api/orders/1/cancel
Authorization: Bearer {token}
```

### Admin Endpoints (Requires Admin Authentication)

**Note:** Use the admin credentials: `admin@cac.com` / `Admin@123`

#### 11. Get All Categories

```http
GET /api/admin/categories
Authorization: Bearer {adminToken}
```

#### 12. Create Category

```http
POST /api/admin/categories
Authorization: Bearer {adminToken}
Content-Type: application/json

{
  "name": "New Category",
  "description": "Category description"
}
```

#### 13. Update Category

```http
PUT /api/admin/categories/1
Authorization: Bearer {adminToken}
Content-Type: application/json

{
  "id": 1,
  "name": "Updated Category",
  "description": "Updated description"
}
```

#### 14. Delete Category

```http
DELETE /api/admin/categories/1
Authorization: Bearer {adminToken}
```

#### 15. Get All Products (Admin - includes inactive)

```http
GET /api/admin/products?includeInactive=true
Authorization: Bearer {adminToken}
```

#### 16. Create Product

```http
POST /api/admin/products
Authorization: Bearer {adminToken}
Content-Type: application/json

{
  "name": "New Product",
  "description": "Product description",
  "price": 99.99,
  "categoryId": 1,
  "stockQuantity": 100
}
```

#### 17. Update Product

```http
PUT /api/admin/products/1
Authorization: Bearer {adminToken}
Content-Type: application/json

{
  "id": 1,
  "name": "Updated Product",
  "description": "Updated description",
  "price": 149.99,
  "categoryId": 1,
  "stockQuantity": 75
}
```

#### 18. Soft Delete Product

```http
DELETE /api/admin/products/1
Authorization: Bearer {adminToken}
```

#### 19. Get All Orders (Admin)

```http
GET /api/admin/orders?pageNumber=1&pageSize=10&sortBy=OrderDate&sortDirection=desc
Authorization: Bearer {adminToken}
```

#### 20. Get Order by ID (Admin)

```http
GET /api/admin/orders/1
Authorization: Bearer {adminToken}
```

#### 21. Update Order Status

```http
PUT /api/admin/orders/1/status
Authorization: Bearer {adminToken}
Content-Type: application/json

{
  "status": "Completed"
}
```

**Valid Status Values:** `Pending`, `Completed`, `Cancelled`

## Using cURL Examples

### Login and Get Token

```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "Customer@123"
  }'
```

### Get Products (with token)

```bash
curl -X GET "https://localhost:5001/api/products?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### Place Order

```bash
curl -X POST "https://localhost:5001/api/orders" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "items": [
      {
        "productId": 1,
        "quantity": 2
      }
    ]
  }'
```

## Postman Collection

A complete Postman collection is available for easy API testing. The collection includes all endpoints with pre-configured requests and examples.

**Location:** `CAC.Api/Data/CAC API.postman_collection.json`

### How to Import:

1. Open Postman
2. Click **Import** button
3. Select **File** tab
4. Navigate to `CAC.Api/Data/CAC API.postman_collection.json`
5. Click **Import**

### Configuration:

After importing, set up the collection variables:

1. Open the collection settings
2. Go to the **Variables** tab
3. Set the `baseUrl` variable to: `https://localhost:5001`
4. (Optional) Set the `apiKey` variable with your JWT token for easier testing

The collection includes:
- All authentication endpoints (Register, Login, Refresh Token)
- Public product endpoints
- Customer endpoints (Profile, Orders)
- Admin endpoints (Categories, Products, Orders management)
- Pre-configured authentication using Bearer tokens

**Note:** Some requests in the collection already have Bearer token authentication configured. You can update the token value after logging in, or use the collection-level `apiKey` variable.

## Project Structure

```
CAC.E-Commerce/
├── CAC.Api/              # Web API layer (Controllers, Program.cs)
├── CAC.Application/      # Application layer (Commands, Queries, Handlers)
├── CAC.Domain/           # Domain layer (Entities, Enums, Interfaces)
└── CAC.Infrastrucure/    # Infrastructure layer (DbContext, Migrations, DataSeeder)
```

## Technologies Used

- .NET 8
- Entity Framework Core 8
- MediatR (CQRS pattern)
- FluentValidation
- JWT Authentication
- Serilog (Logging)
- Swagger/OpenAPI
- In-Memory Caching

## License

[Your License Here]
